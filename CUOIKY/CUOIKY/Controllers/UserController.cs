using CUOIKY.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CUOIKY.Helpers;
using CUOIKY.Models;
using CUOIKY.Repository.Models;

namespace CUOIKY.Controllers
{
    public class UserController : Controller
    {
        private readonly LTWDbContext _context;

        public UserController(LTWDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
                if (user != null)
                {
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "user", new UserModel
                    {
                        UserId = user.UserId,
                        RoleId = user.RoleId,
                        UserNameString = user.UserNameString,
                        Balance = user.Balance,
                        Gmail = user.Gmail
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Profile()
        {
            // Lấy UserId từ Session
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }

            // Truy vấn thông tin người dùng từ cơ sở dữ liệu dựa trên UserId
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userSession.UserId);

            if (user == null)
            {
                // Nếu không tìm thấy thông tin người dùng, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Login");
            }

            // Chuyển đổi thông tin người dùng sang UserModel để hiển thị trên View
            var userModel = new UserModel
            {
                UserId = user.UserId,
                RoleId = user.RoleId,
                UserNameString = user.UserNameString,
                Balance = user.Balance,
                Gmail = user.Gmail
            };

            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyProduct([FromBody] BuyProductRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Đảm bảo tính toàn vẹn dữ liệu
            try
            {
                // Lấy thông tin người dùng từ Session
                var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
                if (userSession == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để mua sản phẩm." });
                }
                int userId = userSession.UserId;

                // Lấy thông tin người dùng từ cơ sở dữ liệu
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }
                int productId = request.ProductId;
                // Lấy thông tin sản phẩm
                var product = await _context.Products
                    .Include(p => p.Description)
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.StatusSell == true);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not available or already sold." });
                }

                // Kiểm tra số dư người dùng
                if (user.Balance < product.Description.Price || user.Balance == null)
                {
                    return Json(new { success = false, message = "Insufficient balance." });
                }

                // Trừ tiền người dùng
                user.Balance -= product.Description.Price;

                // Cập nhật trạng thái sản phẩm
                product.StatusSell = false;

                // Tạo đơn hàng
                var order = new Order
                {
                    UserId = user.UserId,
                    ProductId = product.ProductId,
                    Quantity = 1,
                    TotalPrice = product.Description.Price,
                    CreatedDate = DateTime.Now
                };

                _context.Orders.Add(order);

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Product purchased successfully!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Hoàn tác nếu có lỗi
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing the purchase." });
            }
        }
        public async Task<IActionResult> TransactionHistory()
        {
            // Lấy thông tin người dùng từ Session
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }

            int userId = userSession.UserId;

            // Lấy danh sách các giao dịch của người dùng từ cơ sở dữ liệu
            var orders = await _context.Orders
                .Include(o => o.Product)
                .ThenInclude(p => p.Description)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            // Chuyển danh sách giao dịch tới View
            return View(orders);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, trả về lại View với lỗi
                return View(model);
            }

            // Kiểm tra tài khoản đã tồn tại chưa
            var existingUser = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return View(model);
            }

            // Tạo user mới với RoleId mặc định là 2
            var user = new User
            {
                UserName = model.UserName,
                Password = model.Password, // Lưu trực tiếp mật khẩu
                RoleId = 2,                // Gán RoleId mặc định là 2
                CreatedDate = DateTime.UtcNow,
                UserNameString = model.Name,
            };

            // Lưu user vào database
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            // Chuyển hướng sau khi đăng ký thành công
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult TopUp()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TopUp(decimal amount)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var userSession = SessionHelper.GetObjectFromJson<UserModel>(HttpContext.Session, "user");
            if (userSession == null)
            {
                return RedirectToAction("Login");
            }

            if (amount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Số tiền phải lớn hơn 0.");
                return View();
            }

            // Lưu yêu cầu nạp tiền vào cơ sở dữ liệu
            var topUpRequest = new Repository.Models.Transaction
            {
                UserId = userSession.UserId,
                Amount = amount,
                RequestDate = DateTime.Now,
                IsApproved = false
            };

            _context.Transactions.Add(topUpRequest);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang thông báo
            return RedirectToAction("TopUpProcessing");
        }

        public IActionResult TopUpProcessing()
        {
            return View();
        }
    }
}
