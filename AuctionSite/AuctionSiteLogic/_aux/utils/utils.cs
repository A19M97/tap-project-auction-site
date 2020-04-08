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

        public static bool IsSiteDisposed(Site site)
        {
            return true;
            /*return site.IsDeleted;*/
        }

        internal static bool ArePasswordsEquals(string password1, string password2)
        {
            return password1 == password2;
        }

        internal static Session CreateNewSession(Site site, User user)
        {
            //using (var context = new AuctionSiteContext(site.ConnectionString))
            //{
            //    var session = new Session
            //    {
            //        Id = CreateSessionId(site, user),
            //        ValidUntil = site.AlarmClock.Now.AddSeconds(site.SessionExpirationInSeconds),
            //        User = user
            //    };
            //    context.Sessions.Add(session); 
            //    context.SaveChanges();
            //    return session;
            //}
            return new Session();
        }

        public static string CreateSessionId(Site site, User user)
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
    }
}
