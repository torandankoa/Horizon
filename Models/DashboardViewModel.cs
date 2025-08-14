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

        // Dữ liệu cho biểu đồ (Sản phẩm theo danh mục)
        public List<string> ProductCategoryNames { get; set; } = new();
        public List<int> ProductCategoryCounts { get; set; } = new();

        // Dữ liệu cho biểu đồ doanh thu
        public List<string> DailyRevenueLabels { get; set; } = new();
        public List<decimal> DailyRevenueValues { get; set; } = new();
    }
}