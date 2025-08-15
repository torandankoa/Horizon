namespace Horizon.Models
{
    public class HomeViewModel
    {
        public List<Product> NewestProducts { get; set; } = new();
        public List<Product> SaleProducts { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
    }
}