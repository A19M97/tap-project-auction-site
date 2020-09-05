using System;
using Mugnai.Model;
using System.Collections.Generic;
using System.Data.Entity;
using Mugnai._aux.utils;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class UserBLL : IUser
    {
        public int UserID { get; }
        public string Username { get; }
        public string Password { get; }
        public string Salt { get; }
        public SiteBLL Site { get; }
        public SessionBLL Session { get; set; }

        public bool IsDeleted;

        public UserBLL(User user, ISite site)
        {
            UserID = user.UserID;
            Username = user.Username;
            Password = user.Password;
            Site = site as SiteBLL;
            Salt = user.Salt;
            if (null != user.Session)
                Session = new SessionBLL(user.Session, this);
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is UserBLL)) return false;
            return ((UserBLL)obj).UserID == UserID;
        }

        public override int GetHashCode() => UserID.GetHashCode();

        public IEnumerable<IAuction> WonAuctions()
        {
            if (Utils.IsUserDisposed(this))
                throw new InvalidOperationException("User disposed.");
            var wonAuctions = new List<IAuction>();
            foreach (var auction in Site.GetEndedAuctions())
            {
                if(Equals(auction.CurrentWinner()))
                    wonAuctions.Add(auction);
            }
            return wonAuctions;
        }

        public void Delete()
        {
            if (Utils.IsUserDisposed(this))
                throw new InvalidOperationException("User disposed.");
            foreach (var auction in Site.GetAuctions(true))
            {
                if (Equals(auction.CurrentWinner()) || Equals(auction.Seller))
                    throw new InvalidOperationException("User cannot be deleted because is a current winner in an auction.");
            }
            foreach (var auction in Site.GetEndedAuctions())
            {
                var auctionBLL = (AuctionBLL) auction;
                if (Equals(auctionBLL.CurrentWinner()))
                    auctionBLL.UpdateDeletedUser();
                else if (Equals(auctionBLL.Seller))
                    auctionBLL.Delete();
            }
            Session?.Logout();
            using (var context = new AuctionSiteContext(Site.ConnectionString))
            {
                var user = context.Users.Find(UserID);
                context.Entry(user).State = EntityState.Deleted;
                context.SaveChanges();
            }

            IsDeleted = true;
        }
    }
}