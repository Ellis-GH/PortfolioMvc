using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPortfolio.Models
{
    public class Exchange
    {
        [Key] //Must match the reqs of the exchanges table
        [StringLength(30)]
        public string ExchangeString { get; set; }

        [Required]
        public string AVCode { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal RatioToOne { get; set; }
    }
}
