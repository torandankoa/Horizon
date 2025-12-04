namespace Horizon.Models
{
    internal class CartViewModel
    {
        public object Items { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}