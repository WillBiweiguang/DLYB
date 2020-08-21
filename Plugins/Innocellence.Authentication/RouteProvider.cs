using System.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;

namespace DLYB.Authentication
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
            routes.MapRoute("DLYB.Authentication.SSO",
                   "OWinLogin/ssoresult",
                   new { controller = "OWinLogin", action = "ssoresult", id = UrlParameter.Optional },
                   new[] { "DLYB.Authentication.Controllers" }
              );
            routes.MapRoute("DLYB.Authentication.SsoResult",
                "Sso/SsoResult",
                new { controller = "sso", action = "ssoresult", id = UrlParameter.Optional },
                new[] { "DLYB.Authentication.Controllers" }
          );
            routes.MapRoute("DLYB.Authentication.oauth", 
                "sso/{controller}/{action}/{id}", 
                new { controller = "OAuth", action = "OAuthToken", id = UrlParameter.Optional }, 
                new[] { "DLYB.Authentication.Controllers" }
        );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }


        public string ModuleName
        {
            get { return "Authentication"; }
        }
    }
}
