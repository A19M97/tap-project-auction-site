using System.Data.Entity;
using System.Security;
using AuctionSiteLogic;
using Ninject.Modules;
using TAP2018_19.AuctionSite.Interfaces;

//[assembly: AllowPartiallyTrustedCallers]

namespace Mugnai
{
    [SecurityCritical]
    public class AuctionSiteNinjectModule : NinjectModule
    {
        [SecurityCritical]
        public override void Load()
        {
            Database.SetInitializer<AuctionSiteContext>(null);
            this.Bind<ISiteFactory>().To<SiteFactory> ();
            this.Bind<ISite>().To<SiteLogic>();
            this.Bind<ISession>().To<SessionLogic>();
            this.Bind<IUser>().To<UserLogic>();
            this.Bind<IAuction>().To<AuctionLogic>();
        }
    }

    
}
