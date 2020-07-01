using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Mugnai._aux.utils;
using Mugnai.Model;
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
                try
                {
                    context.Database.Delete();
                    context.Database.Create();
                }
                catch (Exception e)
                {
                    throw new UnavailableDbException("Database connection error.", e);
                }
            }
        }

        public IEnumerable<string> GetSiteNames(string connectionString)
        {
            if (null == connectionString)
                throw new ArgumentNullException();
            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!ExistsDb(context))
                    throw new UnavailableDbException();
                return (
                    from site in context.Sites
                    select site.Name).ToList();
            }
        }

        public void CreateSiteOnDb(string connectionString, string name, int timezone, int sessionExpirationTimeInSeconds,
            double minimumBidIncrement)
        {
            if (null == connectionString || null == name)
                throw new ArgumentNullException();

            if (!IsValidSiteName(name))
                throw new ArgumentException();

            if (!IsValidTimezone(timezone))
                throw new ArgumentOutOfRangeException();

            if (!IsPositiveSessionExpiration(sessionExpirationTimeInSeconds) || !IsPositiveMinimumBidIncrement(minimumBidIncrement))
                throw new ArgumentOutOfRangeException();

            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!ExistsDb(context))
                    throw new UnavailableDbException();
                if (Utils.SiteNameAlreadyExists(context, name))
                    throw new NameAlreadyInUseException(name);

                var site = new Site()
                {
                    Name = name,
                    Timezone = timezone,
                    SessionExpirationInSeconds = sessionExpirationTimeInSeconds,
                    MinimumBidIncrement = minimumBidIncrement
                };
                context.Sites.Add(site);
                context.SaveChanges();
            }
        }

        public ISite LoadSite(string connectionString, string name, IAlarmClock alarmClock)
        {
            if (null == connectionString || null == name || null == alarmClock)
                throw new ArgumentNullException();
            if (!IsValidSiteName(name))
                throw new ArgumentException();
            using (var context = new AuctionSiteContext(connectionString))
            {

                if (!ExistsDb(context))
                    throw new UnavailableDbException();
                var site =
                    (from siteDb in context.Sites//.Include("Users").Include("Users.Session").Include("Users.Site")
                        where siteDb.Name == name
                        select siteDb).FirstOrDefault();

                if (null == site)
                    throw new InexistentNameException(name);
                if (site.Timezone != alarmClock.Timezone)
                    throw new ArgumentException();
                return new SiteBLL(site, alarmClock, connectionString);
            }
        }

        public int GetTheTimezoneOf(string connectionString, string name)
        {
            if (null == connectionString || null == name)
                throw new ArgumentNullException();
            if (!IsValidSiteName(name))
                throw new ArgumentException();
            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!ExistsDb(context))
                    throw new UnavailableDbException();
                var site =
                    (from siteDb in context.Sites
                        where siteDb.Name == name
                        select siteDb).FirstOrDefault();

                if (null == site)
                    throw new InexistentNameException(name);

                return site.Timezone;
            }
        }

        /* AUX METHODS*/
        private static bool ExistsDb(AuctionSiteContext context)
        {
            return context.Database.Exists();
        }

        private static bool IsPositiveMinimumBidIncrement(double minimumBidIncrement)
        {
            return minimumBidIncrement > 0;
        }

        private static bool IsPositiveSessionExpiration(int sessionExpirationTimeInSeconds)
        {
            return sessionExpirationTimeInSeconds > 0;
        }

        private static bool IsValidTimezone(int timezone)
        {
            return timezone >= DomainConstraints.MinTimeZone && timezone <= DomainConstraints.MaxTimeZone;
        }

        private static bool IsValidSiteName(string name)
        {
            var nameLength = name.Length;
            return nameLength >= DomainConstraints.MinSiteName && nameLength <= DomainConstraints.MaxSiteName;
        }
        /* END AUX METHODS*/
    }
}