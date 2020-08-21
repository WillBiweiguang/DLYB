using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class BatchJobLogConfiguration : EntityConfigurationBase<BatchJobLog, int>
    {
        public BatchJobLogConfiguration()
        {
            ToTable("BatchJobLog");
        }
    }
}
