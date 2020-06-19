using Mugnai.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mugnai._aux.utils;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    class AuctionBLL : IAuction
    {
        public int Id { get; }
        public IUser Seller { get; }
        public string Description { get; }
        public DateTime EndsOn { get; }
        

        public AuctionBLL(Auction auction, IUser seller)
        {
            Id = auction.Id;
            Seller = seller;
            Description = auction.Description;
            EndsOn = auction.EndsOn;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is IAuction)) return false;
            return ((IAuction)obj).Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public IUser CurrentWinner()
        {
            var userBLL = Seller as UserBLL;
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if (null == auction)
                    throw new InvalidOperationException();
                var currentWinner = auction.CurrentWinner;
                if (null == currentWinner)
                    return null;
                return Utils.UserToUserBLL(context.Users.Find(currentWinner), userBLL.Site);
            }
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
            if(null == session) 
                throw new ArgumentNullException();
            if(offer < 0) 
                throw new ArgumentOutOfRangeException();
            if(!session.IsValid() || Seller.Equals(session.User) || !(Seller as UserBLL).Site.Equals((session.User as UserBLL).Site)) 
                throw new ArgumentException();

            var sellerBLL = Seller as UserBLL;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if(null == auction) 
                    throw new InvalidOperationException();
                var lastBid = auction.LastBid;
                var currentWinnerId = auction.CurrentWinnerId;
                var minimumBidIncrement = sellerBLL.Site.MinimumBidIncrement;
                var startingPrice = auction.StartingPrice;
                if (offer < startingPrice)
                    return false;
                if (null == lastBid && null == currentWinnerId) //--> is the first bid.
                {
                    auction.LastBid = offer;
                    auction.CurrentWinnerId = (session.User as UserBLL).UserID;
                    context.Entry(auction).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }

                if (lastBid + minimumBidIncrement >= offer) return false;

                return true;
            }
        }
    }
}
