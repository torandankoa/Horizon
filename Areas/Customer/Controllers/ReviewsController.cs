using Horizon.Data;
using Horizon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Horizon.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Yêu cầu đăng nhập để viết đánh giá
    public class ReviewsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /Customer/Reviews/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // TODO (Nâng cao): Kiểm tra xem user đã thực sự mua sản phẩm này chưa
            // var hasPurchased = _context.OrderDetails.Any(od => od.Order.UserId == userId && od.ProductId == productId);
            // if (!hasPurchased) { return Forbid(); }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Chuyển hướng người dùng trở lại trang chi tiết sản phẩm
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}