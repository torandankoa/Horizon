using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizon.Models
{
    [Table("Orders")] // Đặt tên bảng là "Orders" (số nhiều)
    public class Order
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Thông tin giao hàng
        [Required(ErrorMessage = "Please enter the recipient's name")]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please enter the shipping address")]
        [MaxLength(255)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please enter a phone number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        // Khóa ngoại đến người dùng
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; } // Dùng virtual để hỗ trợ lazy loading nếu cần

        // Navigation property
        public virtual List<OrderDetail> OrderDetails { get; set; } = new();
    }
}