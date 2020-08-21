using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class BiddingQueryConfiguration : EntityConfigurationBase<BiddingQuery, int>
    {
        public BiddingQueryConfiguration()
        {
            ToTable("BiddingQuery");
        }
    }
}
