using System.Data.Entity;
using Mugnai.Model;

namespace Mugnai
{
    public class AuctionSiteContext : DbContext
    {

        public AuctionSiteContext(string connectionString) 
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>().
                HasRequired(c => c.Seller)
                .WithMany(u => u.CreatedAuctions)
                .WillCascadeOnDelete(false);
        }
        public DbSet<Site> Sites { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Session> Sessions { get; set; }

    }
}