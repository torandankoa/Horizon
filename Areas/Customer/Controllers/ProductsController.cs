using Horizon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Horizon.Areas.Customer.Controllers // Namespace phải đúng
{
    [Area("Customer")] // Đánh dấu đây là controller của Area Customer
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: /Customer/Products/Shop
        public async Task<IActionResult> Shop()
        {
            var allProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(allProducts); // View này phải nằm ở /Areas/Customer/Views/Products/Shop.cshtml
        }

        // GET: /Customer/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product); // View này phải nằm ở /Areas/Customer/Views/Products/Details.cshtml
        }
    }
}