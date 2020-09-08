using Mugnai.Model;
using System;
using System.Data.Entity;
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

        public bool IsDeleted;


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
                throw new InvalidOperationException("Invalid operation: user is disposed.");
            var userBLL = (UserBLL) Seller;
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if (null == auction)
                    throw new InvalidOperationException("Invalid operation: auction not found.");
                var currentWinner = auction.CurrentWinner;
                if (null == currentWinner)
                    return null;
                return Utils.UserToUserBLL(currentWinner, userBLL.Site);
            }
        }

        public double CurrentPrice()
        {
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException("Invalid operation: user is disposed.");
            var sellerBLL = (UserBLL) Seller;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if(null == auction)
                    throw new InvalidOperationException("Invalid operation: auction not found.");
                return auction.CurrentPrice;
            }
        }

        public void Delete()
        {
            if (Utils.IsAuctionDisposed(this))
                throw new InvalidOperationException("Invalid operation: user is disposed.");
            var sellerBLL = (UserBLL) Seller;
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
                throw new InvalidOperationException("Invalid operation: user is disposed.");
            if (null == session) 
                throw new ArgumentNullException($"{nameof(session)} cannot be null.");
            if(offer < 0) 
                throw new ArgumentOutOfRangeException($"{nameof(offer)} cannot be a negative number.");
            if(!session.IsValid()) 
                throw new ArgumentException($"{nameof(session)} not valid.");
            if (Seller.Equals(session.User))
                throw new ArgumentException("Bidder and seller cannot be the same user.");
            if (!((UserBLL)Seller).Site.Equals(((UserBLL)session.User).Site))
                throw new ArgumentException($"Seller site must be the same of bidder site.");

            var sellerBLL = (UserBLL) Seller;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if(null == auction) 
                    throw new InvalidOperationException("Invalid operation: auction not found.");
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
                    throw new InvalidOperationException("Invalid operation: session not found");
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
                auction.CurrentWinnerId = ((UserBLL) session.User).UserID;
            if (null != currentPrice)
                auction.CurrentPrice = (double) currentPrice;
            context.Entry(auction).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void UpdateDeletedUser()
        {
            var sellerBLL = (UserBLL) Seller;
            using (var context = new AuctionSiteContext(sellerBLL.Site.ConnectionString))
            {
                var auction = context.Auctions.Find(Id);
                if (null == auction)
                    throw new InvalidOperationException("Invalid operation: auction not found.");
                auction.CurrentWinner = null;
                auction.CurrentWinnerId = null;
                context.Entry(auction).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
