using System;
using System.Runtime.Caching;
using Infrastructure.Utility;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.Core.Caching
{
    /// <summary>
    /// Provides the default implementation for a cache manager. 
    /// The cache manager provides an abstraction over the cache holder allowing it to be easily swaped and isolating it within a component context.
    /// </summary>
    public class DefaultCacheManager : ICacheManager
    {
        private readonly Type _component;
        private readonly ICacheHolder _cacheHolder;
        private readonly ObjectCache _cache;
        private static readonly object _syncObject = new object();

        /// <summary>
        /// Constructs a new cache manager for a given component type and with a specific cache holder implementation.
        /// </summary>
        /// <param name="component">The component to which the cache applies (context).</param>
        /// <param name="cacheHolder">The cache holder that contains the entities cached.</param>
        public DefaultCacheManager(Type component, ICacheHolder cacheHolder)
        {
            _component = component;
            _cacheHolder = cacheHolder;
            _cache = MemoryCache.Default;
        }

        /// <summary>
        /// Gets a cache entry from the cache holder.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to be used to fetch the cache entry.</typeparam>
        /// <typeparam name="TResult">The type of the entry to be obtained from the cache.</typeparam>
        /// <returns>The entry from the cache.</returns>
        public ICache<TKey, TResult> GetCache<TKey, TResult>()
        {
            return _cacheHolder.GetCache<TKey, TResult>(_component);
        }

        public TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire)
        {
            return GetCache<TKey, TResult>().Get(key, acquire);
        }


        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>获取的数据</returns>
        public object Get(string key)
        {
            key.CheckNotNull("key");
            string cacheKey = key;// GetCacheKey(key);
            object value = _cache.Get(cacheKey);
            if (value == null)
            {
                return null;
            }
            DictionaryEntry entry = (DictionaryEntry)value;
            if (!key.Equals(entry.Key))
            {
                return null;
            }
            return entry.Value;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public T Get<T>(string key, Func<T> acquire)
        {
            return Get(key, 60, acquire);
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public T Get<T>(string key, int cacheTime, Func<T> acquire)
        {

            if (_cache.Contains(key))
            {
                return (T)Get(key);
            }

            lock (_syncObject)
            {
                //多线程处理
                if (_cache.Contains(key))
                {
                    return (T)Get(key);
                }

                var result = acquire();
                if (cacheTime > 0)
                    Set(key, result, new TimeSpan(0, cacheTime, 0));
                return result;
            }
        }

        public void Remove(string key)
        {
            key.CheckNotNull("key");
            string cacheKey = key;// GetCacheKey(key);
            lock (_syncObject)
            {
                _cache.Remove(cacheKey);
            }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            lock (_syncObject)
            {
                List<string> lst = new List<string>();
                _cache.Each(a => lst.Add(a.Key));
                foreach (var s in lst)
                {
                    _cache.Remove(s);
                }
            }
        }

        /// <summary>
        /// 添加或替换缓存项并设置相对过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存数据</param>
        /// <param name="slidingExpiration">修改为绝对过期时间</param>
        private void Set(string key, object value, TimeSpan slidingExpiration)
        {
            key.CheckNotNull("key");
            value.CheckNotNull("value");
            string cacheKey = key;// GetCacheKey(key);
            var entry = new DictionaryEntry(key, value);
            var policy = new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddMinutes(slidingExpiration.TotalMinutes)) };
            _cache.Set(cacheKey, entry, policy);
        }
    }
}
