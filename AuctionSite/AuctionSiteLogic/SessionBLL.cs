using Mugnai.Model;
using System;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SessionBLL : ISession
    {
        public string Id { get; }
        public DateTime ValidUntil { get; }
        public IUser User { get; }

        public SessionBLL(Session session, IUser user)
        {
            Id = session.Id;
            ValidUntil = session.ValidUntil;
            User = user;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is ISession)) return false;
            return ((ISession) obj).Id == this.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public IAuction CreateAuction(string description, DateTime endsOn, double startingPrice)
        {
            throw new NotImplementedException();
        }
    }
}