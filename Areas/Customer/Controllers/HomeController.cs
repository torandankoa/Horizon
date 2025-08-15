using Horizon.Models;
using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <<< THÊM DÒNG NÀY VÀO
using System.Linq;
using System.Threading.Tasks;


namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                NewestProducts = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(4) // Lấy 4 sản phẩm mới nhất
                    .ToListAsync(),

                SaleProducts = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => p.SalePrice != null && p.SalePrice < p.Price)
                    .OrderBy(p => Guid.NewGuid()) // Lấy ngẫu nhiên 4 sản phẩm giảm giá
                    .Take(4)
                    .ToListAsync(),

                FeaturedProducts = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => p.IsFeature == true)
                    .OrderBy(p => Guid.NewGuid()) // Lấy ngẫu nhiên 4 sản phẩm nổi bật
                    .Take(4)
                    .ToListAsync()
            };

            return View(viewModel);
        }
        // GET: /Customer/Home/About
        public IActionResult About()
        {
            return View();
        }
        // GET: /Customer/Home/Contact
        public IActionResult Contact()
        {
            return View();
        }
    }
}