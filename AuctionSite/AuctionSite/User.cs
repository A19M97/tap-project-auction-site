using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class User : IUser
    {
        public IEnumerable<IAuction> WonAuctions()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public int UserID { get; }
        [MinLength(DomainConstraints.MinUserName)]
        public string Username { get; }
        public ISite Site { get; }
        public ISession Session { get; }
    }
}