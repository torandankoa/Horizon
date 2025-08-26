using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Areas.Admin.Controllers
{
    // Kế thừa từ AdminBaseController để được bảo vệ
    public class OrdersController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Orders
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả đơn hàng, bao gồm cả thông tin người dùng liên quan
            // Sắp xếp đơn hàng mới nhất lên đầu
            var orders = await _context.Orders
                                   .Include(o => o.User)
                                   .OrderByDescending(o => o.OrderDate)
                                   .ToListAsync();
            return View(orders);
        }

        // GET: /Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin chi tiết của một đơn hàng
            var order = await _context.Orders
                .Include(o => o.User) // Lấy thông tin người đặt
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) // Lấy chi tiết và sản phẩm tương ứng
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Admin/Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(status))
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order status has been updated successfully.";
            }

            // Quay trở lại trang chi tiết của chính đơn hàng đó
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}