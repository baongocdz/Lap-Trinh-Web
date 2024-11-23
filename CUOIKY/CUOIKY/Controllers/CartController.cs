using CUOIKY.Helpers;
using CUOIKY.Repository;
using Microsoft.AspNetCore.Mvc;
using CUOIKY.Helpers;
using CUOIKY.Models;
using CUOIKY.Repository.Models;

namespace CUOIKY.Controllers
{
    public class CartController : Controller
    {
        private readonly LTWDbContext _context;

        public CartController(LTWDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return RedirectToAction("Login", "User");
            }
            var Cart = SessionHelper.GetObjectFromJson<List<CartModel>>(HttpContext.Session, "Cart");
            int itemCount = Cart != null ? Cart.Count : 0;
            if (Cart == null || !Cart.Any())    
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống!";
                return View(new List<CartModel>());
            }

            // Truyền số lượng sản phẩm vào ViewBag để hiển thị trên icon giỏ hàng
            ViewBag.CartItemCount = itemCount;
            return View(Cart);
        }
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var result = _context.Products.Find(id);
            if (result == null)
            {
                return NotFound();
            }

            var Cart = SessionHelper.GetObjectFromJson<List<CartModel>>(HttpContext.Session, "Cart");
            if (Cart == null)
            {
                Cart = new List<CartModel>();
            }

            var item = Cart.FirstOrDefault(x => x.ProductID == result.ProductId);
            if (item == null)
            {
                Cart.Add(new CartModel { Product = result, ProductID = result.ProductId, Quantity = 1 });
            }
            else
            {
                item.Quantity++;
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "Cart", Cart);

            return Ok(); // Trả về 200 OK cho yêu cầu Ajax
        }


        // Cập nhật số lượng sách trong giỏ hàng
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var result = SessionHelper.GetObjectFromJson<List<CartModel>>(HttpContext.Session, "Cart");
            var item = result.FirstOrDefault(x => x.ProductID == id);

            if (item != null)
            {
                item.Quantity = quantity;
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "Cart", result);
            return RedirectToAction("Index");
        }

        // Xóa sách khỏi giỏ hàng
        public IActionResult RemoveFromCart(int id)
        {
            var result = SessionHelper.GetObjectFromJson<List<CartModel>>(HttpContext.Session, "Cart");
            var item = result.FirstOrDefault(x => x.ProductID == id);

            if (item != null)
            {
                result.Remove(item);
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "Cart", result);
            return RedirectToAction("Index");
        }
        public IActionResult GetCartItemCount()
        {
            var gioHang = SessionHelper.GetObjectFromJson<List<CartModel>>(HttpContext.Session, "Cart");

            // Đếm số lượng mặt hàng khác nhau (không quan tâm đến Quantity)
            int itemCount = gioHang != null ? gioHang.Count : 0;

            return Json(itemCount); // Trả về số lượng mặt hàng khác nhau trong giỏ hàng
        }
    }
}
