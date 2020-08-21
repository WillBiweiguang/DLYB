/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：DLYBMessageQueueItem.cs
    文件功能描述：DLYBMessageQueue消息列队项
    
    
    创建标识：DLYB - 20151226
    
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.Weixin.MessageQueue
{
    /// <summary>
    /// DLYBMessageQueue消息列队项
    /// </summary>
    public class DLYBMessageQueueItem
    {
        /// <summary>
        /// 列队项唯一标识
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 列队项目命中触发时执行的委托
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// 此实例对象的创建时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 项目说明（主要用于调试）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 初始化DLYBMessageQueue消息列队项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        public DLYBMessageQueueItem(string key, Action action, string description = null)
        {
            Key = key;
            Action = action;
            Description = description;
            AddTime = DateTime.Now;
        }
    }
}
