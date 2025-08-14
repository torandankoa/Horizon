using Horizon.Data;
using Horizon.Infrastructure;
using Horizon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Cần thiết để lấy UserId của người dùng đang đăng nhập

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Chỉ người dùng đã đăng nhập mới được vào trang này
    public class OrderController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const string CartSessionKey = "Cart";

        // "Tiêm" thêm UserManager để lấy thông tin người dùng
        public OrderController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Customer/Order/Checkout
        // Action này hiển thị form để người dùng điền thông tin giao hàng
        public IActionResult Checkout()
        {
            // Trả về một đối tượng Order rỗng để form có thể binding dữ liệu
            return View(new Order());
        }

        // POST: /Customer/Order/Checkout
        // Action này được gọi khi người dùng submit form thông tin giao hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            // Lấy giỏ hàng từ session
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            // Bỏ qua validation cho UserId và User vì chúng ta sẽ gán thủ công
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
            }

            if (ModelState.IsValid && cart.Any())
            {
                // Gán các thông tin cho đối tượng Order
                order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                order.OrderDate = DateTime.UtcNow;
                order.TotalAmount = cart.Sum(item => item.Subtotal);
                order.Status = "Processing";

                // Vòng lặp để xử lý từng item trong giỏ hàng
                foreach (var cartItem in cart)
                {
                    var productInDb = await _context.Products.FindAsync(cartItem.ProductId);
                    if (productInDb == null || productInDb.Quantity < cartItem.Quantity)
                    {
                        TempData["Error"] = $"Product '{cartItem.ProductName}' is out of stock or quantity is not available.";
                        return RedirectToAction("Index", "ShoppingCart");
                    }

                    // Trừ kho
                    productInDb.Quantity -= cartItem.Quantity;

                    // Tạo một OrderDetail mới
                    var orderDetail = new OrderDetail
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price,
                        // Không cần gán OrderId hay Order ở đây
                    };

                    // >>> BƯỚC QUAN TRỌNG NHẤT BỊ THIẾU <<<
                    // Thêm chi tiết đơn hàng vừa tạo vào danh sách của đơn hàng chính
                    order.OrderDetails.Add(orderDetail);
                }

                // Thêm đối tượng Order (đã bao gồm các OrderDetail) vào DbContext
                _context.Orders.Add(order);

                // Lưu tất cả thay đổi
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove(CartSessionKey);

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderCompleted");
            }

            // Nếu có lỗi, quay lại form
            return View(order);
        }

        // GET: /Customer/Order/OrderCompleted
        public IActionResult OrderCompleted()
        {
            // Hiển thị trang cảm ơn
            return View();
        }
    }
}