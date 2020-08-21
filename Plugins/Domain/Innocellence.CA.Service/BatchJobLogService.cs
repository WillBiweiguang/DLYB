using System.Linq;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Services 
{
    public class BatchJobLogService : BaseService<BatchJobLog> , IBatchJobLogService
    {
        public BatchJobLog GetLogByJobId(string jobId)
        {
            return Repository.Entities.FirstOrDefault(x => x.JobID.Equals(jobId.Trim()));
        }
    }
}
