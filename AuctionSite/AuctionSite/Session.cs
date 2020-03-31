using System;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class Session : ISession
    {
        public Session() { }
        public Session(DateTime ValidUntil)
        {
            this.ValidUntil = ValidUntil;
        }

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

        public string Id { get; set; }
        public DateTime ValidUntil { get; set; }
        public IUser User { get; set; }
    }
}