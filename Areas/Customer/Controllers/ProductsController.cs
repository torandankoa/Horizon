using Horizon.Models;
using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
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

            // 2. Lấy danh sách Products và áp dụng bộ lọc
            var productsQuery = from p in _context.Products.Include(p => p.Category)
                                select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(productCategory))
            {
                productsQuery = productsQuery.Where(x => x.Category.Name == productCategory);
            }

            // 3. Truyền dữ liệu lên View
            ViewBag.ProductCategory = new SelectList(await categoryQuery.Distinct().ToListAsync());
            ViewData["CurrentSearchString"] = searchString;
            ViewData["CurrentCategory"] = productCategory;

            // Trả về View với danh sách sản phẩm đã được lọc
            return View(await productsQuery.ToListAsync());
        }

        // GET: /Customer/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy sản phẩm chính mà người dùng đang xem
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách các sản phẩm liên quan
            // Tiêu chí: Cùng danh mục, không phải là chính nó, lấy ngẫu nhiên 4 sản phẩm
            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToListAsync();

            // Gửi danh sách sản phẩm liên quan sang View bằng ViewBag
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}