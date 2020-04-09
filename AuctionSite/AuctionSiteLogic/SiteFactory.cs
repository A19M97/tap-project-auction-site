using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Mugnai._aux.utils;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SiteFactory : ISiteFactory
    {
        public void Setup(string connectionString)
        {
            if (null == connectionString)
                throw new ArgumentNullException();
            using (var context = new AuctionSiteContext(connectionString))
            {
                context.Database.Delete();
                try
                {
                    context.Database.Create();
                }
                catch (SqlException e)
                {
                    throw new UnavailableDbException("Database connection error.", e);
                }
            }
        }

        public IEnumerable<string> GetSiteNames(string connectionString)
        {
            throw new NotImplementedException();
        }

        public void CreateSiteOnDb(string connectionString, string name, int timezone, int sessionExpirationTimeInSeconds,
            double minimumBidIncrement)
        {
            throw new NotImplementedException();
        }

        public ISite LoadSite(string connectionString, string name, IAlarmClock alarmClock)
        {
            throw new NotImplementedException();
        }

        public int GetTheTimezoneOf(string connectionString, string name)
        {
            throw new NotImplementedException();
        }
    }
}