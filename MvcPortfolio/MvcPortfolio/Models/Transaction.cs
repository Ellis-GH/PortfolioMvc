using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPortfolio.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Ticker")]
        [StringLength(30)]
        [Display(Name = "Ticker")]
        public string TickerString{ get; set; }
        public virtual Ticker? Ticker { get; set; } //Navigation property

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Cost { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public bool Buy { get; set; }
    }
}
