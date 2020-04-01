using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            if(Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if(null == Users)
                return  new List<User>();
            return Users;
        }

        public IEnumerable<ISession> GetSessions()
        {
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

        }

        public IEnumerable<IAuction> GetAuctions(bool onlyNotEnded)
        {
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

        }

        public ISession Login(string username, string password)
        {
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
        }

        public ISession GetSession(string sessionId)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == sessionId)
                throw new ArgumentNullException();
            foreach (var user in Users)
                if (user.Session.Id == sessionId && user.Session.IsValid())
                    return user.Session;
            return null;
        }

        public void CreateUser(string username, string password)
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null == username || null == password)
                throw new ArgumentNullException();
            if(!IsValidUsername(username) || !IsValidPassword(password))
                throw new ArgumentException();

            if (IsUsernameAlreadyUsedInSite(username)) 
                throw new NameAlreadyInUseException("Username already used: "+username);

           AddUser(username, password);
        }

        public void Delete()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            if (null != Users)
                foreach (var user in Users)
                    user.Delete();
            if (null != Auctions)
                foreach (var auction in Auctions)
                    auction.Delete();
            IsDeleted = true;
        }

        public void CleanupSessions()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            throw new System.NotImplementedException();
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
            if (null == Users)
                Users = new List<User>();
            Users.Add(
                new User{
                    Username = username,
                    Password = password
                }
            );
        }

        private bool IsUsernameAlreadyUsedInSite(string username)
        {
            foreach (var user in GetUsers())
                if (user.Username == username)
                    return true;
            return false;
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

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public IAlarmClock AlarmClock;
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Auction> Auctions { get; set; }

        public IAlarm Alarm;
    }
}