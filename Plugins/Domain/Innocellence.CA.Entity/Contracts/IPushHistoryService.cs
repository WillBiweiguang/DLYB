using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IPushHistoryService : IDependency, IBaseService<PushHistoryEntity>
    {
    }
}
