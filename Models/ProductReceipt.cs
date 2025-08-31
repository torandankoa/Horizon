using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizon.Models
{
    public class ProductReceipt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int Quantity { get; set; } // Số lượng nhập vào

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostPrice { get; set; } // Giá nhập tại thời điểm đó

        // Khóa ngoại đến Sản phẩm được nhập
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}