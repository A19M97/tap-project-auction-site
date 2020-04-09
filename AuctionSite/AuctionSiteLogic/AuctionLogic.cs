﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAP2018_19.AuctionSite.Interfaces;

namespace AuctionSiteLogic
{
    class AuctionLogic : IAuction
    {
        public int Id { get; }
        public IUser Seller { get; }
        public string Description { get; }
        public DateTime EndsOn { get; }
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