using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mugnai._aux.utils;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class Site : ISite
    {
        public IEnumerable<IUser> GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ISession> GetSessions()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IAuction> GetAuctions(bool onlyNotEnded)
        {
            throw new NotImplementedException();
            //if(null == Auctions)
            //    Auctions = new List<IAuction>();
            //if (!onlyNotEnded)
            //    return Auctions;
            //var auctionsNotEnded = new List<IAuction>();
            //foreach (var auction in Auctions)
            //    if(!Utils.IsEndedAuction(auction))
            //        auctionsNotEnded.Add(auction);
            
        }

        public ISession Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public ISession GetSession(string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateUser(string username, string password)
        {
            if (null == username || null == password)
                throw new ArgumentNullException();
            if(!IsValidUsername(username) || !IsValidPassword(password))
                throw new ArgumentException();

            if (IsUsernameAlreadyUsedInSite(username)) 
                throw new NameAlreadyInUseException("Username already used: "+username);

            AddUser(username);
        }

        public void Delete()
        {
            if(null != Users)
                foreach (var user in Users)
                    user.Delete();
            if (null != Auctions)
                foreach (var auction in Auctions)
                    auction.Delete();

        }

        public void CleanupSessions()
        {
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

        private void AddUser(string username)
        {
            if (null == Users)
                Users = new List<User>();
            Users.Add(new User(username));
        }

        private bool IsUsernameAlreadyUsedInSite(string username)
        {
            if (null == Users)
                return false;
            foreach (var user in Users)
                if (user.Username == username)
                    return true;
            return false;
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
    }
}