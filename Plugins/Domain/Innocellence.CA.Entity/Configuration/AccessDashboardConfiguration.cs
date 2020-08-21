using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class AccessDashboardConfiguration : EntityConfigurationBase<AccessDashboard, int>
    {
        public AccessDashboardConfiguration()
        {
            ToTable("DrugReimbursement");
        }
    }
}
