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
            throw new NotImplementedException();
        }

        public static string CreateSessionId(SiteBLL site, UserBLL user)
        {
            throw new NotImplementedException();
            /*return site.Name + user.UserID;*/
        }

        public static bool IsValidUsername(string username)
        {
            var usernameLength = username.Length;
            return usernameLength >= DomainConstraints.MinUserName && usernameLength <= DomainConstraints.MaxUserName;
        }

        internal static IEnumerable<IUser> UsersToUsersBLL(List<User> users)
        {
            var usersBLL = new List<UserBLL>();
            foreach (var user in users)
                usersBLL.Add(new UserBLL(user));
            return usersBLL;

        }

        public static bool IsValidPassword(string password)
        {
            return password.Length >= DomainConstraints.MinUserPassword;
        }
    }
}
