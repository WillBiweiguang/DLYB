using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class FeedBackConfiguration : EntityConfigurationBase<FeedBackEntity, int>
    {
        public FeedBackConfiguration()
        {
            ToTable("Feedback");
        }
    }
}
