using System.Web.Mvc;
using Infrastructure.Web.Mvc.Routing;

namespace DLYB.FaultSearch
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(AreaRegistrationContext routes)
        {
            routes.MapRoute("FaultSearch", "FaultSearch/{controller}/{action}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { "DLYB.FaultSearch.Controllers" }
           );
        }
        public int Priority
        {
            get
            {
                return 101;
            }
        }
        public string ModuleName
        {
            get { return "FaultSearch"; }
        }
    }
}
