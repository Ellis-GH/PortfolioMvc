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

        [Display(Name = "Average Cost Basis")]
        [NotMapped]
        public decimal ACB { get; set; }

        [Display(Name = "Current Value")]
        [NotMapped]
        public decimal CurrentValue { get; set; }

        [Display(Name = "Gain on ACB")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [NotMapped]
        public decimal TotalGain { get; set; }

        [Display(Name = "Remaining Invested Capital")]
        [NotMapped]
        public decimal RIC { get; set; }

        [Display(Name = "Time Weighted Return")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [NotMapped]
        public decimal TWR { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [NotMapped]
        public double XIRR { get; set; }
    }
}
