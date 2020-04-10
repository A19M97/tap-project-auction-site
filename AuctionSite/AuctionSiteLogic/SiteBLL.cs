using System.Collections.Generic;
using TAP2018_19.AlarmClock.Interfaces;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai
{
    public class SiteBLL : ISite
    {
        public string Name { get; }
        public int Timezone { get; }
        public int SessionExpirationInSeconds { get; }
        public double MinimumBidIncrement { get; }
        public IAlarmClock AlarmClock { get; }
        public IAlarm Alarm { get; }


        public SiteBLL(Site site, IAlarmClock alarmClock)
        {
            Name = site.Name;
            Timezone = site.Timezone;
            SessionExpirationInSeconds = site.SessionExpirationInSeconds;
            MinimumBidIncrement = site.MinimumBidIncrement;
            AlarmClock = alarmClock;
            Alarm = AlarmClock.InstantiateAlarm(5 * 60 * 1000); /* 5*60*1000 = 300000 = 5 minutes */
            Alarm.RingingEvent += CleanupSessions;
        }

        

        public IEnumerable<IUser> GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ISession> GetSessions()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IAuction> GetAuctions(bool onlyNotEnded)
        {
            throw new System.NotImplementedException();
        }

        public ISession Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public ISession GetSession(string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public void CreateUser(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public void CleanupSessions()
        {
            throw new System.NotImplementedException();
        }


        /* AUX METHODS */
        
        /* END AUX METHODS */
    }
}