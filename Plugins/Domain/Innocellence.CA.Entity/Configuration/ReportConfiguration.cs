using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class ReportConfiguration : EntityConfigurationBase<MenuReportEntity, int>
    {
        public ReportConfiguration()
        {
            ToTable("MenuReport");
        }
    }
    public class AppReportConfiguration : EntityConfigurationBase<AppAccessReport, int>
    {
        public AppReportConfiguration()
        {
            ToTable("AppAccessReport");
        }
    }
}
