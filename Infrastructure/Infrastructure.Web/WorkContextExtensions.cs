using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Infrastructure.Core;

namespace Infrastructure.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorkContextExtensions {
        //public static WorkContext GetContext(this IWorkContextAccessor workContextAccessor, ControllerContext controllerContext) {
        //    return workContextAccessor.GetContext(controllerContext.RequestContext.HttpContext);
        //}

        /// <summary>
        /// 
        /// </summary>
        public const string strWorkContextKey = "_WorkContextKey";

        /// <summary>
        /// GetWorkContext
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public static WebWorkContext GetWorkContext(this RequestContext requestContext)
        {
            if (requestContext == null)
                return null;


            return requestContext.HttpContext.Items[strWorkContextKey] as WebWorkContext;
           

            //var routeData = requestContext.RouteData;
            //if (routeData == null || routeData.DataTokens == null)
            //    return null;
            
            //object workContextValue;
            //if (!routeData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue)) {
            //    workContextValue = FindWorkContextInParent(routeData);
            //}

            //if (!(workContextValue is IWorkContextAccessor))
            //    return null;

            //var workContextAccessor = (IWorkContextAccessor)workContextValue;
            //return workContextAccessor.GetContext(requestContext.HttpContext);
        }

        public static WebWorkContext GetWorkContext(this HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
                return null;



            return HttpContext.Current.Items[strWorkContextKey] as WebWorkContext;

            //var routeData = controllerContext.RouteData;
            //if (routeData == null || routeData.Values == null)
            //    return null;

            //object workContextValue;
            //if (!routeData.Values.TryGetValue("IWorkContextAccessor", out workContextValue)) {
            //    return null;
            //}

            //if (workContextValue == null || !(workContextValue is IWorkContextAccessor))
            //    return null;

            //var workContextAccessor = (IWorkContextAccessor)workContextValue;
            //return workContextAccessor.GetContext();
        }

        private static object FindWorkContextInParent(RouteData routeData) {
            object parentViewContextValue;
            if (!routeData.DataTokens.TryGetValue("ParentActionViewContext", out parentViewContextValue)
                || !(parentViewContextValue is ViewContext)) {
                return null;
            }

            var parentRouteData = ((ViewContext)parentViewContextValue).RouteData;
            if (parentRouteData == null || parentRouteData.DataTokens == null)
                return null;

            object workContextValue;
            if (!parentRouteData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue)) {
                workContextValue = FindWorkContextInParent(parentRouteData);
            }

            return workContextValue;
        }

        /// <summary>
        /// GetWorkContext
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static WebWorkContext GetWorkContext(this ControllerContext controllerContext)
        {
            if (controllerContext == null)
                return null;

            return GetWorkContext(controllerContext.RequestContext);
        }

        //public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope, HttpContextBase httpContext) {
        //    return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope(httpContext);
        //}

        //public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope) {
        //    return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope();
        //}
    }
}
