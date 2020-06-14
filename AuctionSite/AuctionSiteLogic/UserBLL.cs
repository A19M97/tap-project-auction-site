using Mugnai.Model;
using System.Collections.Generic;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class UserBLL : IUser
    {
        public int UserID { get; }
        public string Username { get; }
        public string Password { get; }
        public SiteBLL Site { get; }
        public SessionBLL Session { get; set; }

        public UserBLL(User user, SiteBLL site)
        {
            UserID = user.UserID;
            Username = user.Username;
            Password = user.Password;
            Site = site;
            if (null != user.Session)
                Session = new SessionBLL(user.Session, this);
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is UserBLL)) return false;
            return ((UserBLL)obj).UserID == UserID;
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
    }
}