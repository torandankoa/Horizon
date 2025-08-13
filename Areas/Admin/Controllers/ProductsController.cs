using Horizon.Models;
using Horizon.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
namespace Horizon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : AdminBaseController
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string productCategory, string searchString)
        {
            // --- Phần 1: Lấy danh sách Categories để đưa lên Dropdown ---
            // Sử dụng LINQ để lấy tất cả các danh mục từ CSDL.
            IQueryable<string> categoryQuery = from c in _context.Categories
                                               orderby c.Name
                                               select c.Name;

            // --- Phần 2: Lấy danh sách Products và áp dụng bộ lọc ---
            // Bắt đầu với một truy vấn lấy tất cả sản phẩm, bao gồm cả thông tin Category liên quan.
            var products = from p in _context.Products.Include(p => p.Category)
                           select p;

            // Lọc theo tên sản phẩm nếu người dùng nhập vào ô tìm kiếm.
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }

            // Lọc theo Category nếu người dùng chọn một mục trong dropdown.
            if (!string.IsNullOrEmpty(productCategory))
            {
                products = products.Where(x => x.Category.Name == productCategory);
            }

            // --- Phần 3: Gửi dữ liệu lên View ---
            // Tạo một SelectList chứa các danh mục đã lọc (nếu có) và gửi nó qua ViewBag.
            ViewBag.ProductCategory = new SelectList(await categoryQuery.Distinct().ToListAsync());

            // Trả về View với danh sách sản phẩm đã được lọc.
            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
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

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Quantity,CategoryId,ImageUrl")] Product product, IFormFile? imageFile)
        {
            // Bây giờ chúng ta bind cả ImageUrl, nên không cần Remove khỏi ModelState

            if (ModelState.IsValid)
            {
                // Quy tắc ưu tiên:
                // 1. Ưu tiên file upload nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Logic lưu file đã làm ở bước trước
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Ghi đè bất kỳ URL nào đã nhập bằng đường dẫn file đã lưu
                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }
                // 2. Nếu không có file upload, ImageUrl đã được bind tự động từ form.
                // Chúng ta không cần làm gì thêm ở đây, vì giá trị từ ô text "ImageUrl" đã nằm trong `product.ImageUrl`.

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Quantity,ImageUrl,CreatedAt,CategoryId")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Logic ưu tiên xử lý ảnh
                    // 1. Ưu tiên file upload mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // (Bonus) Xóa ảnh cũ nếu nó tồn tại trên server của mình
                        // Điều này chỉ nên làm với các file ta đã upload, không xóa link url bên ngoài
                        if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/images/"))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu file ảnh mới (logic tương tự hàm Create)
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Ghi đè đường dẫn ảnh mới vào sản phẩm
                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }
                    // 2. Nếu không có file mới được upload, thì ImageUrl đã được bind từ ô nhập URL.
                    // Chúng ta không cần làm gì thêm, EF Core sẽ tự cập nhật giá trị mới này.

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        //GET: /Products/Shop
        public async Task<IActionResult> Shop()
        {
            var allProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(allProducts);
        }
    }
}
