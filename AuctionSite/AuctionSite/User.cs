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

        public int UserID { get; set; }
        [MinLength(DomainConstraints.MinUserName)]
        public string Username { get; set; }
        public ISite Site { get; set; }
        public ISession Session { get; set; }
    }
}