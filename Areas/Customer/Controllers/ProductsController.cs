using Horizon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Shop(string productCategory, string searchString)
        {
            // 1. Lấy danh sách Categories để đưa lên Dropdown
            IQueryable<string> categoryQuery = from c in _context.Categories
                                               orderby c.Name
                                               select c.Name;

            // 2. Lấy danh sách Products và chuẩn bị áp dụng bộ lọc
            var products = from p in _context.Products.Include(p => p.Category)
                           select p;

            // 3. Áp dụng bộ lọc nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(productCategory))
            {
                products = products.Where(x => x.Category.Name == productCategory);
            }

            // 4. Truyền dữ liệu tìm kiếm và lọc lên View để "nhớ" lựa chọn
            ViewBag.ProductCategory = new SelectList(await categoryQuery.Distinct().ToListAsync());
            ViewData["CurrentSearchString"] = searchString;

            // 5. Trả về View với danh sách sản phẩm đã được lọc
            return View(await products.ToListAsync());
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