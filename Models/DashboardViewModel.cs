namespace Horizon.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // <<< THÊM DÒNG NÀY VÀO ĐÂY >>>
        public int TotalUsers { get; set; }

        // Dữ liệu cho BIỂU ĐỒ TRÒN MỚI: Doanh số theo danh mục
        public List<string> SalesByCategoryNames { get; set; } = new();
        public List<int> SalesByCategoryCounts { get; set; } = new();

        // Dữ liệu cho BIỂU ĐỒ CỘT MỚI: Doanh thu theo tháng
        public List<string> MonthlyRevenueLabels { get; set; } = new();
        public List<decimal> MonthlyRevenueValues { get; set; } = new();

        // Dữ liệu cho CẢNH BÁO TỒN KHO
        public List<Product> LowStockProducts { get; set; } = new();
    }
}