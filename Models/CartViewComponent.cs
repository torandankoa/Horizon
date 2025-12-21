using Horizon.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Horizon.Models
{
    public class CartViewComponent : ViewComponent
    {
        private const string CartSessionKey = "Cart";

        public IViewComponentResult Invoke(HttpContext httpContext)
        {
            // Lấy danh sách giỏ hàng từ session hoặc cơ sở dữ liệu
            var cartItems = httpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            // Tính tổng số lượng và tổng tiền
            int totalQuantity = cartItems.Sum(item => item.Quantity);
            decimal totalPrice = cartItems.Sum(item => item.Subtotal);
            // Tạo một đối tượng ViewModel để truyền dữ liệu vào View
            var viewModel = new CartViewModel
            {
                Items = cartItems,
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice
            };
            return View(viewModel);
        }
    }
}
