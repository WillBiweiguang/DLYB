using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class ArticleReportConfiguration : EntityConfigurationBase<ArticleReport, int>
    {
        public ArticleReportConfiguration()
        {
            ToTable("ArticleReport");
        }
    }
}
