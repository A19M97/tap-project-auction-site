using Mugnai.Model;
using System.Collections.Generic;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class UserBLL : IUser
    {
        public string Username { get; }

        public SessionBLL Session { get; }

        public UserBLL(User user)
        {
            Username = user.Username;
            Session = new SessionBLL(user.Session, this);
        }
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