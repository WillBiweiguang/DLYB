using System;

namespace Infrastructure.Core.Caching {
    public interface IAsyncTokenProvider {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}