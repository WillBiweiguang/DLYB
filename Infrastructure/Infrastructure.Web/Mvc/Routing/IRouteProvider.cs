using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Infrastructure.Web.Mvc.Routing
{
    public interface IRouteProvider
    {
       // void RegisterRoutes(RouteCollection routes);

        void RegisterRoutes(AreaRegistrationContext routes);

        int Priority { get; }

        string ModuleName {  get; }
    }
}
