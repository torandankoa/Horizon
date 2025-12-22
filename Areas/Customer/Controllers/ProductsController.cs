using Horizon.Models;
using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Threading.Tasks;
using Horizon.Infrastructure;

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")] // Đánh dấu đây là controller của Area Customer
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Shop(string productCategory, string searchString, decimal? minPrice, decimal? maxPrice)
        {
            // 1. Lấy toàn bộ sản phẩm (kèm category)
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // 2. Lọc theo tên (Search)
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            // 3. Lọc theo danh mục
            if (!string.IsNullOrEmpty(productCategory))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == productCategory);
            }

            // 4. Lọc theo giá thấp nhất
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            // 5. Lọc theo giá cao nhất
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // 6. Lấy lại danh sách Category cho Sidebar
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.ProductCategory = new SelectList(categories, "Name", "Name", productCategory);

            // 7. Lưu lại giá trị lọc để hiển thị lại lên form sau khi load trang
            ViewData["CurrentSearchString"] = searchString;
            ViewData["CurrentCategory"] = productCategory;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            return View(await productsQuery.ToListAsync());
        }

        // GET: /Customer/Products/Details/5
        // GET: /Customer/Products/Details/m4a1-tactical-rifle
        [Route("product/{slug}")] // Thêm Route để URL trông đẹp hơn
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Slug == slug); // Tìm theo Slug thay vì ID

            if (product == null) return NotFound();

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