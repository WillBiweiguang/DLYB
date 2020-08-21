using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class PushHistoryConfiguration : EntityConfigurationBase<PushHistoryEntity, int>
    {
        public PushHistoryConfiguration()
        {
            ToTable("PushHistory");
        }
    }
}
