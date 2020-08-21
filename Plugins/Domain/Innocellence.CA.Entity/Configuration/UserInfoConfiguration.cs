using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.Configuration
{
    public class UserInfoConfiguration : EntityConfigurationBase<UserInfo, int>
    {
        public UserInfoConfiguration()
        {
            ToTable("UserInfo");
        }
    }
}
