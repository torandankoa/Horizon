using Microsoft.AspNetCore.Authorization; // Thêm using này
using Horizon.Infrastructure; // Quan trọng: Thêm using cho SessionExtensions
using Horizon.Models;
using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly MyDbContext _context;
        private const string CartSessionKey = "Cart"; // Đặt tên cho key của giỏ hàng trong session

        public ShoppingCartController(MyDbContext context)
        {
            _context = context;
        }

        // >>> HÀM TRỢ GIÚP 1: LẤY GIỎ HÀNG TỪ SESSION <<<
        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        // >>> HÀM TRỢ GIÚP 2: LƯU GIỎ HÀNG VÀO SESSION <<<
        private void SaveCartItems(List<CartItem> cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart);
        }

        // GET: /Customer/ShoppingCart/Index
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session, nếu chưa có thì tạo mới một danh sách rỗng
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            return View(cart);
        }

        // POST: /Customer/ShoppingCart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // Tìm sản phẩm trong CSDL
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound(); // Hoặc trả về một lỗi khác
            }

            // Lấy giỏ hàng từ session
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            CartItem? existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem == null)
            {
                // Nếu chưa có, tạo mới và thêm vào giỏ
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                // Nếu đã có, chỉ cộng thêm số lượng
                existingItem.Quantity += quantity;
            }

            // Lưu giỏ hàng đã được cập nhật trở lại vào session
            HttpContext.Session.Set(CartSessionKey, cart);

            // Chuyển hướng người dùng về trang họ vừa ở (hoặc trang Shop)
            return RedirectToAction("Shop", "Products");
        }

        // Các action Update
        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            List<CartItem> cart = GetCartItems();
            CartItem? cartItem = cart.FirstOrDefault(p => p.ProductId == productId);

            if (cartItem != null)
            {
                // Nếu người dùng nhập số lượng > 0, cập nhật nó
                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                }
                else
                {
                    // Nếu người dùng nhập số lượng <= 0, coi như họ muốn xóa
                    cart.Remove(cartItem);
                }
            }

            SaveCartItems(cart);
            // Sau khi cập nhật, quay lại trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Các action Remove
        [HttpGet] // Dùng GET cho đơn giản, vì đây là link
        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = GetCartItems();

            // Xóa tất cả các item có ProductId trùng khớp (dù chỉ có 1)
            cart.RemoveAll(p => p.ProductId == productId);

            SaveCartItems(cart);
            // Sau khi xóa, quay lại trang giỏ hàng
            return RedirectToAction("Index");
        }
    }
}