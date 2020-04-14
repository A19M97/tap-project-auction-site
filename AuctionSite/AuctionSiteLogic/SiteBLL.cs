using System;
using System.Collections.Generic;
using System.Linq;
using Mugnai._aux.utils;
using Mugnai.Model;
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
        public string ConnectionString { get; }

        public bool IsDeleted;

        public SiteBLL(Site site, IAlarmClock alarmClock, string connectionString)
        {
            Name = site.Name;
            Timezone = site.Timezone;
            SessionExpirationInSeconds = site.SessionExpirationInSeconds;
            MinimumBidIncrement = site.MinimumBidIncrement;
            AlarmClock = alarmClock;
            Alarm = AlarmClock.InstantiateAlarm(5 * 60 * 1000); /* 5*60*1000 = 300000 = 5 minutes */
            Alarm.RingingEvent += CleanupSessions;
            this.ConnectionString = connectionString;
        }

        

        public IEnumerable<IUser> GetUsers()
        {
            if (Utils.IsSiteDisposed(this))
                throw new InvalidOperationException();
            using (var context = new AuctionSiteContext(ConnectionString))
            {
                var users =
                        (from user in context.Users.Include("Session")
                        where user.SiteName == Name
                        select user).ToList();
                
                return Utils.UsersToUsersBLL(users);
            }
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