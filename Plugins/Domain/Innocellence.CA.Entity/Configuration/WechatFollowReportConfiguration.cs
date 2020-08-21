using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class WechatFollowReportConfiguration : EntityConfigurationBase<WechatFollowReport, int>
    {
        public WechatFollowReportConfiguration()
        {
            ToTable("WechatFollowReport");
        }
    }
}
