using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Infrastructure.Core.Infrastructure;

using Infrastructure.Core.Infrastructure.DependencyManagement;
using Infrastructure.Core.Configuration;
using System.Reflection;
using Autofac.Core;

namespace Infrastructure.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// </summary>
    public class WebEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;
        private WebCoreConfig _config;
        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies(WebCoreConfig config)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //we create new instance of ContainerBuilder
            //because Build() or Update() method can only be called once on a ContainerBuilder.


            //dependencies
            var typeFinder = new WebAppTypeFinder(config);
            builder = new ContainerBuilder();
            builder.RegisterInstance(config).As<WebCoreConfig>().SingleInstance();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            builder.Update(container);

            //register dependencies provided by other assemblies
            builder = new ContainerBuilder();
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar) Activator.CreateInstance(drType));
            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder);
            builder.Update(container);



            builder = new ContainerBuilder();
            drTypes = typeFinder.FindClassesOfType<IDependency>();
            foreach (var item in drTypes)
            {

                if (item.IsGenericType) { continue; }
               // if (item.Name.IndexOf("Service") < 0 || item.Name.IndexOf("BaseService") >= 0) { continue; }
                var temp = builder.RegisterType(item);
                foreach (var it in item.GetInterfaces().Where(itf => typeof(IDependency).IsAssignableFrom(itf)).Reverse())
                {
                    temp.As(it).InstancePerLifetimeScope();
                    //break;
                }
            }

            builder.Update(container);

            //DatabaseInitialize(typeFinder.GetAssemblies());

            this._containerManager = new ContainerManager(container);
            
            //set dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(WebCoreConfig config)
        {
            _config = config;
            //register dependencies
            RegisterDependencies(config);

            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }



        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
		{
            return ContainerManager.Resolve<T>();
		}

        public T Resolve<T>(params Parameter[] parameters) where T : class
        {
            return ContainerManager.Resolve<T>(parameters);
        }

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }
        
        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }


        public WebCoreConfig WebConfig
        {
            get { return _config; }
        }

        #endregion
    }
}
