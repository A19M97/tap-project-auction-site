﻿using System;
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

        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}