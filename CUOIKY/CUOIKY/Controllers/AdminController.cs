using CUOIKY.Repository;
using CUOIKY.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using CUOIKY.Models;
using System.IO;
using CUOIKY.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CUOIKY.Controllers
{
    public class AdminController : Controller
    {
        private readonly LTWDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(LTWDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductModel model)
        {
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");

            // Kiểm tra người dùng đã đăng nhập và có RoleId = 1 hay không
            if (userSession == null || userSession.RoleId != 1)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Login", "User");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Step 1: Create the Product
                var product = new Product
                {
                    ProductTypeId = model.ProductTypeId,
                    Quantity = model.Quantity,
                    StatusSell = model.StatusSell, // đảm bảo giá trị không null
                    UserName = model.UserName,
                    Password = model.Password,
                    Gmail = model.Gmail,
                    CreatedDate = DateTime.Now
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Step 2: Create the Description and link it to the Product
                var description = new Description
                {
                    Description1 = model.DescriptionText,
                    Server = model.Server,
                    Planet = model.Planet,
                    Portal = model.Portal,
                    Disciple = model.Disciple,
                    Price = model.Price,
                    CreatedDate = DateTime.Now,
                    ProductId = product.ProductId
                };

                _context.Descriptions.Add(description);
                await _context.SaveChangesAsync();

                // Step 3: Update Product with DescriptionId
                product.DescriptionId = description.DescriptionId;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                // Step 4: Handle Image Uploads
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Image");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var file in model.Images)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(folderPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            ImageName = fileName,
                            DescriptionId = description.DescriptionId,
                            CreatedDate = DateTime.Now
                        };

                        _context.Images.Add(image);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product. Please try again.");
                // Optionally log the exception here
                Console.WriteLine(ex.Message);
                return View(model);
            }
        }
        public async Task<IActionResult> ManageProducts()
        {
            var products = await _context.Products
                .Include(p => p.Description)
                .ToListAsync();

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            // Kiểm tra quyền truy cập
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null || userSession.RoleId != 1)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Login", "User");
            }

            // Lấy sản phẩm từ cơ sở dữ liệu
            var product = await _context.Products
                .Include(p => p.Description)
                .Include(p => p.Description.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Chuyển đổi sang model để hiển thị trong form
            var model = new EditProductModel
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                StatusSell = product.StatusSell ?? true,
                UserName = product.UserName,
                Password = product.Password,
                Gmail = product.Gmail,
                DescriptionText = product.Description?.Description1,
                Server = product.Description?.Server,
                Planet = product.Description?.Planet,
                Portal = product.Description?.Portal ?? false,
                Disciple = product.Description?.Disciple ?? false,
                Price = product.Description?.Price,
                ExistingImages = product.Description?.Images?.Select(i => i.ImageName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductModel model)
        {
            // Kiểm tra quyền truy cập
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null || userSession.RoleId != 1)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = await _context.Products
                .Include(p => p.Description)
                .Include(p => p.Description.Images)
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                // Cập nhật thông tin sản phẩm
                product.ProductTypeId = model.ProductTypeId;
                product.Quantity = model.Quantity;
                product.StatusSell = model.StatusSell;
                product.UserName = model.UserName;
                product.Password = model.Password;
                product.Gmail = model.Gmail;

                // Cập nhật thông tin mô tả
                if (product.Description != null)
                {
                    product.Description.Description1 = model.DescriptionText;
                    product.Description.Server = model.Server;
                    product.Description.Planet = model.Planet;
                    product.Description.Portal = model.Portal;
                    product.Description.Disciple = model.Disciple;
                    product.Description.Price = model.Price;
                }

                // Xử lý upload hình ảnh mới nếu có
                if (model.Images != null && model.Images.Any())
                {
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Image");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    foreach (var file in model.Images)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(folderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var image = new Image
                            {
                                ImageName = fileName,
                                DescriptionId = product.Description.DescriptionId,
                                CreatedDate = DateTime.Now
                            };

                            _context.Images.Add(image);
                        }
                    }
                }

                // Xóa hình ảnh nếu người dùng chọn
                if (model.ImagesToDelete != null && model.ImagesToDelete.Any())
                {
                    foreach (var imageName in model.ImagesToDelete)
                    {
                        var image = await _context.Images.FirstOrDefaultAsync(i => i.ImageName == imageName);
                        if (image != null)
                        {
                            // Xóa file vật lý
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Image", imageName);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }

                            // Xóa bản ghi trong cơ sở dữ liệu
                            _context.Images.Remove(image);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("ManageProducts");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật sản phẩm. Vui lòng thử lại.");
                Console.WriteLine(ex.Message);
                return View(model);
            }
        }
    }
}
