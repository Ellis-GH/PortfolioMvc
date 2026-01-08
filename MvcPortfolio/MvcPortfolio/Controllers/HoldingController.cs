using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPortfolio.Services;
using MvcPortfolio.Data;
using MvcPortfolio.Models;
using Microsoft.VisualBasic;

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
            /*
            var holdings = await _context.Transaction //May need to replace with a C# script for more complex stuff
                .Include(t => t.Ticker)
                .GroupBy(t => t.Ticker)
                .Select(g => new Holding
                {
                    Ticker = g.Key,
                    Quantity = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity),
                    RIC = g.Sum(t => t.Buy ? t.Cost : -t.Cost),
                    CurrentValue = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity) * g.Key.Price,
                    XIRR = g.Sum(t => t.Price / ((1 + rate) * ((dayofinvestment - firstdayofinvestment) / 365)))
                })
                .ToListAsync();
            */
            var transactionGroups = await _context.Transaction
                .Include(t => t.Ticker)
                .GroupBy(t => t.Ticker)
                .Select(g => new
                {
                    Ticker = g.Key,
                    Transactions = g.Select(t => new
                    {
                        Date = t.Date,
                        Amount = t.Buy ? -t.Cost : t.Cost, // cash-flow sign
                        Quantity = t.Quantity
                    }).ToList(),

                    Quantity = g.Sum(t => t.Buy ? t.Quantity : -t.Quantity),
                    RIC = g.Sum(t => t.Buy ? t.Cost : -t.Cost),
                    ACBCostSum = g.Sum(t => t.Buy ? t.Cost : 0),
                    ACBQuantSum = g.Sum(t => t.Buy ? t.Quantity : 0)
                })
                .ToListAsync();

            var holdings = transactionGroups.Select(g =>
            {
                // Convert transactions to tuples for XIRR
                var cashFlows = g.Transactions
                    .Select(t => (t.Date, t.Amount, t.Quantity))
                    .ToList();

                //decimal TempTWR = CalculateTWR(cashFlows);

                // Add terminal cash flow: today's market value
                cashFlows.Add((DateTime.Today, g.Quantity * g.Ticker.Price, g.Quantity));

                return new Holding
                {
                    Ticker = g.Ticker,
                    Quantity = g.Quantity,
                    CurrentValue = g.Quantity * g.Ticker.Price,
                    ACB = (g.ACBCostSum / g.ACBQuantSum) * g.Quantity, // returns double
                    TotalGain = ((g.Quantity * g.Ticker.Price) / ((g.ACBCostSum / g.ACBQuantSum) * g.Quantity)) - 1, //(1 - (ACB / CV)) * 100
                    RIC = g.RIC,
                    XIRR = CalculateXIRR(cashFlows), // returns double
                    TWR = CalculateTWR(cashFlows)
                };
            }).ToList();

            return View(holdings);
            //return View(await mvcPortfolioContext.ToListAsync());
        }

        public static double CalculateXIRR(List<(DateTime Date, decimal Amount, decimal Quantity)> cashFlows, double guess = 0.1)
        {
            double rate = guess;

            for (int i = 0; i < 100; i++)
            {
                double f = 0;
                double df = 0;
                var t0 = cashFlows.Min(c => c.Date);

                foreach (var cashFlow in cashFlows)
                {
                    var years = (cashFlow.Date - t0).TotalDays / 365.0;
                    f += (double)cashFlow.Amount / Math.Pow(1 + rate, years);
                    df += -years * (double)cashFlow.Amount / Math.Pow(1 + rate, years + 1);
                }

                rate -= f / df;
            }

            return rate;
        }

        public static decimal CalculateTWR(List<(DateTime Date, decimal Amount, decimal Quantity)> cashFlows)
        {
            decimal startValue = cashFlows[0].Amount;
            decimal endValue;
            decimal holdingQuantity = cashFlows[0].Quantity;

            decimal TWR = 0;

            for (int i = 1; i < cashFlows.Count ; i++)
            {
                decimal priceAtThatMoment = Math.Abs(cashFlows[i].Amount) / cashFlows[i].Quantity;
                endValue = holdingQuantity * priceAtThatMoment;
                TWR += (endValue - startValue) / startValue;

                holdingQuantity += cashFlows[i].Amount > 0 ? cashFlows[i].Quantity : -cashFlows[i].Quantity;
                startValue = priceAtThatMoment * holdingQuantity;
            }

            return (1 + TWR);
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
