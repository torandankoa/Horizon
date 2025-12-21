using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizon.Models
{
    public class Transaction
    {
        [Key]
        public long Id { get; set; } // Dùng long để khớp với kiểu dữ liệu của VNPAY
        public string TransactionType { get; set; } = "VNPAY";
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string? TransactionNo { get; set; } // Mã giao dịch của VNPAY
        public string? ResponseCode { get; set; } // Mã phản hồi (00 là thành công)
        public string? PayDate { get; set; } // Ngày thanh toán
        public string Status { get; set; } // Trạng thái: Success / Failed
        public string? SecureHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}