using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class ArticleThumbsUpConfiguration : EntityConfigurationBase<ArticleThumbsUp, int>
    {
        public ArticleThumbsUpConfiguration()
        {
            ToTable("ArticleThumbsUp");
        }
    }
}
