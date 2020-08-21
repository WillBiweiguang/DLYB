using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Entity;


namespace DLYB.CA.Contracts.Configuration
{
    public class OAuthClientsConfiguration : EntityConfigurationBase<OAuthClientsEntity, int>
    {
        public OAuthClientsConfiguration()
        {
            ToTable("OAuthClients");
        } 
    }

    public class OAuthTokenEntityConfiguration : EntityConfigurationBase<OAuthTokenEntity, int>
    {
        public OAuthTokenEntityConfiguration()
        {
            ToTable("OAuthToken");
        }
    }
}
