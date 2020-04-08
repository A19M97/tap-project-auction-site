using System.Collections.Generic;
using TAP2018_19.AuctionSite.Interfaces;

namespace AuctionSiteLogic
{
    public class UserLogic : IUser
    {
        public string Username { get; }
        public IEnumerable<IAuction> WonAuctions()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}