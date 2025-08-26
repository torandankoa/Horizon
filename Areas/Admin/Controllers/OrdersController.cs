using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(int? month, int? year)
        {
            // Bắt đầu với một truy vấn cơ sở lấy tất cả đơn hàng
            var ordersQuery = _context.Orders
                                      .Include(o => o.User)
                                      .AsQueryable();

            // Lấy năm hiện tại để làm giá trị mặc định cho dropdown
            int currentYear = DateTime.UtcNow.Year;
            var yearList = Enumerable.Range(currentYear - 5, 6).Reverse();

            // Nếu không có năm nào được chọn, mặc định là năm hiện tại
            if (!year.HasValue)
            {
                year = currentYear;
            }

            // Áp dụng bộ lọc theo NĂM
            if (year.HasValue && year > 0)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Year == year.Value);
            }

            // Áp dụng bộ lọc theo THÁNG
            if (month.HasValue && month > 0)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Month == month.Value);
            }

            // Sắp xếp và thực thi truy vấn
            var orders = await ordersQuery.OrderByDescending(o => o.OrderDate).ToListAsync();

            // Gửi các giá trị lọc và danh sách năm về cho View để hiển thị trên form
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.YearList = new SelectList(yearList);

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