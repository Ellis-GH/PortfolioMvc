using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPortfolio.Services;
using MvcPortfolio.Data;
using MvcPortfolio.Models;

namespace MvcPortfolio.Controllers
{
    public class HoldingController : Controller
    {
        private readonly MvcPortfolioContext _context;
        private readonly MarketPriceService _marketPriceService;
        public HoldingController(MvcPortfolioContext context, MarketPriceService marketPriceService)
        {
            _context = context;
            _marketPriceService = marketPriceService;
        }

        // GET: Holding
        public async Task<IActionResult> Index()
        {
            // var mvcPortfolioContext = _context.Transaction.Include(t => t.Ticker);
            /*
            var transactions = await _context.Transaction
                .Include(t => t.Ticker)
                .OrderBy(t => t.Date)
                .ToListAsync();
            */
            
            var holdings = await _context.Transaction //May need to replace with a C# script for more complex stuff
                .Include(t => t.Ticker)
                .GroupBy(t => t.Ticker)
                .Select(g => new Holding
                {
                    Ticker = g.Key,
                    Quantity = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity),
                    RIC = g.Sum(t => t.Buy ? t.Cost : -t.Cost),
                    CurrentValue = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity) * g.Key.Price
                })
                .ToListAsync();
            

            return View(holdings);
            //return View(await mvcPortfolioContext.ToListAsync());
        }
    
        public async Task<IActionResult> UpdatePrice(string id)
        {
            var ticker = await _context.Ticker
                .Include(t => t.Exchange) // navigation property you want
                .FirstOrDefaultAsync(t => t.TickerString == id);
            if (ticker == null)
                return NotFound();

            if (ticker.Exchange == null)
            {
                //debug
            }

            decimal currentPrice = await _marketPriceService.GetCurrentPriceAsync(ticker.TickerString, ticker.Exchange.AVCode);
            if (currentPrice == -1)
            {
                return NotFound(); //Wrong response
            }

            ticker.Price = currentPrice / ticker.Exchange.RatioToOne;
            ticker.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
