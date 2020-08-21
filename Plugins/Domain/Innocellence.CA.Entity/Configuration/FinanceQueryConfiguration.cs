using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class FinanceQueryConfiguration : EntityConfigurationBase<FinanceQueryEntity, string>
    {
        public FinanceQueryConfiguration()
        {
            ToTable("FinanceQuery");

            Property(x => x.MoneySum).HasPrecision(18, 4);
        }
    }
}
