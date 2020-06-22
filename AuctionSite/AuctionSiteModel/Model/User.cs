using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai.Model
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MinLength(DomainConstraints.MinUserName), MaxLength(DomainConstraints.MaxUserName)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Salt { get; set; }

        [MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName)]
        public string SiteName { get; set; }

        [ForeignKey("SiteName")]
        public virtual Site Site { get; set; }

        public virtual string SessionId { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        public virtual ICollection<Auction> CreatedAuctions { get; set; }

    }
}