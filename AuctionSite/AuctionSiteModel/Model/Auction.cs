using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai.Model
{
    public class Auction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime EndsOn { get; set; }

        [MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName), Required]
        public string SiteName { get; set; }

        [ForeignKey("SiteName"), ]
        public Site Site { get; set; }

        public int SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual User Seller { get; set; }

        public int? CurrentWinnerId { get; set; }

        [ForeignKey("CurrentWinnerId")]
        public virtual User CurrentWinner { get; set; }

        public double? LastBid { get; set; }
        public double StartingPrice { get; set; }
    }
}