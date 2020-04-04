using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class Session : ISession
    {
        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is ISession)) return false;
            return (obj as ISession).Id == this.Id;
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
            if (!IsValid())
                throw new InvalidOperationException();
            if (null == description)
                throw new ArgumentNullException();
            if ("" == description)
                throw new ArgumentException();
            if(startingPrice < 0)
                throw new ArgumentOutOfRangeException();
            throw new NotImplementedException();
        }

        public string Id { get; set; }
        public DateTime ValidUntil { get; set; }
        public virtual IUser User { get; set; }


    }
}