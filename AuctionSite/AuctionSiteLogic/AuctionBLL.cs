﻿using Mugnai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    class AuctionBLL : IAuction
    {
        public int Id { get; }
        public IUser Seller { get; }
        public string Description { get; }
        public DateTime EndsOn { get; }

        public AuctionBLL(Auction auction, IUser seller)
        {
            Id = auction.Id;
            Seller = seller;
            Description = auction.Description;
            EndsOn = auction.EndsOn;
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (!(obj is IAuction)) return false;
            return ((IAuction)obj).Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public IUser CurrentWinner()
        {
            throw new NotImplementedException();
        }

        public double CurrentPrice()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool BidOnAuction(ISession session, double offer)
        {
            throw new NotImplementedException();
        }
    }
}
