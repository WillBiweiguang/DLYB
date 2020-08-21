using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;


namespace DLYB.CA.Contracts.Contracts
{
    public interface IOauthTokenDataService : IBaseService<OAuthTokenEntity>, IDependency
    {
    }

    public interface IOauthClientDataService : IBaseService<OAuthClientsEntity>, IDependency
    {
    }
}
