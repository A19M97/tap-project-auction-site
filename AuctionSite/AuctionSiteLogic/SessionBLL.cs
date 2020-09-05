using Mugnai.Model;
using System;
using System.Data.Entity;
using System.Linq;
using Mugnai._aux.utils;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SessionBLL : ISession
    {
        public string Id { get; }
        public DateTime ValidUntil { get; set; }
        public IUser User { get; }

        public bool IsDeleted = false;

        public bool LoggedOut;
        private IAlarmClock AlarmClock { get; }

        public SessionBLL(Session session)
        {
            Id = session.Id;
            ValidUntil = session.ValidUntil;
        }

        public SessionBLL(Session session, IUser user)
        {
            Id = session.Id;
            ValidUntil = session.ValidUntil;
            User = user;
            UserBLL userBLL = (UserBLL) User;
            AlarmClock = userBLL.Site.AlarmClock;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is ISession)) return false;
            return ((ISession) obj).Id == this.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public bool IsValid()
        {
            if (IsLoggedOut()) return false;
            
            var userBLL = User as UserBLL;
            DateTime validUntil;
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                var session = context.Sessions.Find(Id);
                if (null == session)
                    return false;
                validUntil = session.ValidUntil;
            }

            return AlarmClock.Now <= validUntil;
        }

        public void Logout()
        {
            if (IsLoggedOut()) 
                throw new InvalidOperationException("User already logged out.");

            var userBLL = User as UserBLL;
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                var session = context.Sessions.Find(Id);
                if (null == session)
                    throw new InvalidOperationException("invalid operation: session not found.");
                context.Users.Where(u => u.UserID == userBLL.UserID).Load();
                context.Sessions.Remove(session);
                context.SaveChanges();
            }

            LoggedOut = true;
        }

        public IAuction CreateAuction(string description, DateTime endsOn, double startingPrice)
        {
            if(!IsValid() || Utils.IsSessionDisposed(this))
                throw new InvalidOperationException("Session not valid or disposed.");
            if(null == description)
                throw new ArgumentNullException($"{nameof(description)} cannot be null.");
            if(string.Empty == description)
                throw new ArgumentException($"{nameof(description)} cannot be an empty string.");
            if(IsStartingPriceNegative(startingPrice))
                throw new ArgumentOutOfRangeException($"{nameof(startingPrice)} cannot be a negative number.");
            if(endsOn < AlarmClock.Now)
                throw new UnavailableTimeMachineException($"{endsOn} cannot be in the past.");
            var userBLL = User as UserBLL;
            Auction auction;
            var validUntil = AlarmClock.Now.AddSeconds(userBLL.Site.SessionExpirationInSeconds);
            using (var context = new AuctionSiteContext(userBLL.Site.ConnectionString))
            {
                auction = new Auction()
                {
                    Description = description,
                    EndsOn = endsOn,
                    SiteName = userBLL.Site.Name,
                    SellerId = userBLL.UserID,
                    CurrentWinnerId = null,
                    LastBid = null,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice
                };
                context.Auctions.Add(auction);

                var session = context.Sessions.Find(Id);
                if(null == session)
                    throw new InvalidOperationException("Invalid operation: session not found.");
                session.ValidUntil = validUntil;
                context.Entry(session).State = EntityState.Modified;
                context.SaveChanges();
            }
            ValidUntil = validUntil;
            return new AuctionBLL(auction, User);
        }

        /*AUX METHODS*/
        private bool IsStartingPriceNegative(double startingPrice)
        {
            return startingPrice < 0;
        }

        private bool IsLoggedOut()
        {
            return LoggedOut;
        }

        /*END AUX METHODS*/
    }
}