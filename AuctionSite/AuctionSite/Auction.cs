using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class Auction : IAuction
    {
        public IUser CurrentWinner()
        {
            throw new NotImplementedException();
        }

        public double CurrentPrice()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool BidOnAuction(ISession session, double offer)
        {
            throw new NotImplementedException();
        }

        public int Id { get; set; }
        public IUser Seller { get; set; }
        public string Description { get; set; }
        public DateTime EndsOn { get; set; }
        public string SiteName { get; set; }
        [ForeignKey("SiteName")]
        public ISite Site { get; set; }
    }
}