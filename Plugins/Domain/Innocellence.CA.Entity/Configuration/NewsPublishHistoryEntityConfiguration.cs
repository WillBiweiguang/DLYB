using System.Data.Entity.ModelConfiguration;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class NewsPublishHistoryEntityConfiguration:EntityTypeConfiguration<NewsPublishHistoryEntity>
    {
        public NewsPublishHistoryEntityConfiguration()
        {
            ToTable("NewsPublishHistory");
        }

    }
}
