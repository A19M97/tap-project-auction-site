using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class Site
    {
        [Key, MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName)]
        public string Name { get; set; }

        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Range(1, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }
    }
}
