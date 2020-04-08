using System.Collections.Generic;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SiteLogic : ISite
    {
        public string Name { get; }
        public int Timezone { get; }
        public int SessionExpirationInSeconds { get; }
        public double MinimumBidIncrement { get; }
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
    }
}