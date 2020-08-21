/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB
    
    文件名：BaseContainerBag.cs
    文件功能描述：微信容器接口中的封装Value（如Ticket、AccessToken等数据集合）
    
    
    创建标识：DLYB - 20151003
    
----------------------------------------------------------------*/

using System;
using System.Runtime.CompilerServices;
using DLYB.Weixin.Annotations;
using DLYB.Weixin.Cache;
using DLYB.Weixin.Entities;
using DLYB.Weixin.Helpers;
using DLYB.Weixin.MessageQueue;
using DLYB.Weixin.CommonAPIs;

namespace DLYB.Weixin.Containers
{
    /// <summary>
    /// IBaseContainerBag
    /// </summary>
    public interface IBaseContainerBag
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// 当前对象被缓存的时间
        /// </summary>
        DateTime CacheTime { get; set; }
    }

    /// <summary>
    /// BaseContainer容器中的Value类型
    /// </summary>
    [Serializable]
    public class BaseContainerBag<T> : BindableBase, IBaseContainerBag
    {
        public BaseContainerBag()
        {

        }
        /// <summary>
        /// CorpId
        /// </summary>
        public string CorpId
        {
            get;
            set;
        }
        /// <summary>
        /// CorpSecret
        /// </summary>
        public string CorpSecret
        {
            get;
            set;
        }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime
        {
            get;
            set;
        }

        public DateTime CacheTime
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public T TokenResult
        {
            get;
            set;
        }

        /// <summary>
        /// 只针对这个AppId的锁
        /// </summary>
        public object Lock = new object();
    }
}
