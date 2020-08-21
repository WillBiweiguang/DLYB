using Infrastructure.Core;
using Infrastructure.Core.Events;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web;
using Infrastructure.Web.MVC;
using Microsoft.Owin;
using Owin;
using System;
using System.IO;
using System.Web;

[assembly: OwinStartupAttribute(typeof(DLYB.Web.Startup))]
[assembly: PreApplicationStartMethod(typeof(DLYB.Web.Startup), "Start")]
namespace DLYB.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IOWinAuthorizeProvider>();
            foreach (var startUpTaskType in startUpTaskTypes)
                ((IOWinAuthorizeProvider)Activator.CreateInstance(startUpTaskType)).ConfigureAuth(app);

        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        public static void Start()
        {
            // 初始化可能需要的目录
            var siteRootDirPath = HttpRuntime.AppDomainAppPath.TrimEnd('\\');
            if (!Directory.Exists(siteRootDirPath + "\\marks"))
            {
                Directory.CreateDirectory(siteRootDirPath + "\\marks");
            }
            if (!Directory.Exists(siteRootDirPath + "\\temp"))
            {
                Directory.CreateDirectory(siteRootDirPath + "\\temp");
            }
            if (!Directory.Exists(siteRootDirPath + "\\wxUserImage"))
            {
                Directory.CreateDirectory(siteRootDirPath + "\\wxUserImage");
            }
            if (!Directory.Exists(siteRootDirPath + "\\pluginsTemp"))
            {
                Directory.CreateDirectory(siteRootDirPath + "\\pluginsTemp");
            }

            //设置全局Context对象
            GlobalApplicationObject.Current.ApplicationContext = new ApplicationContext();

            GlobalApplicationObject.Current.ApplicationContext.PreApplicationStartInitialize();
            //执行预初始化事件
            GlobalApplicationObject.Current.EventsManager.InitApplicationEvents(null, ApplicationEvents.OnApplication_PreInitialize);
        }
    }
}
