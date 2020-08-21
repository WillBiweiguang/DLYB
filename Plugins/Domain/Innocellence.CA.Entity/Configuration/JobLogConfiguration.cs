using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class JobLogConfiguration : EntityConfigurationBase<ReportJobLogEntity, string>
    {
        public JobLogConfiguration()
        {
            ToTable("ReportJobLog");
        }
    }
}
