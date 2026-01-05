using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcPortfolio.Models;

namespace MvcPortfolio.Data
{
    public class MvcPortfolioContext : DbContext
    {
        public MvcPortfolioContext (DbContextOptions<MvcPortfolioContext> options)
            : base(options)
        {
        }

        public DbSet<MvcPortfolio.Models.Ticker> Ticker { get; set; } = default!;

        public DbSet<MvcPortfolio.Models.Exchange> Exchange { get; set; } = default!;
        public DbSet<MvcPortfolio.Models.Transaction> Transaction { get; set; } = default!;

        
    }
}
