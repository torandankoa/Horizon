using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // <<< Ổ KHÓA QUAN TRỌNG NHẤT
    public class AdminBaseController : Controller
    {
        // Controller này không cần có action nào cả
    }
}