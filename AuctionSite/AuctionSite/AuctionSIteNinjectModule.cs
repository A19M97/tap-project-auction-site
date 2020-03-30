using System.Data.Entity;
using System.Security;
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
            this.Bind<ISite>().To<Site>();
            this.Bind<ISession>().To<Session>();
            this.Bind<IUser>().To<User>();
            this.Bind<IAuction>().To<Auction>();
        }
    }

    
}
