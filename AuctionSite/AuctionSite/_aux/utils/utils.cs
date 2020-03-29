using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugnai._aux.utils
{
    public static class Utils
    { 
        public static bool SiteNameAlreadyExists(AuctionSiteContext context, string name)
        {
            return context.Sites.Any(site => site.Name == name);
        }
    }
}
