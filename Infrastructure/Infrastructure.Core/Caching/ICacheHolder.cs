using System;

namespace Infrastructure.Core.Caching {
    public interface ICacheHolder : ISingletonDependency {
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}
