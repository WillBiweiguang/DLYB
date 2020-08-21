using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IBatchJobLogService : IDependency, IBaseService<BatchJobLog>
    {
        BatchJobLog GetLogByJobId(string jobId);
    }
}
