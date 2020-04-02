﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    from _users in context.Users
                    where _users.SiteName == Name
                    select _users
                ).ToList();
                return users;
            }
        }

        public IEnumerable<ISession> GetSessions()
        {
            throw new NotImplementedException();
            /*
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            var sessions = new List<ISession>();
            if (null == Users)
                return sessions;
            foreach (var user in Users)
            {
                if (null != user.Session)
                    sessions.Add(user.Session);
            }
            return sessions;
            */
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
                        from _auctions in context.Auctions
                        select _auctions
                    ).ToList();
                }else {
                    auctions = (
                        from _auctions in context.Auctions
                        where _auctions.EndsOn > this.AlarmClock.Now
                        select _auctions
                    ).ToList();
                }
                return auctions;
            }

            /*
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == Auctions)
                Auctions = new List<Auction>();
            if (!onlyNotEnded)
                return Auctions;
            var auctionsNotEnded = new List<IAuction>();
            foreach (var auction in Auctions)
                if (!Utils.IsEndedAuction(auction))
                    auctionsNotEnded.Add(auction);
            return auctionsNotEnded;
            */
        }

        public ISession Login(string username, string password)
        {
            throw new NotImplementedException();
            /*
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if(null == username || null == password)
                throw new ArgumentNullException();
            if (!IsValidUsername(username) || !IsValidPassword(password))
                throw new ArgumentException();

            User user = GetUserByUsername(username);

            if (!Utils.ArePasswordsEquals(user.Password, password))
                return null;

            if (null == user.Session || !user.Session.IsValid())
                user.Session = Utils.CreateNewSession(this, user);
            return user.Session;
            */
        }

        public ISession GetSession(string sessionId)
        {
            throw new NotImplementedException();
            /*
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == sessionId)
                throw new ArgumentNullException();
            foreach (var user in Users)
                if (user.Session.Id == sessionId && user.Session.IsValid())
                    return user.Session;
            return null;
            */
        }

        public void CreateUser(string username, string password)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == username || null == password)
                throw new ArgumentNullException();
            if (!IsValidUsername(username) || !IsValidPassword(password))
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
                        from _sessions in context.Sessions
                        select _sessions
                    );
                foreach (var session in sessions)
                    if (!session.IsValid())
                        context.Entry(session).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        /*AUX METHODS*/

        private bool IsValidUsername(string username)
        {
            var usernameLength = username.Length;
            return usernameLength >= DomainConstraints.MinUserName && usernameLength <= DomainConstraints.MaxUserName;
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= DomainConstraints.MinUserPassword;
        }

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