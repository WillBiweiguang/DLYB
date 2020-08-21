using Infrastructure.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

//======================================================================
//
//        Copyright (C) DLYB
//        All rights reserved
//
//        filename :PluginRazorViewEngine
//        description : 这种方式效率稍低，如果是高并发环境，最好改造一下。
//                      可以参考 base.FindView的源码，直接重写GetPath方法。或者参考nop的theme实现。
//
//        created by heying at  2015/09/21 
//        
//
//======================================================================
namespace Infrastructure.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginRazorViewEngine : RazorViewEngine
    {


        public PluginRazorViewEngine()
            : this((IViewPageActivator)null)
        {
            
        }
        public PluginRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            this.FileExtensions = new[]
              {
                "cshtml"
              };
        }
        /// <summary>
        /// 定义区域视图页所在地址。
        /// </summary>
        private string[] _areaViewLocationFormats = new[]
        {
           // "~/Views/Parts/{0}.cshtml",
           // "~/Plugins/{2}/Views/{1}/{0}.cshtml",
           // "~/Plugins/{2}/Views/Shared/{0}.cshtml",
           "~/Plugins/{pluginName}/Areas/{2}/Views/{1}/{0}.cshtml",
           "~/Plugins/{pluginName}/Areas/{2}/Views/Shared/{0}.cshtml",

         //   "~/{2}/Views/{1}/{0}.cshtml",
          //  "~/{2}/Views/Shared/{0}.cshtml",
            "~/Areas/{2}/Views/{1}/{0}.cshtml",
            "~/Areas/{2}/Views/Shared/{0}.cshtml",
            
        };

        /// <summary>
        /// 定义视图页所在地址。
        /// </summary>
        private string[] _viewLocationFormats = new[]
        {
           // "~/Views/Parts/{0}.cshtml",
            "~/Plugins/{pluginName}/Views/{1}/{0}.cshtml",
            "~/Plugins/{pluginName}/Views/Shared/{0}.cshtml",
           // "~/{pluginName}/Views/{1}/{0}.cshtml",
           // "~/{pluginName}/Views/Shared/{0}.cshtml",
            "~/Views/{1}/{0}.cshtml",
            "~/Views/Shared/{0}.cshtml",
        };

        //private string[] _MasterLocationFormats = new[]
        //    {
        //          "~/Plugins/{pluginName}/Views/{1}/{0}.cshtml",
        //    "~/Plugins/{pluginName}/Views/Shared/{0}.cshtml",
        //   // "~/{pluginName}/Views/{1}/{0}.cshtml",
        //   // "~/{pluginName}/Views/Shared/{0}.cshtml",
        //    "~/Views/{1}/{0}.cshtml",
        //    "~/Views/Shared/{0}.cshtml",
        //    };
        //private string[] _PartialViewLocationFormats = new[]
        //    {
        //          "~/Plugins/{pluginName}/Views/{1}/{0}.cshtml",
        //    "~/Plugins/{pluginName}/Views/Shared/{0}.cshtml",
        //   // "~/{pluginName}/Views/{1}/{0}.cshtml",
        //   // "~/{pluginName}/Views/Shared/{0}.cshtml",
        //    "~/Views/{1}/{0}.cshtml",
        //    "~/Views/Shared/{0}.cshtml"
        //    };


      private string[] __AreaViewLocationFormats=  new[]
              {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
              };

             private string[] __ViewLocationFormats = new[]
              {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
              };



        /// <summary>
        /// 搜索部分视图页。
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="partialViewName"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            string pluginName = string.Empty;
            //if (controllerContext.HttpContext != null)
            //    pluginName = controllerContext.HttpContext.Request.QueryString["pluginName"];
            //if (string.IsNullOrWhiteSpace(pluginName) && controllerContext.RouteData.Values.ContainsKey("pluginName"))
            //{
            //    pluginName = controllerContext.RouteData.GetRequiredString("pluginName");
            //}


            if (controllerContext.HttpContext != null)
            {
                //pluginName = controllerContext.HttpContext.Request.QueryString["pluginName"];
                var objPlugin = PluginManager.ReferencedPlugins.FirstOrDefault(a => a.Controllers.Exists(
                    b => controllerContext.RouteData.DataTokens.Values.Count(c => ((string[])c).Count(d => d == b) > 0) > 0));
                if (objPlugin != null)
                {
                    pluginName = objPlugin.SystemName;
                }
            }
            if (!string.IsNullOrWhiteSpace(pluginName))
            {
                this.AreaViewLocationFormats = this._areaViewLocationFormats;
                this.AreaMasterLocationFormats = this._areaViewLocationFormats;
                this.AreaPartialViewLocationFormats = this._areaViewLocationFormats;

                this.ViewLocationFormats = this._viewLocationFormats;
                this.MasterLocationFormats = this.ViewLocationFormats;
                this.PartialViewLocationFormats = this.ViewLocationFormats;
                this.UpdatePath(pluginName);
            }
            else
            {
                SetPath();
            }
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string pluginName = string.Empty;
            if (controllerContext.HttpContext != null)
            {
                //pluginName = controllerContext.HttpContext.Request.QueryString["pluginName"];
                var objPlugin = PluginManager.ReferencedPlugins.FirstOrDefault(a => a.Controllers.Exists(
                    b => controllerContext.RouteData.DataTokens.Values.Count(c => c is string[] && ((string[])c).Count(d => d == b) > 0) > 0));
                if (objPlugin != null)
                {
                    pluginName = objPlugin.SystemName;
                }
            }


            //if (string.IsNullOrWhiteSpace(pluginName) && controllerContext.RouteData.Values.ContainsKey("pluginName"))
            //{
            //    pluginName = controllerContext.RouteData.GetRequiredString("pluginName");
            //}
            if (!string.IsNullOrWhiteSpace(pluginName))
            {
                this.AreaViewLocationFormats = this._areaViewLocationFormats;
                this.AreaMasterLocationFormats = this._areaViewLocationFormats;
                this.AreaPartialViewLocationFormats = this._areaViewLocationFormats;

                this.ViewLocationFormats = this._viewLocationFormats;
                this.MasterLocationFormats = this.ViewLocationFormats;
                this.PartialViewLocationFormats = this.ViewLocationFormats;
                this.UpdatePath(pluginName);
            }
            else
            {
                SetPath();
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        private void SetPath()
        {
            this.AreaViewLocationFormats =__AreaViewLocationFormats;
            this.AreaMasterLocationFormats = __AreaViewLocationFormats;
            this.AreaPartialViewLocationFormats = __AreaViewLocationFormats;
            this.ViewLocationFormats = __ViewLocationFormats;
            this.MasterLocationFormats = __ViewLocationFormats;
            this.PartialViewLocationFormats = __ViewLocationFormats;
           

        }

        /// <summary>
        /// 更新路径中的插件名称参数。
        /// </summary>
        /// <param name="moduleName"></param>
        private void UpdatePath(string moduleName)
        {
            string[] viewLocationFormats = new string[this._viewLocationFormats.Length];
            string[] areaViewLocationFormats = new string[this._areaViewLocationFormats.Length];

            if (areaViewLocationFormats != null)
            {
                for (int index = 0; index < areaViewLocationFormats.Length; index++)
                {
                    areaViewLocationFormats[index] = this._areaViewLocationFormats[index].Replace("{pluginName}", moduleName);
                }

                this.AreaViewLocationFormats = areaViewLocationFormats;
                this.AreaMasterLocationFormats = areaViewLocationFormats;
                this.AreaPartialViewLocationFormats = areaViewLocationFormats;
            }

            if (viewLocationFormats != null)
            {
                for (int index = 0; index < viewLocationFormats.Length; index++)
                {
                    viewLocationFormats[index] = this._viewLocationFormats[index].Replace("{pluginName}", moduleName);
                }

                this.ViewLocationFormats = viewLocationFormats;
                this.MasterLocationFormats = viewLocationFormats;
                this.PartialViewLocationFormats = viewLocationFormats;
            }
        }

        ///// <summary>
        ///// 给运行时编译的页面加了引用程序集。
        ///// </summary>
        ///// <param name="pluginName"></param>
        //private void CodeGeneration(string pluginName)
        //{
        //    RazorBuildProvider.CodeGenerationStarted += (object sender, EventArgs e) =>
        //    {
        //        RazorBuildProvider provider = (RazorBuildProvider)sender;

        //        var plugin = PluginManager.GetPlugin(pluginName);

        //        if (plugin != null)
        //        {
        //            provider.AssemblyBuilder.AddAssemblyReference(plugin.Assembly);
        //        }
        //    };
        //}
    }
}
