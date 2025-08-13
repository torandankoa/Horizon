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
            // Bây giờ dòng code này sẽ hết báo lỗi màu đỏ
            var newProducts = await _context.Products
                                        .Include(p => p.Category)
                                        .OrderByDescending(p => p.CreatedAt)
                                        .Take(8)
                                        .ToListAsync();

            return View(newProducts);
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