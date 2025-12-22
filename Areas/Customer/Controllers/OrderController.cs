using Horizon.Data;
using Horizon.Infrastructure;
using Horizon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims; // Cần thiết để lấy UserId của người dùng đang đăng nhập

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Chỉ người dùng đã đăng nhập mới được vào trang này
    public class OrderController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private const string CartSessionKey = "Cart";

        // "Tiêm" thêm UserManager để lấy thông tin người dùng
        public OrderController(MyDbContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration; 
        }

        // GET: /Customer/Order/Checkout
        // Action này hiển thị form để người dùng điền thông tin giao hàng
        public IActionResult Checkout()
        {
            // Trả về một đối tượng Order rỗng để form có thể binding dữ liệu
            return View(new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            // Lấy giỏ hàng từ session
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            // Bỏ qua validation cho các trường hệ thống
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty. Please add items before checking out.");
            }

            if (ModelState.IsValid && cart.Any())
            {
                // Gán các thông tin ban đầu cho đơn hàng
                order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                order.OrderDate = DateTime.UtcNow;
                order.TotalAmount = cart.Sum(item => item.Subtotal);

                // --- THAY ĐỔI QUAN TRỌNG 1: Đặt trạng thái là "Chờ thanh toán" ---
                order.Status = "Pending Payment";

                // Lặp qua giỏ hàng để tạo OrderDetail, nhưng KHÔNG trừ kho
                foreach (var cartItem in cart)
                {
                    // Kiểm tra tồn kho trước khi tạo đơn hàng
                    var productInDb = await _context.Products.FindAsync(cartItem.ProductId);
                    if (productInDb == null || productInDb.Quantity < cartItem.Quantity)
                    {
                        TempData["Error"] = $"Product '{cartItem.ProductName}' is out of stock or quantity is not available.";
                        return RedirectToAction("Index", "ShoppingCart");
                    }

                    // Tạo và thêm chi tiết đơn hàng vào đơn hàng chính
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    });
                }

                // Thêm đối tượng Order (và các OrderDetail con của nó) vào DbContext
                _context.Orders.Add(order);

                // --- THAY ĐỔI QUAN TRỌNG 2: Lưu đơn hàng vào CSDL để lấy Id ---
                await _context.SaveChangesAsync();

                // --- THAY ĐỔI QUAN TRỌNG 3: Chuyển hướng sang Action tạo URL VNPAY ---
                // Không xóa giỏ hàng, không trừ kho ở đây
                return RedirectToAction("CreatePaymentUrl", new { orderId = order.Id });
            }

            // Nếu có lỗi, quay lại form checkout
            return View(order);
        }

        // GET: /Customer/Order/OrderCompleted
        public IActionResult OrderCompleted()
        {
            // Hiển thị trang cảm ơn
            return View();
        }

        // GET: /Customer/Order/History
        public async Task<IActionResult> History()
        {
            // Lấy UserId của người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truy vấn tất cả các đơn hàng của người dùng đó, sắp xếp theo ngày mới nhất
            var orders = await _context.Orders
                                   .Where(o => o.UserId == userId)
                                   .OrderByDescending(o => o.OrderDate)
                                   .ToListAsync();

            return View(orders);
        }

        // GET: /Customer/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy UserId của người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truy vấn đơn hàng, bao gồm cả chi tiết (OrderDetails) và thông tin sản phẩm (Product)
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) // Quan trọng: Lấy cả thông tin sản phẩm trong chi tiết
                .Where(o => o.Id == id && o.UserId == userId) // Đảm bảo user chỉ xem được đơn hàng của chính mình
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound(); // Không tìm thấy đơn hàng hoặc không phải của user này
            }

            return View(order);
        }

        // GET: /Customer/Order/CreatePaymentUrl?orderId=5
        public IActionResult CreatePaymentUrl(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            // SỬA 2: Đọc đúng tên section "VnPay"
            var vnpaySettings = _configuration.GetSection("VnPay");
            var tmnCode = vnpaySettings["TmnCode"];
            var hashSecret = vnpaySettings["HashSecret"];
            var baseUrl = vnpaySettings["BaseUrl"];
            var returnUrl = vnpaySettings["ReturnUrl"];

            // Thêm kiểm tra null để gỡ rối
            if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret))
            {
                // Có thể ghi log hoặc ném ra một exception rõ ràng
                TempData["Error"] = "VNPAY configuration is missing.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var pay = new VnPayLibrary();
            var hanoiZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnp_CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hanoiZone);

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", ((long)order.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", vnp_CreateDate.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Payment for order #{order.Id}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", order.Id.ToString());

            string paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);
            return Redirect(paymentUrl);
        }

        // GET: /Customer/Order/PaymentCallback
        public async Task<IActionResult> PaymentCallback()
        {
            var vnpayData = HttpContext.Request.Query;
            var pay = new VnPayLibrary();

            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value.ToString());
                }
            }

            long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));
            string vnpResponseCode = pay.GetResponseData("vnp_ResponseCode");
            string vnpSecureHash = Request.Query["vnp_SecureHash"];

            string hashSecret = _configuration["VnPay:HashSecret"];

            bool checkSignature = pay.ValidateSignature(vnpSecureHash, hashSecret);
            if (!checkSignature)
            {
                ViewBag.Message = "Invalid signature from VNPAY.";
                return View("PaymentResult");
            }

            // Quan trọng: Phải nạp kèm OrderDetails để có danh sách sản phẩm cần trừ kho
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                ViewBag.Message = "Order not found.";
                return View("PaymentResult");
            }

            // Xử lý kết quả thanh toán...
            if (vnpResponseCode == "00")
            {
                // Chỉ xử lý nếu đơn hàng đang ở trạng thái chờ thanh toán
                // Tránh việc trừ kho 2 lần nếu VnPay gọi lại nhiều lần
                if (order.Status == "Pending Payment")
                {
                    order.Status = "Processing";

                    // >>> LOGIC TRỪ KHO ĐÂY CẬU ƠI <<<
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(detail.ProductId);
                        if (product != null)
                        {
                            // Trừ số lượng tồn kho theo số lượng khách đã mua
                            product.Quantity -= detail.Quantity;

                            // Chống trường hợp số lượng bị âm (nếu cần)
                            if (product.Quantity < 0) product.Quantity = 0;
                        }
                    }

                    // Xóa giỏ hàng trong Session
                    HttpContext.Session.Remove("Cart");

                    // Lưu toàn bộ thay đổi (Status đơn hàng + Số lượng sản phẩm)
                    await _context.SaveChangesAsync();
                }
                ViewBag.Message = $"Payment successful for order #{orderId}! Inventory has been updated.";
            }
            else
            {
                // Nếu đã từng xử lý rồi (Status không còn là Pending Payment) thì không cập nhật lại thành Failed
                if (order.Status == "Pending Payment")
                {
                    order.Status = "Payment Failed";
                    await _context.SaveChangesAsync();
                }
                ViewBag.Message = $"Payment failed for order #{orderId}. Reason: {vnpResponseCode}";
            }

            return View("PaymentResult");
        }
    }
}