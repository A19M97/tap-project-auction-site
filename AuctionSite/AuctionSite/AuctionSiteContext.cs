using System;
using System.Data.Entity;
using System.Security;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class AuctionSiteContext : DbContext
    {

        public AuctionSiteContext(string connectionString) 
            : base(connectionString)
        {
        }

        public DbSet<Site> Sites { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Session> Sessions { get; set; }

    }
}