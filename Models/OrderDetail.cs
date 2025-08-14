using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizon.Models
{
    [Table("OrderDetails")] // Đặt tên bảng là "OrderDetails"
    public class OrderDetail
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } // Giá tại thời điểm mua

        // Khóa ngoại đến Đơn hàng
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // Khóa ngoại đến Sản phẩm
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}