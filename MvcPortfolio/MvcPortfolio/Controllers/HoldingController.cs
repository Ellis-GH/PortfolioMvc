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
                if (g.Quantity != 0)
                {
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
                }
                else
                {
                    return new Holding
                    {

                    };
                }
            }).ToList();

            ViewBag.APICalls = await _marketPriceService.GetCallCount();

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

        public static decimal CalculateTWR(List<(DateTime Date, decimal Amount, decimal Quantity)> transactions)
        {
            /*
            decimal startValue = transactions[0].Amount; //Initial value of the holding = the cost paid for it
            decimal endValue;
            decimal holdingQuantity = transactions[0].Quantity; //Initial quantity of the holding = the quantity bought

            decimal TWR = 1;
            
            for (int i = 1; i < transactions.Count ; i++) //For each transaction
            {
                Console.WriteLine("Starting value: " + startValue + " holdingQuantity: " + holdingQuantity + " amount: " + transactions[i].Amount);

                decimal priceAtThatMoment = Math.Abs(transactions[i].Amount) / transactions[i].Quantity; //The price at the moment/moment b4
                endValue = holdingQuantity * priceAtThatMoment; //Value of holding at period ending at moment of this transaction

                if (startValue != 0)
                {
                    TWR *= (endValue - startValue) / startValue; //Time Weighted Return multiplied in
                }
                else
                {
                    Console.WriteLine("Transaction: " + i + " has a start value of 0! " + transactions[i].Amount + " Holding quantity is: " + holdingQuantity);
                }

                holdingQuantity += transactions[i].Amount > 0 ? transactions[i].Quantity : -transactions[i].Quantity; //New quantity determined
                startValue = priceAtThatMoment * holdingQuantity; //New value calced for start of new period
            }

            return (1 + TWR);
            */
            decimal twrFactor = 1m;

            decimal holdingQty = transactions[0].Quantity;
            decimal lastPrice = Math.Abs(transactions[0].Amount) / transactions[0].Quantity;
            decimal startValue = holdingQty * lastPrice;

            for (int i = 1; i < transactions.Count; i++)
            {
                
                var tx = transactions[i];

                if (tx.Quantity != 0)
                {
                    decimal price = Math.Abs(tx.Amount) / tx.Quantity;
                    decimal endValue = holdingQty * price;

                    // Sub-period return (BEFORE cash flow)
                    if (startValue != 0)
                    {
                        decimal periodReturn = (endValue - startValue) / startValue;
                        twrFactor *= (1 + periodReturn);
                    }

                    // Apply cash flow (after return calculation)
                    holdingQty += tx.Amount < 0 ? tx.Quantity : -tx.Quantity;

                    startValue = holdingQty * price;
                    lastPrice = price;
                }
            }

            return twrFactor - 1;
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

            ticker.Price = currentPrice / ticker.RatioToOne;
            ticker.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
