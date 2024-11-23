using CUOIKY.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CUOIKY.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace CUOIKY.Controllers
{
    public class HomeController : Controller
    {
        private readonly LTWDbContext _context;

        public HomeController(LTWDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? productId, int? priceRange, int? server, int? planet, bool? disciple, bool? portal, int? page)
        {
            // L?y ng??i dùng có nhi?u ti?n nh?t
            var topUsers = _context.Users
                .OrderByDescending(u => u.Balance)
                .Take(5)
                .ToList();

            // Link YouTube video
            var youTubeLink = "https://youtu.be/f2JVY9K6aNA?si=76SiYx70BgYnTRU_"; // Thay 'your-video-id' b?ng ID video c?a b?n

            // L?y danh sách s?n ph?m và áp d?ng b? l?c tìm ki?m n?u có
            var productsQuery = _context.Products
                .Include(p => p.Description)
                .ThenInclude(d => d.Images)
                .AsQueryable();

            // Áp d?ng b? l?c tìm ki?m
            if (productId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductId == productId.Value);
            }
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 20000);
                        break;
                    case 2:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 50000);
                        break;
                    case 3:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 100000);
                        break;
                    case 4:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 200000);
                        break;
                    case 5:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 500000);
                        break;
                    case 6:
                        productsQuery = productsQuery.Where(p => p.Description.Price >= 1000000);
                        break;
                }
            }
            if (server.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Description.Server == server.Value);
            }
            if (planet.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Description.Planet == planet.Value);
            }
            if (disciple.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Description.Disciple == disciple.Value);
            }
            if (portal.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Description.Portal == portal.Value);
            }

            // S?p x?p s?n ph?m m?i nh?t ? trên cùng (n?u mu?n)
            productsQuery = productsQuery.OrderByDescending(p => p.CreatedDate);

            // Phân trang
            int pageSize = 12; // S? s?n ph?m trên m?i trang
            int pageNumber = page ?? 1;
            var products = productsQuery.ToPagedList(pageNumber, pageSize);

            // Chuy?n d? li?u vào ViewBag
            ViewBag.TopUsers = topUsers;
            ViewBag.YouTubeLink = youTubeLink;

            return View(products);
        }

        [HttpGet("Home/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            // Retrieve the product with the specified ID, including related data
            var product = await _context.Products
                .Include(p => p.Description)
                .ThenInclude(d => d.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(); // Return a 404 error if the product is not found
            }

            return View(product); // Pass the product to the Details view
        }

    }
}
