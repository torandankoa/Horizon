using Horizon.Data;
using Horizon.Infrastructure;
using Horizon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Chỉ người dùng đã đăng nhập mới được đặt hàng
    public class OrderController : Controller
    {
        private readonly MyDbContext _context;
        private const string CartSessionKey = "Cart";

        public OrderController(MyDbContext context)
        {
            _context = context;
        }

        // GET: /Customer/Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            if (!cart.Any())
            {
                // Nếu giỏ hàng rỗng, không cho checkout
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Lặp qua giỏ hàng để trừ kho
            foreach (var item in cart)
            {
                Product? product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    if (product.Quantity < item.Quantity)
                    {
                        // Nếu không đủ hàng trong kho
                        TempData["Error"] = $"Sorry, we only have {product.Quantity} of {product.Name} in stock.";
                        return RedirectToAction("Index", "ShoppingCart");
                    }
                    // Trừ kho
                    product.Quantity -= item.Quantity;
                }
            }

            // Lưu tất cả các thay đổi vào CSDL
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đã checkout thành công
            HttpContext.Session.Remove(CartSessionKey);

            // Hiển thị trang cảm ơn
            return View("OrderCompleted");
        }
    }
}