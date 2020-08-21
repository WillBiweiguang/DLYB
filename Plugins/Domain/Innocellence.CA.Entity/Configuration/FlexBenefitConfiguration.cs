using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class FlexBenefitConfiguration : EntityConfigurationBase<FlexBenefit, int>
    {
        public FlexBenefitConfiguration()
        {
            ToTable("FlexBenefit");
        }
    }
}
