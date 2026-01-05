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

        public async Task<IActionResult> Shop(string productCategory, string searchString, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            // 1. Lấy truy vấn cơ sở
            var productsQuery = _context.Products.Include(p => p.Category).Include(p => p.Reviews).AsQueryable();

            // 2. Áp dụng các bộ lọc 
            if (!string.IsNullOrEmpty(searchString)) productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            if (!string.IsNullOrEmpty(productCategory)) productsQuery = productsQuery.Where(p => p.Category.Name == productCategory);
            if (minPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            // 3. LOGIC SẮP XẾP MỚI ĐÂY CẬU ƠI
            switch (sortOrder)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "rating":
                    // Sắp xếp theo điểm đánh giá trung bình
                    productsQuery = productsQuery.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0);
                    break;
                case "newest":
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt);
                    sortOrder = "newest"; // Giá trị mặc định
                    break;
            }

            // 4. Đổ dữ liệu ra View
            ViewBag.ProductCategory = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Name", "Name", productCategory);

            // Lưu lại trạng thái để View hiển thị đúng
            ViewData["CurrentSort"] = sortOrder;
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