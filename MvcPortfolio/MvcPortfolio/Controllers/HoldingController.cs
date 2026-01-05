using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPortfolio.Data;
using MvcPortfolio.Models;

namespace MvcPortfolio.Controllers
{
    public class HoldingController : Controller
    {
        private readonly MvcPortfolioContext _context;

        public HoldingController(MvcPortfolioContext context)
        {
            _context = context;
        }

        // GET: Holding
        public async Task<IActionResult> Index()
        {
            // var mvcPortfolioContext = _context.Transaction.Include(t => t.Ticker);
            var holdings = await _context.Transaction
                .Include(t => t.Ticker)
                .GroupBy(t => t.Ticker)
                .Select(g => new Holding
                {
                    Ticker = g.Key,
                    Quantity = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity),
                    RIC = g.Sum(t => t.Buy ? t.Cost : -t.Cost)
                })
                .ToListAsync();

            return View(holdings);
            //return View(await mvcPortfolioContext.ToListAsync());
        }
    }
}
