using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Kế thừa từ AdminBaseController để được bảo vệ bởi [Authorize(Roles = "Admin")]
    public class HomeController : AdminBaseController
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // "Tiêm" DbContext và UserManager vào
        public HomeController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action chính cho trang Dashboard
        public async Task<IActionResult> Index(int? year)
        {
            // --- 1. Tạo ViewModel để chứa tất cả dữ liệu ---
            var viewModel = new DashboardViewModel
            {
                // Tính toán các số liệu thống kê tổng quan
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                TotalUsers = await _userManager.Users.CountAsync() // Đếm tổng số người dùng
            };

            // --- 2. Lấy danh sách sản phẩm có tồn kho thấp ---
            viewModel.LowStockProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.Quantity <= 5) // Điều kiện: số lượng còn lại <= 5
                .OrderBy(p => p.Quantity)
                .Take(10) // Chỉ lấy 10 sản phẩm để danh sách không quá dài
                .ToListAsync();

            // --- 3. Tính toán cho Biểu đồ Tròn (Doanh số bán ra theo Danh mục) ---
            var salesByCategory = await _context.OrderDetails
                .AsNoTracking()
                .Include(od => od.Product.Category) // Join với bảng Product và Category
                .GroupBy(od => od.Product.Category.Name) // Nhóm theo tên danh mục
                .Select(group => new {
                    CategoryName = group.Key,
                    TotalQuantitySold = group.Sum(od => od.Quantity) // Tính tổng số lượng bán ra
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .ToListAsync();

            // Đổ dữ liệu đã tính vào ViewModel
            viewModel.SalesByCategoryNames = salesByCategory.Select(s => s.CategoryName).ToList();
            viewModel.SalesByCategoryCounts = salesByCategory.Select(s => s.TotalQuantitySold).ToList();

            // --- 4. Tính toán cho Biểu đồ Cột (Doanh thu theo Tháng/Năm) ---
            int selectedYear = year ?? DateTime.UtcNow.Year; // Lấy năm được chọn, nếu không thì lấy năm hiện tại

            var monthlyRevenueData = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate.Year == selectedYear) // Lọc theo năm đã chọn
                .GroupBy(o => o.OrderDate.Month) // Nhóm theo tháng
                .Select(group => new {
                    Month = group.Key,
                    Revenue = group.Sum(o => o.TotalAmount) // Tính tổng doanh thu cho mỗi tháng
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            // Chuẩn bị dữ liệu cho đủ 12 tháng (để biểu đồ luôn có 12 cột)
            viewModel.MonthlyRevenueLabels = Enumerable.Range(1, 12).Select(m => new DateTime(selectedYear, m, 1).ToString("MMM")).ToList();
            var revenueDict = monthlyRevenueData.ToDictionary(k => k.Month, v => v.Revenue);
            viewModel.MonthlyRevenueValues = Enumerable.Range(1, 12).Select(m => revenueDict.ContainsKey(m) ? revenueDict[m] : 0).ToList();

            // --- 5. Gửi dữ liệu và các lựa chọn năm cho View ---
            var yearList = Enumerable.Range(DateTime.UtcNow.Year - 5, 6).Reverse();
            ViewBag.SelectedYear = selectedYear;
            ViewBag.YearList = new SelectList(yearList, selectedYear);

            // Trả về View cùng với ViewModel chứa đầy đủ dữ liệu
            return View(viewModel);
        }
    }
}