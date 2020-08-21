using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class Finance3rdQueryConfiguration : EntityConfigurationBase<Finance3rdQueryEntity, string>
    {
        public Finance3rdQueryConfiguration()
        {
            ToTable("Finance3rdQuery");
        }
    }
}
