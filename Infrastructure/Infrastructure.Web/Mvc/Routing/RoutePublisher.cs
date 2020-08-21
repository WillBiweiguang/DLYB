using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
//using Infrastructure.Core.Plus;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core;
using Infrastructure.Core.Plugins;
using System.Web.Mvc;

namespace Infrastructure.Web.Mvc.Routing
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

            foreach (var plugin in PluginManager.ReferencedPlugins)
            {
                if (!plugin.IsEnabled || plugin.ReferencedAssembly.FullName == null)
                    continue;

                if (plugin.ReferencedAssembly.FullName == providerType.Assembly.FullName)
                    return plugin;
            }

            return null;
        }

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        public virtual void RegisterRoutes(RouteCollection routes)
        {
            var routeProviderTypes = typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = new List<IRouteProvider>();
            foreach (var providerType in routeProviderTypes)
            {
                //Ignore not installed plugins
                var plugin = FindPlugin(providerType);
                if (plugin != null && !plugin.Installed)
                    continue;

                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }
            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp =>
            {
                AreaRegistrationContext context = new AreaRegistrationContext(rp.ModuleName, routes);

                string thisNamespace = rp.GetType().Namespace;
                if (thisNamespace != null)
                {
                    context.Namespaces.Add(thisNamespace + ".*");
                }

                rp.RegisterRoutes(context);
            }
            );
        }


    }
}
