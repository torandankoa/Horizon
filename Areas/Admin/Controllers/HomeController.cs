using Microsoft.AspNetCore.Mvc;

namespace Horizon.Areas.Admin.Controllers
{
    // Kế thừa từ AdminBaseController để được bảo vệ bởi [Authorize(Roles = "Admin")]
    public class HomeController : AdminBaseController
    {
        // GET: /Admin/Home/Index hoặc /Admin
        public IActionResult Index()
        {
            // Sau này chúng ta sẽ truyền dữ liệu thống kê vào đây
            return View();
        }
    }
}