using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mugnai._aux._debug;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai._aux.utils
{
    public static class Utils
    { 
        public static bool SiteNameAlreadyExists(AuctionSiteContext context, string name)
        {
            return context.Sites.Any(site => site.Name == name);
        }

        public static bool IsEndedAuction(IAuction auction)
        {
            return auction.EndsOn < DateTime.Now;
        }

        public static bool IsSiteDisposed(Site site)
        {
            return site.IsDeleted;
        }

        internal static bool ArePasswordsEquals(string password1, string password2)
        {
            return password1 == password2;
        }

        internal static Session CreateNewSession(Site site, User user)
        {
            using (var context = new AuctionSiteContext(site.ConnectionString))
            {
                var session = new Session
                {
                    Id = CreateSessionId(site, user),
                    ValidUntil = site.AlarmClock.Now.AddSeconds(site.SessionExpirationInSeconds)
                };
                context.Sessions.Add(session); 
                context.SaveChanges();
                return session;
            }
        }

        public static string CreateSessionId(Site site, User user)
        {
            return site.Name.ToString() + user.UserID.ToString();
        }
    }
}
