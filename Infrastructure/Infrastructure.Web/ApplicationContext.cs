using Infrastructure.Core.Performance.Watch;
//using Infrastructure.Core.Plus;
using Infrastructure.Core;
//using Infrastructure.Core.Plus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Infrastructure.Utility;
//using Infrastructure.Core.Res;
//using Infrastructure.Core.Strategy.Logger;
using System.Web.Compilation;
//using Infrastructure.Core.Strategy;
//using Infrastructure.Core.Plus.Info;
//using Infrastructure.Core.Strategy;
//using Infrastructure.Core.Strategy.Cache;
//using Infrastructure.Core.Strategy.Email;
//using Infrastructure.Core.Strategy.Session;
//using Infrastructure.Core.Strategy.SMS;
using Infrastructure.Core.Config;
//using Infrastructure.Core.Strategy.User;
//using Infrastructure.Core.Routing;
using System.Web;
//using Infrastructure.Core.Data.API.SiteNavs;
//using Infrastructure.Core.Data.API;
//using Infrastructure.Core.DocumentProtocols;
using Infrastructure.Web.Mvc.Routing;
using Infrastructure.Core.Plugins;
using Infrastructure.Web.Logging;
using Infrastructure.Core.Logging;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;

namespace Infrastructure.Web
{
    /// <summary>
    /// 应用程序上下文对象
    /// </summary>
    public class ApplicationContext : ApplicationContextBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            #region 初始化路由
           // this.RoutingManager = new RoutingManager();
           // this.RoutingManager.Initialize();
            #endregion
            //初始化全局配置
            GlobalConfigurationManager.Config();
            using (var watch = new CodeWatch("Initialize", 20000))
            {
                #region 【废弃】处理插件
                using (new CodeWatch("LoadPlusAndPlusResource", 5000))
                {
                    //var taskDoingList = new List<Task>();
                    #region 【废弃】加载程序集资源
                    //foreach (var plus in PluginManager.InstalledPluginsList)
                    //{
                    //    var loadPlusTask = new Task((plusAss) => CurrentResourceHelper.LoadPlusAndPlusResource((IPlusInfo)plusAss), plus);
                    //    taskDoingList.Add(loadPlusTask);
                    //    loadPlusTask.Start();
                    //}
                    //Task.WaitAll(taskDoingList.ToArray());
                    //taskDoingList.Clear();
                    #endregion

                    #region 【废弃】初始化程序集
                    //foreach (var plus in PlusAssemblysList)
                    //{
                    //    //程序集初始化
                    //    var initTask = new Task((plusInfo) =>
                    //    {
                    //        var plu = (IPlusInfo)plusInfo;
                    //        CurrentResourceHelper.AssemblyInitialize(plu.Assembly, plu.PlusAssemblys);
                    //    }, plus);
                    //    taskDoingList.Add(initTask);
                    //    initTask.Start();
                    //}
                    //Task.WaitAll(taskDoingList.ToArray());
                    //taskDoingList.Clear();
                    #endregion

                    #region 【废弃】执行程序集初始化函数
                    //foreach (var plus in PluginManager.PluginsList)
                    //{
                    //    var loadingTask = new Task((plusInfo) =>
                    //    {
                    //        var plu = (IPlusInfo)plusInfo;
                    //        //执行插件Loading函数
                    //        plu.Assembly.GetTypes().Where(p => p.IsClass && p.GetInterface(typeof(IPlus).FullName) != null).Each(
                    //            t =>
                    //            {
                    //                using (new CodeWatch("Plu Initialize", ApplicationLog, 3000))
                    //                {
                    //                    try
                    //                    {
                    //                        var type = (IPlus)Activator.CreateInstance(t);
                    //                        type.Initialize();
                    //                    }
                    //                    catch (Exception ex)
                    //                    {
                    //                        ApplicationLog.Log(LoggerLevels.Error, string.Format("Assembly:{0}，Type:{1}{2}", plu.Assembly.FullName, t.FullName, Environment.NewLine), ex);
                    //                    }
                    //                }
                    //            });
                    //    }, plus);
                    //    taskDoingList.Add(loadingTask);
                    //    loadingTask.Start();
                    //}
                    //Task.WaitAll(taskDoingList.ToArray());
                    //taskDoingList.Clear();
                    #endregion
                }
                #endregion
            }
        }

        ///// <summary>
        ///// 加载应用程序集
        ///// </summary>
        ///// <param name="folder"></param>
        //public override void LoadAssemblies()
        //{
        //    if (PlusAssemblysList == null)
        //        PlusAssemblysList = new List<PluginDescriptor>();
        //    else
        //        PlusAssemblysList.Clear();

        //    #region 插件文件目录
        //    var plusFilesDirectoryInfo = new DirectoryInfo(SitePaths.PlusFilesDirPath);
        //    #endregion
        //    //插件前缀必须为DLYB
        //    var plusDlls = plusFilesDirectoryInfo.GetFiles("*.dll", SearchOption.AllDirectories).ToList();
        //    if (plusDlls.Count == 0) return;
        //    //插件程序集
        //    //必须是DLYB插件才会执行插件部署
        //    //dll名称必须与插件目录名称一致才能部署
        //    //{PlusDir}/{dllName}
        //    //{PlusDir}/bin/{dllName}
        //    var magicodesPlusDlls = plusDlls.Where(p => p.Name.StartsWith("DLYB.") && (p.Directory.Name + ".dll" == p.Name || p.Directory.Parent.Name + ".dll" == p.Name));
        //    //依赖的程序集
        //    var orthersDlls = plusDlls.Where(p => !p.Name.StartsWith("DLYB.")).Distinct();
        //    #region 设置程序域
        //    //var setup = new AppDomainSetup
        //    //{
        //    //    ApplicationName = "Infrastructure.Core",
        //    //    //DynamicBase = SitePaths.PlusShadowCopyDirPath,
        //    //    PrivateBinPath = SitePaths.PlusShadowCopyDirPath,
        //    //    DisallowCodeDownload = true,
        //    //    ShadowCopyFiles = "true",
        //    //    CachePath = SitePaths.PlusCacheDirPath,
        //    //    ShadowCopyDirectories = SitePaths.PlusShadowCopyDirPath,
        //    //    //PrivateBinPath = SitePaths.PlusShadowCopyDirPath
        //    //};
        //    //var appDomain = AppDomain.CreateDomain("Infrastructure.Core.Domain", null, setup);
        //    CurrentAppDomain = AppDomain.CurrentDomain;
        //    if (!CurrentAppDomain.IsFullyTrusted)
        //        throw new DLYBException("请将当前应用程序信任级别设置为完全信任");

        //    #endregion
        //    var binDir = new DirectoryInfo(SitePaths.SiteRootBinDirPath);

        //    #region 部署依赖程序集
        //    foreach (var plus in orthersDlls)
        //    {
        //        //如果网站bin目录不存在此dll，则将该dll复制到动态程序集目录
        //        if (binDir.GetFiles(plus.Name).Length == 0 && PluginManager.DynamicDirectory.GetFiles(plus.Name).Length == 0)
        //        {
        //            _PluginManager.CopyToDynamicDirectory(plus);
        //            Assembly assembly = Assembly.LoadFrom(plus.FullName);
        //            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //            if (!assemblies.Any(p => p.FullName == assembly.FullName))
        //                //将程序集添加到当前应用程序域
        //                BuildManager.AddReferencedAssembly(assembly);
        //        }

        //    }
        //    #endregion
        //    #region 部署插件程序集
        //    foreach (var plus in magicodesPlusDlls)
        //    {
        //        PluginManager.Deploy(plus);
        //    }
        //    #endregion
        //    #region 添加插件菜单
        //    var siteAdminNavigationRepository = APIContext<string>.Current.SiteAdminNavigationRepository;
        //    if (siteAdminNavigationRepository == null) return;
        //    #region 移除所有的插件菜单
        //    if (siteAdminNavigationRepository.GetQueryable().Any())
        //    {
        //        siteAdminNavigationRepository.RemoveRange(siteAdminNavigationRepository.GetQueryable());
        //        siteAdminNavigationRepository.SaveChanges();
        //    }
        //    #endregion
        //    //foreach (var plusInfo in PluginManager.ReferencedPlugins)
        //    //{
        //    //    if (plusInfo.PlusConfigInfo != null && plusInfo.PlusConfigInfo.PlusMenus != null && plusInfo.PlusConfigInfo.PlusMenus.Length > 0)
        //    //    {
        //    //        foreach (var plusMenu in plusInfo.PlusConfigInfo.PlusMenus)
        //    //        {
        //    //            AddPlusMenu(plusInfo, siteAdminNavigationRepository, plusMenu, null);
        //    //        }
        //    //    }
        //    //}
        //    siteAdminNavigationRepository.SaveChanges();
        //    #endregion
        //}
        ///// <summary>
        ///// 添加菜单
        ///// </summary>
        ///// <param name="plusInfo"></param>
        ///// <param name="r"></param>
        ///// <param name="plusMenu"></param>
        ///// <param name="parentId"></param>
        //private static void AddPlusMenu(PluginDescriptor plusInfo, SiteAdminNavigationRepositoryBase<string> r, PlusMenu plusMenu, Guid? parentId)
        //{
        //    var nav = new SiteAdminNavigationBase<string>()
        //    {
        //        BadgeRequestUrl = plusMenu.BadgeRequestUrl,
        //        Href = plusMenu.Href,
        //        IconCls = plusMenu.IconCls,
        //        Id = plusMenu.Id ?? Guid.NewGuid(),
        //        IsShowBadge = plusMenu.IsShowBadge,
        //        MenuBadgeType = plusMenu.MenuBadgeType,
        //        ParentId = plusMenu.ParentId ?? parentId,
        //        Text = plusMenu.Text,
        //        TextCls = plusMenu.TextCls,
        //        Deleted = false,
        //        //TODO:设置管理员账号
        //        CreateBy = "{B0FBB2AC-3174-4E5A-B772-98CF776BD4B9}",
        //        CreateTime = DateTime.Now,
        //        PlusId = plusInfo.Id
        //    };
        //    r.Add(nav);
        //    if (plusMenu.SubMenus != null && plusMenu.SubMenus.Length > 0)
        //    {
        //        foreach (var item in plusMenu.SubMenus)
        //        {
        //            AddPlusMenu(plusInfo, r, item, nav.Id);
        //        }
        //    }
        //}


        /// <summary>
        /// 应用程序日志类
        /// </summary>
        //public override LoggerStrategyBase ApplicationLog
        //{
        //    get
        //    {
        //        return this.StrategyManager.GetDefaultStrategy<LoggerStrategyBase>();
        //    }
        //}

        public override void PreApplicationStartInitialize()
        {
            //初始化策略管理器
           // StrategyManager = new StrategyManager();
            //初始化插件管理器
            //_PluginManager = new PluginManager();
            //【优先加载框架策略】加载框架策略作为默认策略，如果插件实现了该策略，则会被覆盖
            //默认集成了日志策略
           // _PluginManager.LoadPlusStrategys(this.GetType().Assembly);
            //初始化嵌入资源管理器
            //ManifestResourceManager = new ManifestResourceManager();
            //初始化文档协议管理器
         //   DocumentsOpenProtocolManager = new DocumentsOpenProtocolManager();
           
            
            log4net.Config.XmlConfigurator.Configure(new FileInfo(HostingEnvironment.MapPath("~/config/log4net.config")));  

            Log4NetLoggerAdapter adapter = new Log4NetLoggerAdapter();
            LogManager.AddLoggerAdapter(adapter);

             //初始化配置管理器
            ConfigManager = new ConfigManager();
            //加载程序集
           // LoadAssemblies();

            //初始化Plugin
            PluginManager.Initialize();

            //初始化系统
            EngineContext.Initialize(false);

            //初始化数据库
            DatabaseInitialize();




            GlobalConfigurationManager.MapHttpAttributeRoutes();
            //初始化OData
            GlobalConfigurationManager.InitializeODATA();
            //初始化WebApi
            GlobalConfigurationManager.InitializeWebAPI();
            //初始化MVC插件引擎
            GlobalConfigurationManager.InitializeMVCEngines();


            //执行plugin的install方法
            //PluginManager.ExcuteAllInstallEvent();

        }

        private static void DatabaseInitialize()
        {
           

            // string file = HttpContext.Current.Server.MapPath("bin/DLYB.CA.Service.dll");
            // Assembly assembly = Assembly.LoadFrom(file);


           var Asss= AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("DLYB") || a.FullName.Contains("Infrastructure"));
           foreach (var ass in Asss)
           {
               DatabaseInitializer.AddMapperAssembly(ass);
           }

            foreach (var ass in PluginManager.ReferencedPlugins) 
            {
                if (ass.IsEnabled)
                {
                    DatabaseInitializer.AddMapperAssembly(ass.ReferencedAssembly);
                }
            }
            //DatabaseInitializer.Initialize(); //SQL Server初始化
            DatabaseInitializer.InitializeMySql(); //MySQL 初始化
        }
        /// <summary>
        /// 策略管理器
        /// </summary>
       // public override StrategyManagerBase StrategyManager { get; set; }
    }
}
