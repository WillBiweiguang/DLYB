using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class PageReportConfiguration : EntityConfigurationBase<PageReport, int>
    {
        public PageReportConfiguration()
        {
            ToTable("PageReport");
        }
    }
}
