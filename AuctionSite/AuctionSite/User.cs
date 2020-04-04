using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class User : IUser
    {
        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is User)) return false;
            return ((User)obj).UserID == this.UserID;
        }

        public override int GetHashCode() => UserID.GetHashCode();


        public IEnumerable<IAuction> WonAuctions()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MinLength(DomainConstraints.MinUserName)]
        public string Username { get; set; }
        public string Password { get; set; }
        public string SiteName { get; set; }

        [ForeignKey("SiteName")]
        public ISite Site { get; set; }
        public virtual string SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}