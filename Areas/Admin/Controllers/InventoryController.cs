using Horizon.Areas.Admin.Controllers;
using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class InventoryController : AdminBaseController
{
    private readonly MyDbContext _context;
    public InventoryController(MyDbContext context) { _context = context; }

    // GET: /Admin/Inventory
    public async Task<IActionResult> Index()
    {
        var history = await _context.ProductReceipts
                                .Include(p => p.Product)
                                .OrderByDescending(p => p.ReceiptDate)
                                .ToListAsync();
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