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

        public IActionResult Index(int? productId, int? priceRange, int? server, int? planet, bool? disciple, bool? portal, int? itemId, int? page)
        {
            var topUsers = _context.Users
                .OrderByDescending(u => u.Balance)
                .Take(4)
                .ToList();

            ViewBag.Items = _context.Items.ToList();

            var gameAccountsQuery = _context.Products
                .Include(p => p.Description)
                .ThenInclude(d => d.Images)
                .Where(p => p.StatusSell == true && p.ProductTypeId == 1)
                .AsQueryable();

            if (productId.HasValue)
            {
                gameAccountsQuery = gameAccountsQuery.Where(p => p.ProductId == productId.Value);
            }
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 1:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 20000);
                        break;
                    case 2:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 50000);
                        break;
                    case 3:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 100000);
                        break;
                    case 4:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 200000);
                        break;
                    case 5:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 500000);
                        break;
                    case 6:
                        gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Price >= 1000000);
                        break;
                }
            }
            if (server.HasValue)
            {
                gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Server == server.Value);
            }
            if (planet.HasValue)
            {
                gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Planet == planet.Value);
            }
            if (disciple.HasValue)
            {
                gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Disciple == disciple.Value);
            }
            if (portal.HasValue)
            {
                gameAccountsQuery = gameAccountsQuery.Where(p => p.Description.Portal == portal.Value);
            }

            // S?p x?p s?n ph?m m?i nh?t ? trên cùng (n?u mu?n)
            gameAccountsQuery = gameAccountsQuery.OrderByDescending(p => p.CreatedDate);

            // Phân trang
            int pageSize = 12; // S? s?n ph?m trên m?i trang
            int pageNumber = page ?? 1;
            var products = gameAccountsQuery.ToPagedList(pageNumber, pageSize);
            var gameAccounts = gameAccountsQuery.ToPagedList(pageNumber, pageSize);

                // Chuy?n d? li?u vào ViewBag
            ViewBag.TopUsers = topUsers;

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
