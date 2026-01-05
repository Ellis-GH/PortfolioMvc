using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcPortfolio.Data;
using MvcPortfolio.Models;

namespace MvcPortfolio.Controllers
{
    public class TickersController : Controller
    {
        private readonly MvcPortfolioContext _context;

        public TickersController(MvcPortfolioContext context)
        {
            _context = context;
        }

        // GET: Tickers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ticker.ToListAsync());
        }

        // GET: Tickers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticker = await _context.Ticker
                .FirstOrDefaultAsync(m => m.TickerString == id);
            if (ticker == null)
            {
                return NotFound();
            }

            return View(ticker);
        }

        // GET: Tickers/Create
        public IActionResult Create()
        {
            var exchanges = _context.Exchange
                .OrderBy(e=> e.ExchangeString)
                .ToList();
            ViewBag.ExchangeList = new SelectList(exchanges, "ExchangeString", "ExchangeString");

            return View();
        }

        // POST: Tickers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TickerString,ExchangeString,Name,Type,Price,LastUpdated")] Ticker ticker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticker);
        }

        // GET: Tickers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticker = await _context.Ticker.FindAsync(id);
            if (ticker == null)
            {
                return NotFound();
            }
            return View(ticker);
        }

        // POST: Tickers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TickerString,ExchangeString,Name,Type,Price,LastUpdated")] Ticker ticker)
        {
            if (id != ticker.TickerString)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TickerExists(ticker.TickerString))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticker);
        }

        // GET: Tickers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticker = await _context.Ticker
                .FirstOrDefaultAsync(m => m.TickerString == id);
            if (ticker == null)
            {
                return NotFound();
            }

            return View(ticker);
        }

        // POST: Tickers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ticker = await _context.Ticker.FindAsync(id);
            if (ticker != null)
            {
                _context.Ticker.Remove(ticker);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TickerExists(string id)
        {
            return _context.Ticker.Any(e => e.TickerString == id);
        }
    }
}
