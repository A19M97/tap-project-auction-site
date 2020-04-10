using System;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SessionBLL : ISession
    {
        public string Id { get; }
        public DateTime ValidUntil { get; }
        public IUser User { get; }
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