using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mugnai._aux._debug;
using Ninject.Infrastructure.Language;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SiteFactory : ISiteFactory
    {
        public void Setup(string connectionString)
        {
            if(null == connectionString)
                throw new ArgumentNullException();
            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!IsValidConnectionString(context))
                    throw new UnavailableDbException();
                context.Database.Delete();
                context.Database.Create();
            }
        }

        public IEnumerable<string> GetSiteNames(string connectionString)
        {
            if (null == connectionString)
                throw new ArgumentNullException();
            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!IsValidConnectionString(context))
                    throw new UnavailableDbException();
                return ( 
                    from site in context.Sites
                    select site.Name ).ToList();
            }
        }

        public void CreateSiteOnDb(string connectionString, string name, int timezone, int sessionExpirationTimeInSeconds,
            double minimumBidIncrement)
        {
            if(null == connectionString || null == name)
                throw new ArgumentNullException();

            var nameLength = name.Length;
            if(nameLength < DomainConstraints.MinSiteName || nameLength > DomainConstraints.MaxSiteName)
                throw new ArgumentException();

            if (timezone < DomainConstraints.MinTimeZone || timezone > DomainConstraints.MaxTimeZone)
                throw new ArgumentOutOfRangeException();

            if (sessionExpirationTimeInSeconds < 0 || minimumBidIncrement < 0)
                throw new ArgumentOutOfRangeException();

            using (var context = new AuctionSiteContext(connectionString))
            {
                if (!IsValidConnectionString(context))
                    throw new UnavailableDbException();
                if (siteNameAlreadyExists(context, name))
                    throw new NameAlreadyInUseException(name);

                var site = new Site()
                {
                    Name = name,
                    Timezone = timezone,
                    SessionExpirationInSeconds = sessionExpirationTimeInSeconds,
                    MinimumBidIncrement = minimumBidIncrement
                };
                context.Sites.Add(site);
                /*
                 * debug class
                 * @link https://stackoverflow.com/questions/7795300/validation-failed-for-one-or-more-entities-see-entityvalidationerrors-propert
                 */
                context.SaveChanges();
            }
        }

        


        public ISite LoadSite(string connectionString, string name, IAlarmClock alarmClock)
        {
            throw new NotImplementedException();
        }

        public int GetTheTimezoneOf(string connectionString, string name)
        {
            throw new NotImplementedException();
        }

        /* Aux methods*/
        private static bool IsValidConnectionString(AuctionSiteContext context)
        {
            return context.Database.Exists();
        }

        private bool siteNameAlreadyExists(AuctionSiteContext context, string name)
        {
            return context.Sites.Any(site => site.Name == name);
        }
    }
}
