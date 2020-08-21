using System;

namespace Infrastructure.Core.Caching {
    public interface ICacheManager {
        TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
        ICache<TKey, TResult> GetCache<TKey, TResult>();

        T Get<T>(string key, int cacheTime, Func<T> acquire);

        T Get<T>(string key, Func<T> acquire);

        void Remove(string key);

        void Clear();
    }
}
