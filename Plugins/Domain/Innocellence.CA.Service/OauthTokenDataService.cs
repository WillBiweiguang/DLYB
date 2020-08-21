using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;


namespace DLYB.CA.Service
{
    public class OauthTokenDataService : BaseService<OAuthTokenEntity>, IOauthTokenDataService
    {
    }

    public class OauthClientDataService : BaseService<OAuthClientsEntity>, IOauthClientDataService
    {
    }
}
