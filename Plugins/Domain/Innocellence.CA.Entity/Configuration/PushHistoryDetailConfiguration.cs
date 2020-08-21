using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class PushHistoryDetailConfiguration : EntityConfigurationBase<PushHistoryDetailEntity, int>
    {
        public PushHistoryDetailConfiguration()
        {
            ToTable("PushHistoryDetail");
        }
    }
}
