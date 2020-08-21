//using Infrastructure.Core.Strategy.User;
using Infrastructure.Core.Themes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Infrastructure.Core
{
    /// <summary>
    /// Web上下文对象
    /// </summary>
    public abstract class WorkContextBase : IWorkContext
    {

        public abstract T Resolve<T>();

        /// <summary>
        /// Tries to resolve a registered dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <param name="service">An instance of the dependency if it could be resolved</param>
        /// <returns>True if the dependency could be resolved, false otherwise</returns>
        public abstract bool TryResolve<T>(out T service);



        public RequestContext ReqeustContext { get; set; }
        /// <summary>
        /// 获取当前请求附带的路由参数的集合
        /// </summary>
        public RouteValueDictionary RouteValues
        {
            get
            {
                if (ReqeustContext != null)
                    return ReqeustContext.RouteData.Values;
                return null;
            }
        }
        /// <summary>
        /// 获取指定名称的路由地址参数的值
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns></returns>
        public object GetRouteValue(string key)
        {
            object resultValue = null;
            if (RouteValues != null && RouteValues.Count > 0)
                RouteValues.TryGetValue(key, out resultValue);
            return resultValue;
        }

        /// <summary>
        /// 当前Http上下文对象
        /// </summary>
        public HttpContext HttpContext { get { return HttpContext.Current; } }


        public virtual CultureInfo WorkingLanguage { get; set; }


      public  TimeZoneInfo CurrentTimeZone { get; set; }

        /// <summary>
        /// The Layout shape corresponding to the work context
        /// </summary>
        public dynamic Layout
        {
            get;
            set;
        }

        ///// <summary>
        ///// Settings of the site corresponding to the work context
        ///// </summary>
        //public ISite CurrentSite
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// The user, if there is any corresponding to the work context
        /// </summary>
        public object CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// The theme used in the work context
        /// </summary>
        public ITheme CurrentTheme
        {
            get;
            set;
        }

        /// <summary>
        /// Active culture of the work context
        /// </summary>
        public string CurrentCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Active calendar of the work context
        /// </summary>
        public string CurrentCalendar
        {
            get;
            set;
        }

        /// <summary>
        /// Time zone of the work context
        /// </summary>
        //public TimeZoneInfo CurrentTimeZone
        //{
        //    get;
        //    set;
        //}

        public bool IsAdmin { get; set; }

        /// <summary>
        /// 当前应用程序上下文对象
        /// </summary>
        public ApplicationContextBase ApplicationContext { get { return GlobalApplicationObject.Current.ApplicationContext; } }
    }
}
