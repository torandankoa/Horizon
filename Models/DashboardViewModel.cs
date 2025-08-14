namespace Horizon.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<string> ProductCategoryNames { get; set; } = new();
        public List<int> ProductCategoryCounts { get; set; } = new();
    }
}
