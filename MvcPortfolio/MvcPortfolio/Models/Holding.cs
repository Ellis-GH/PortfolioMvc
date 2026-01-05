using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPortfolio.Models
{
    public class Holding
    {
        [ForeignKey("Ticker")]
        [StringLength(30)]
        [Display(Name = "Ticker")]
        public string TickerString { get; set; }
        public virtual Ticker? Ticker { get; set; } //Navigation property

        [NotMapped]
        public decimal Quantity { get; set; }

        [Display(Name = "Remaining Invested Capital")]
        [NotMapped]
        public decimal RIC { get; set; }

        [NotMapped]
        public decimal CurrentValue { get; set; }

        [NotMapped]
        public decimal TotalGain { get; set; }

        [NotMapped]
        public decimal TWR { get; set; }

        [NotMapped]
        public decimal XIRR { get; set; }
    }
}
