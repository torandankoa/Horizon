using Horizon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Horizon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransactionsController : AdminBaseController
    {
        private readonly MyDbContext _context;
        public TransactionsController(MyDbContext context) { _context = context; }

        // GET: /Admin/Transactions/Index
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                                         .Include(t => t.Order)
                                         .OrderByDescending(t => t.CreatedAt)
                                         .ToListAsync();
            return View(transactions);
        }
    }
}
