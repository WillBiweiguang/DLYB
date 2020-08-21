using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class ArticlePublishInfoConfiguration : EntityConfigurationBase<ArticlePublishInfo, int>
    {
        public ArticlePublishInfoConfiguration()
        {
            ToTable("ArticlePublish");
        }
    }
}
