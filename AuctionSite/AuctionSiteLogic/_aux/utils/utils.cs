using Mugnai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai._aux.utils
{
    public static class Utils
    {

        private const int SALT_SIZE = 16; // size in bytes
        private const int HASH_SIZE = 24; // size in bytes
        private const int ITERATIONS = 1000; // number of pbkdf2 iterations

        public static bool SiteNameAlreadyExists(AuctionSiteContext context, string name)
        {
            return context.Sites.Any(site => site.Name == name);
        }

        public static bool IsSiteDisposed(SiteBLL site)
        {
            return site.IsDeleted;
        }

        internal static bool ArePasswordsEquals(string password1, string password2, byte[] salt)
        {
            return password1 == Convert.ToBase64String(HashPassword(password2, salt));
        }

        internal static byte[] HashPassword(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                hashGenerator.IterationCount = ITERATIONS;
                return hashGenerator.GetBytes(HASH_SIZE);
            }
        }

        internal static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SALT_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
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

        public static bool IsSessionDisposed(SessionBLL sessionBLL)
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

        public static bool IsAuctionDisposed(AuctionBLL auctionBll)
        {
            return auctionBll.IsDeleted;
        }
        public static bool IsUserDisposed(UserBLL user)
        {
            return user.IsDeleted;
        }

    }
}
