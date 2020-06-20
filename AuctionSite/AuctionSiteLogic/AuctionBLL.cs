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
    public class AuctionBLL : IAuction
    {
        public int Id { get; }
        public IUser Seller { get; }
        public string Description { get; }
        public DateTime EndsOn { get; }
        public bool IsDeleted = false;


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
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException();
            var userBLL = Seller as UserBLL;
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if (null == auction)
                    throw new InvalidOperationException();
                var currentWinner = auction.CurrentWinner;
                if (null == currentWinner)
                    return null;
                return Utils.UserToUserBLL(currentWinner, userBLL.Site);
            }
        }

        public double CurrentPrice()
        {
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException();
            var sellerBLL = Seller as UserBLL;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if(null == auction)
                    throw new InvalidOperationException();
                return auction.CurrentPrice;
            }
        }

        public void Delete()
        {
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException();
            var sellerBLL = Seller as UserBLL;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                context.Entry(auction).State = EntityState.Deleted;
                context.SaveChanges();
            }
            IsDeleted = true;
        }

        public bool BidOnAuction(ISession session, double offer)
        {
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException();
            if (null == session) 
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
                var isFirstBid = null == lastBid && null == currentWinnerId;
                var bidderAlreadyWinning = currentWinnerId == ((UserBLL)session.User).UserID;
                if (bidderAlreadyWinning)
                    if (lastBid + minimumBidIncrement >= offer)
                        return false;
                if (offer < CurrentPrice())
                    return false;
                if (!isFirstBid && offer < CurrentPrice() + minimumBidIncrement)
                    return false;
                var sessionDb = context.Sessions.Find(session.Id);
                if(null == sessionDb)
                    throw new InvalidOperationException();
                sessionDb.ValidUntil = sellerBLL.Site.AlarmClock.Now.AddSeconds(sellerBLL.Site.SessionExpirationInSeconds);
                context.Entry(sessionDb).State = EntityState.Modified;
                ((SessionBLL) session).ValidUntil = sellerBLL.Site.AlarmClock.Now.AddSeconds(sellerBLL.Site.SessionExpirationInSeconds);
                if (isFirstBid)
                {
                    UpdateAuction(context, auction, session, offer, null);
                    return true;
                }
                if (bidderAlreadyWinning)
                {
                    UpdateAuction(context, auction, null, offer, null);
                    return true;
                }
                if(offer > lastBid)
                {
                    UpdateAuction(context, auction, session, offer, Math.Min(offer, (double)lastBid + minimumBidIncrement));
                    return true;
                }
                UpdateAuction(context, auction, null, offer, Math.Min(offer + minimumBidIncrement, (double) lastBid));
                return true;
            }
        }

        private static void UpdateAuction(AuctionSiteContext context, Auction auction, ISession session, double offer, double? currentPrice)
        {
            auction.LastBid = offer;
            if(null != session)
                auction.CurrentWinnerId = (session.User as UserBLL).UserID;
            if (null != currentPrice)
                auction.CurrentPrice = (double) currentPrice;
            context.Entry(auction).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
