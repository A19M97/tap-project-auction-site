using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Mugnai._aux.utils;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;
using ISite = TAP2018_19.AuctionSite.Interfaces.ISite;

namespace Mugnai
{
    public class Site : ISite
    {
        
        public IEnumerable<IUser> GetUsers()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var users = (
                    from user in context.Users.Include("Session")
                    where user.SiteName == Name
                    select user
                ).ToList();
                return users;
            }
        }

        public IEnumerable<ISession> GetSessions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            var sessions = new List<Session>();
            foreach (var iUser in GetUsers())
            {
                var user = iUser as User;
                if(user?.Session != null)
                {
                    user.Session.User = user;
                    sessions.Add(user.Session);
                }
            }
            return sessions;
        }

        public IEnumerable<IAuction> GetAuctions(bool onlyNotEnded)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                List<Auction> auctions;
                if (!onlyNotEnded) { 
                    auctions = (
                        from auction in context.Auctions
                        where auction.SiteName == Name
                        select auction
                    ).ToList();
                }else {
                    auctions = (
                        from auction in context.Auctions
                        where auction.SiteName == Name &&auction.EndsOn > this.AlarmClock.Now
                        select auction
                    ).ToList();
                }
                return auctions;
            }
        }

        public ISession Login(string username, string password)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == username || null == password)
                throw new ArgumentNullException();
            if (!Utils.IsValidUsername(username) || !Utils.IsValidPassword(password))
                throw new ArgumentException();

            User user = GetUserByUsername(username);
            if (null == user)
                return null;

            if (!Utils.ArePasswordsEquals(user.Password, password))
                return null;

            var userSession = GetUserSession(user);
            user.SessionId = userSession.Id;
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }

            return userSession;

        }

        public ISession GetSession(string sessionId)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == sessionId)
                throw new ArgumentNullException();
            if ("" == sessionId)
                return null;
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var session = context.Sessions.Find(sessionId);
                if(null != session && session.IsValid())
                    return session;
                return null;
            }
        }

        public void CreateUser(string username, string password)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == username || null == password)
                throw new ArgumentNullException();
            if (!Utils.IsValidUsername(username) || !Utils.IsValidPassword(password))
                throw new ArgumentException();
            if (IsUsernameAlreadyUsedInSite(username))
                throw new NameAlreadyInUseException("Username already used: " + username);
            AddUser(username, password);
        }

        public void Delete()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            DeleteUsers();
            DeleteAuctions();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                context.Entry(this).State = EntityState.Deleted;
                context.SaveChanges();
                IsDeleted = true;
            }
        }

        public void CleanupSessions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var sessions = (
                        from session in context.Sessions
                        select session
                    );
                foreach (var session in sessions)
                    if (!session.IsValid())
                        context.Entry(session).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        /*AUX METHODS*/

        private void AddUser(string username, string password)
        {
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var user = new User()
                {
                    Username = username,
                    Password = password,
                    SiteName = Name
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        private bool IsUsernameAlreadyUsedInSite(string username)
        {
            foreach (var user in GetUsers())
                if (user.Username == username)
                    return true;
            return false;
        }

        private void DeleteAuctions()
        {
            foreach (var auction in GetAuctions(false))
                auction.Delete();
        }

        private void DeleteUsers()
        {
            foreach (var user in GetUsers())
                user.Delete();
        }

        private User GetUserByUsername(string username)
        {
            foreach (var user in GetUsers())
            {
                if (user.Username == username)
                    return user as User;
            }
            return null;
        }

        private Session GetUserSession(User user)
        {

            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var session = context.Sessions.Find(Utils.CreateSessionId(this, user));
                if (null == session || !session.IsValid())
                    user.Session = Utils.CreateNewSession(this, user);
                //session.User = user;
                return user.Session;
            }
        }

        public void CleanupSessionOnRingingEvent()
        {
            this.CleanupSessions();
        }

        /*END AUX METHODS*/

        [Key, MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName)]
        public string Name { get; set; }

        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Range(1, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }

        public IAlarm Alarm;

        internal string ConnectionString;
        
        public bool IsDeleted;

        public IAlarmClock AlarmClock;
    }
}