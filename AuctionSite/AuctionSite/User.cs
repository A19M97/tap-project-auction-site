using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class User : IUser
    {
        public User() { }

        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

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
        public string Password { get; set; }
        public string SiteName { get; set; }

        [ForeignKey("SiteName")]
        public ISite Site { get; set; }

        public virtual Session Session { get; set; }
    }
}