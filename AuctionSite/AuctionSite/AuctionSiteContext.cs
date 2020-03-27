using System;
using System.Data.Entity;
using System.Security;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class AuctionSiteContext : DbContext
    {
        
        public AuctionSiteContext(string connectionString)
        { 
            if (connectionString == null)
                throw new ArgumentNullException();
            
        }

        public DbSet<ISite> Sites { get; set; }
        public DbSet<IUser> Users { get; set; }
        public DbSet<IAuction> Auctions { get; set; }
        public DbSet<ISession> Sessions { get; set; }

    }
}