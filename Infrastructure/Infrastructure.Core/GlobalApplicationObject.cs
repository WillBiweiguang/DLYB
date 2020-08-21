using Infrastructure.Core.Events;
using Infrastructure.Core.Performance.WatchPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core
{
    /// <summary>
    /// 全局应用程序对象
    /// </summary>
    public class GlobalApplicationObject
    {
        #region 当前全局上下文对象
        private static readonly Lazy<GlobalApplicationObject> LazyContext = new Lazy<GlobalApplicationObject>(() => new GlobalApplicationObject());
        /// <summary>
        /// 当前全局上下文对象
        /// </summary>
        public static GlobalApplicationObject Current { get { return LazyContext.Value; } }
        #endregion
        #region 应用程序上下文对象
        /// <summary>
        /// 应用程序上下文对象
        /// </summary>
        public ApplicationContextBase ApplicationContext { get; set; }
        #endregion

        private string connectionStringName;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionStringName
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringName))
                {
                    connectionStringName = System.Configuration.ConfigurationManager.AppSettings["DLYB.NET_ConnectionStringName"];
                    if (string.IsNullOrEmpty(connectionStringName))
                        connectionStringName = "DLYB.NET";
                }
                return connectionStringName;
            }
            set { connectionStringName = value; }
        }

        #region 事件管理器
        readonly Lazy<EventsManager> eventsManager = new Lazy<EventsManager>(() => new EventsManager());
        /// <summary>
        /// 事件管理器
        /// </summary>
        public EventsManager EventsManager { get { return eventsManager.Value; } }
        #endregion

        #region 监控面板
        readonly Lazy<WatchPanel> watchPanel = new Lazy<WatchPanel>(() => new WatchPanel());
        /// <summary>
        /// 监控面板
        /// </summary>
        public WatchPanel WatchPanel { get { return watchPanel.Value; } }
        #endregion
    }
}
