using Horizon.Areas.Admin.Controllers;
using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class InventoryController : AdminBaseController
{
    private readonly MyDbContext _context;
    public InventoryController(MyDbContext context) { _context = context; }

    // GET: /Admin/Inventory
    public async Task<IActionResult> Index(int? categoryId, string searchString)
    {
        // Bắt đầu với một truy vấn cơ sở lấy tất cả lịch sử nhập hàng
        var historyQuery = _context.ProductReceipts
                                   .Include(p => p.Product)
                                       .ThenInclude(prod => prod.Category) // Join thêm bảng Category
                                   .AsQueryable();

        // Áp dụng bộ lọc theo Tên sản phẩm
        if (!string.IsNullOrEmpty(searchString))
        {
            historyQuery = historyQuery.Where(p => p.Product.Name.Contains(searchString));
        }

        // Áp dụng bộ lọc theo Danh mục
        if (categoryId.HasValue && categoryId > 0)
        {
            historyQuery = historyQuery.Where(p => p.Product.CategoryId == categoryId.Value);
        }

        // Sắp xếp và thực thi truy vấn
        var history = await historyQuery.OrderByDescending(p => p.ReceiptDate).ToListAsync();

        // Chuẩn bị dữ liệu cho các dropdown lọc
        ViewBag.CategoryList = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId);
        ViewData["CurrentSearchString"] = searchString;

        return View(history);
    }

    // GET: /Admin/Inventory/AddStock/5
    public async Task<IActionResult> AddStock(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        ViewBag.Product = product;
        return View(new ProductReceipt { ProductId = productId });
    }

    // POST: /Admin/Inventory/AddStock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStock(ProductReceipt receipt)
    {
        if (ModelState.IsValid)
        {
            // 1. Thêm bản ghi vào lịch sử nhập hàng
            _context.ProductReceipts.Add(receipt);

            // 2. Cập nhật lại số lượng tồn kho của sản phẩm
            var product = await _context.Products.FindAsync(receipt.ProductId);
            if (product != null)
            {
                product.Quantity += receipt.Quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products"); // Quay về trang quản lý sản phẩm
        }

        // Nếu lỗi, tải lại thông tin và hiển thị lại form
        ViewBag.Product = await _context.Products.FindAsync(receipt.ProductId);
        return View(receipt);
    }
}