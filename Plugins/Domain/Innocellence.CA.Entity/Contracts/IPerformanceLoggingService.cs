using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts
{
    public interface IPerformanceLoggingService : IDependency, IBaseService<PerformanceLogging>
    {
        void InsertWithTransaction(PerformanceLogging performance);
    }
}
