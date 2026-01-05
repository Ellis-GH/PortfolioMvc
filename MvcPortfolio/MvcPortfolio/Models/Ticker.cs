using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPortfolio.Models
{
    public class Ticker
    {
        [Key]
        [StringLength(30)]
        [Display(Name = "Ticker")]
        public string TickerString { get; set; }

        [ForeignKey("Exchange")]
        [StringLength(30)]
        [Display(Name = "Exchange")]
        public string ExchangeString {  get; set; }
        public virtual Exchange? Exchange { get; set; } //Navigation property

        public string Name { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        [StringLength(30)]
        public string Type { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.Date)]
        public DateTime LastUpdated { get; set; }
    }
}
