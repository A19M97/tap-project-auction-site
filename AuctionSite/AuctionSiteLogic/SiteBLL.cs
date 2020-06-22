using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mugnai._aux.utils;
using Mugnai.Model;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SiteBLL : ISite
    {
        public string Name { get; }
        public int Timezone { get; }
        public int SessionExpirationInSeconds { get; }
        public double MinimumBidIncrement { get; }
        public IAlarmClock AlarmClock { get; }
        public IAlarm Alarm { get; }
        public string ConnectionString { get; }

        public bool IsDeleted;

        public SiteBLL(Site site, IAlarmClock alarmClock, string connectionString)
        {
            Name = site.Name;
            Timezone = site.Timezone;
            SessionExpirationInSeconds = site.SessionExpirationInSeconds;
            MinimumBidIncrement = site.MinimumBidIncrement;
            AlarmClock = alarmClock;
            Alarm = AlarmClock.InstantiateAlarm(5 * 60 * 1000); /* 5*60*1000 = 300000 = 5 minutes */
            Alarm.RingingEvent += CleanupSessions;
            this.ConnectionString = connectionString;
        }


        public IEnumerable<IUser> GetUsers()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var users =
                        (from user in context.Users.Include("Session")
                        where user.SiteName == Name
                        select user).ToList();
                
                return Utils.UsersToUsersBLL(users, this);
            }
        }

        public IEnumerable<ISession> GetSessions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            var sessions = new List<SessionBLL>();
            foreach (var iUser in GetUsers())
            {
                var user = iUser as UserBLL;
                if (user?.Session != null)
                {
                    //user.Session.User = user;
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
                if (!onlyNotEnded)
                {
                    auctions = (
                        from auction in context.Auctions
                        where auction.SiteName == Name
                        select auction
                    ).ToList();
                }
                else
                {
                    auctions = (
                        from auction in context.Auctions
                        where auction.SiteName == Name && auction.EndsOn > AlarmClock.Now
                        select auction
                    ).ToList();
                }
                return Utils.AuctionsToAuctionsBLL(auctions, this);
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

            var user = GetUserByUsername(username);
            if (null == user)
                return null;

            if (!Utils.ArePasswordsEquals(user.Password, password, Convert.FromBase64String(user.Salt)))
                return null;

            var userSession = GetUserSession(user);
            //user.Session = userSession;
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var dbUser = context.Users.Find(user.UserID);
                if(null == dbUser)
                    throw new InvalidOperationException();
                dbUser.SessionId = userSession.Id;
                context.Entry(dbUser).State = EntityState.Modified;
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

            foreach (var user in GetUsers())
            {
                var userBLL = (UserBLL) user;
                if (userBLL.Session == null || userBLL.Session.Id != sessionId) continue;
                if (userBLL.Session.IsValid())
                    return userBLL.Session;
                return null;
            }
            return null;
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
            AddUser(username, password, Utils.GenerateSalt());
        }

        public void Delete()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            DeleteUsers();
            DeleteAuctions();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var site = context.Sites.Find(this.Name);
                context.Entry(site).State = EntityState.Deleted;
                context.SaveChanges();
            }
            IsDeleted = true;
        }

        public void CleanupSessions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                foreach (var iUser in GetUsers())
                {
                    var user = iUser as UserBLL;
                    if (user?.Session != null)
                        if (!user.Session.IsValid())
                        {
                            var dbUser = context.Users.Find(user.UserID);
                            if(null == dbUser)
                                throw new InvalidOperationException();
                            dbUser.Session = null;
                            dbUser.SessionId = null;
                            context.Entry(dbUser).State = EntityState.Modified;
                            context.Entry(context.Sessions.Find(user.Session.Id)).State = EntityState.Deleted;
                        }
                }
                context.SaveChanges();
            }
        }


        /* AUX METHODS */
        private bool IsUsernameAlreadyUsedInSite(string username)
        {
            var users = GetUsers();
            if (null == users)
                return false;
            foreach (var user in users)
                if (user.Username == username)
                    return true;
            return false;
        }

        private void AddUser(string username, string password, byte[] salt)
        {
            User user;
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                user = new User()
                {
                    Username = username,
                    Password = Convert.ToBase64String(Utils.HashPassword(password, salt)),
                    Salt = Convert.ToBase64String(salt),
                    SiteName = Name
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        private UserBLL GetUserByUsername(string username)
        {
            foreach (var user in GetUsers())
            {
                if (user.Username == username)
                    return user as UserBLL;
            }
            return null;
        }

        private SessionBLL GetUserSession(UserBLL user)
        {
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var session = context.Sessions.Find(Utils.CreateSessionId(this, user));
                if(null == session)
                {
                    user.Session = Utils.CreateNewSession(this, user);
                    return user.Session;
                }
                var sessionBLL = new SessionBLL(session, user);
                if (!sessionBLL.IsValid())
                {
                    sessionBLL.Logout();
                    user.Session = Utils.CreateNewSession(this, user);
                }
                else
                {
                    var validUntil = AlarmClock.Now.AddSeconds(SessionExpirationInSeconds);
                    session.ValidUntil = validUntil;
                    user.Session.ValidUntil = validUntil;
                    context.Entry(session).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return user.Session;
            }
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

        public IEnumerable<IAuction> GetEndedAuctions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var auctions = (
                    from auction in context.Auctions
                    where auction.SiteName == Name && auction.EndsOn < AlarmClock.Now
                    select auction
                ).ToList();
                return Utils.AuctionsToAuctionsBLL(auctions, this);
            }
        }

        /* END AUX METHODS */
    }
}