using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public void CleanupSessions()
        {
            throw new System.NotImplementedException();
        }

        [Key, MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName)]
        public string Name { get; set; }

        [Range(DomainConstraints.MinTimeZone, DomainConstraints.MaxTimeZone)]
        public int Timezone { get; set; }

        [Range(1, int.MaxValue)]
        public int SessionExpirationInSeconds { get; set; }

        [Range(1, double.MaxValue)]
        public double MinimumBidIncrement { get; set; }
        public virtual ICollection<IUser> Users { get; set; }
        public virtual ICollection<IAuction> Auctions { get; set; }
    }
}