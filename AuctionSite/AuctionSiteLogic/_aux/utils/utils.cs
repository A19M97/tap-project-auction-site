using Mugnai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai._aux.utils
{
    public static class Utils
    { 
        public static bool SiteNameAlreadyExists(AuctionSiteContext context, string name)
        {
            return context.Sites.Any(site => site.Name == name);
        }

        public static bool IsSiteDisposed(SiteBLL site)
        {
            return site.IsDeleted;
        }

        internal static bool ArePasswordsEquals(string password1, string password2)
        {
            return password1 == password2;
        }

        internal static SessionBLL CreateNewSession(SiteBLL site, UserBLL user)
        {
            using (var context = new AuctionSiteContext(site.ConnectionString))
            {
                var session = new Session
                {
                    Id = CreateSessionId(site, user),
                    ValidUntil = site.AlarmClock.Now.AddSeconds(site.SessionExpirationInSeconds),
                };
                context.Sessions.Add(session);
                context.SaveChanges();
                return new SessionBLL(session, user);
            }
        }

        internal static bool IsSessionDisposed(SessionBLL sessionBLL)
        {
            return sessionBLL.IsDeleted;
        }

        public static string CreateSessionId(SiteBLL site, UserBLL user)
        {
            return site.Name + user.UserID;
        }

        public static bool IsValidUsername(string username)
        {
            var usernameLength = username.Length;
            return usernameLength >= DomainConstraints.MinUserName && usernameLength <= DomainConstraints.MaxUserName;
        }

        public static bool IsValidPassword(string password)
        {
            return password.Length >= DomainConstraints.MinUserPassword;
        }

        internal static IEnumerable<IUser> UsersToUsersBLL(List<User> users, ISite site)
        {
            var usersBLL = new List<UserBLL>();
            foreach (var user in users)
                usersBLL.Add(new UserBLL(user, site));
            return usersBLL;

        }

        internal static IUser UserToUserBLL(User user, ISite site)
        {
            return new UserBLL(user, site);

        }

        internal static IEnumerable<IAuction> AuctionsToAuctionsBLL(List<Auction> auctions, ISite site)
        {
            var auctionsBLL = new List<AuctionBLL>();
            foreach (var auction in auctions)
                auctionsBLL.Add(new AuctionBLL(auction, new UserBLL(auction.Seller, site as SiteBLL)));
            return auctionsBLL;
        }
    }
}
