using System.Web.Mvc;
using Autofac;
using Infrastructure.Web.Localization.Services;
using Infrastructure.Web.FileSystems.WebSite;
using Infrastructure.Core.Caching;
using Infrastructure.Web.FileSystems.VirtualPath;
using Infrastructure.Core.Services;
using Infrastructure.Core;

namespace Infrastructure.Web.Localization
{

    public class LocalizationUtilities
    {
        public static Localizer Resolve(WorkContextBase workContext, string scope)
        {
            return workContext == null ? NullLocalizer.Instance : Resolve(workContext.Resolve<ILifetimeScope>(), scope);
        }

        public static Localizer Resolve(ControllerContext controllerContext, string scope)
        {
            var workContext = controllerContext.GetWorkContext();
            return Resolve(workContext, scope);
        }

        public static Localizer Resolve(IComponentContext context, string scope)
        {
            var text = context.Resolve<IText>(new NamedParameter("scope", scope));
            //var text = context.Resolve<IText>();
            return text.Get;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    //public class LocalizationUtilities {
    //    public static Localizer Resolve(WorkContextBase workContext, string scope)
    //    {
    //       // return workContext == null ? NullLocalizer.Instance : Resolve(workContext.Resolve<ILifetimeScope>(), scope);
    //        var text = new Text(scope,workContext,
    //           new DefaultLocalizedStringManager(
    //           new WebSiteFolder(new DefaultVirtualPathMonitor(new Clock()), new DefaultVirtualPathProvider()), new Signals()) { DisableMonitoring = true });// context.Resolve<IText>(new NamedParameter("scope", scope));
    //        return text.Get;
    //    }

    //    public static Localizer Resolve(ControllerContext controllerContext, string scope) {
    //       // var workContext = controllerContext.GetWorkContext();
    //        return Resolve((IComponentContext)null, scope);
    //    }

    //    public static Localizer Resolve(IComponentContext context, string scope) {
    //        //var text = new Text(scope,
    //        //    new DefaultLocalizedStringManager(
    //        //    new WebSiteFolder(new DefaultVirtualPathMonitor(new Clock()), new DefaultVirtualPathProvider()), new Signals()){  DisableMonitoring=true }) ;// context.Resolve<IText>(new NamedParameter("scope", scope));
    //        //return text.Get;
    //        return null;
    //    }
    //}
}
