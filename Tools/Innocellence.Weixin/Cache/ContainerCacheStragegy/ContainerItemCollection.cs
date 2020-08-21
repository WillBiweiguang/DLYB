using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLYB.Weixin.Containers;
using DLYB.Weixin.CommonAPIs;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.Cache
{
    /// <summary>
    /// IContainerItemCollection，对某个Container下的缓存值ContainerBag进行封装
    /// </summary>
    public interface IContainerItemCollection : IBaseCacheStrategy<string, AccessTokenBag>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">缓存键（通常为AppId，值和AccessTokenBag.Key相等）</param>
        /// <returns></returns>
        AccessTokenBag this[string key] { get; set; }
    }

    /// <summary>
    /// 储存某个Container下所有ContainerBag的字典集合
    /// </summary>
    [Serializable]
    public class ContainerItemCollection : IContainerItemCollection
    {
        private Dictionary<string, AccessTokenBag> _cache;//TODO:可以考虑升级到统一的式缓存策略中

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">缓存键（通常为AppId，值和AccessTokenBag.Key相等）</param>
        /// <returns></returns>
        public AccessTokenBag this[string key]
        {
            get { return this.Get(key); }
            set { this.Update(key, value); }
        }

        public ContainerItemCollection()
        {
            _cache = new Dictionary<string, AccessTokenBag>(StringComparer.OrdinalIgnoreCase);
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string CacheSetKey { get; set; }

        #region 实现IContainerItemCollection : IBaseCacheStrategy<string, AccessTokenBag>接口

        public void InsertToCache(string key, AccessTokenBag value)
        {
            _cache[key] = value;
        }

        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }

        public AccessTokenBag Get(string key)
        {
            if (this.CheckExisted(key))
            {
                return _cache[key];
            }
            return null;
        }

        public IDictionary<string, AccessTokenBag> GetAll()
        {
            return _cache;
        }

        public bool CheckExisted(string key)
        {
            return _cache.ContainsKey(key);
        }

        public long GetCount()
        {
            return _cache.Count;
        }

        public void Update(string key, AccessTokenBag value)
        {
            _cache[key] = value;
        }

        #endregion
    }
}
