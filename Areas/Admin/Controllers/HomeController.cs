using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Areas.Admin.Controllers
{
    [Area("Admin")]
    // Đảm bảo được bảo vệ (kế thừa từ AdminBaseController hoặc có [Authorize])
    public class HomeController : AdminBaseController
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Đảm bảo constructor đã inject đủ các service
        public HomeController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // TẠO RA MỘT ĐỐI TƯỢNG VIEWMODEL
            var viewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                TotalUsers = await _userManager.Users.CountAsync()
            };

            // Tính toán dữ liệu cho biểu đồ tròn
            var productsByCategory = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .Select(c => new {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count()
                })
                .OrderByDescending(x => x.ProductCount)
                .ToListAsync();

            foreach (var item in productsByCategory)
            {
                viewModel.ProductCategoryNames.Add(item.CategoryName);
                viewModel.ProductCategoryCounts.Add(item.ProductCount);
            }

            // Tính toán dữ liệu cho biểu đồ cột
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var dailyRevenueData = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= sevenDaysAgo)
                .GroupBy(o => o.OrderDate.Date)
                .Select(group => new {
                    Date = group.Key,
                    Revenue = group.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            foreach (var data in dailyRevenueData)
            {
                viewModel.DailyRevenueLabels.Add(data.Date.ToString("MMM dd"));
                viewModel.DailyRevenueValues.Add(data.Revenue);
            }

            // QUAN TRỌNG NHẤT: TRUYỀN VIEWMODEL SANG CHO VIEW
            return View(viewModel);
        }
    }
}