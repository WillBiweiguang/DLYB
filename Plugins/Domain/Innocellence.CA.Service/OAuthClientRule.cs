using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;

namespace DLYB.CA.Service
{
    public class OAuthClientRule : IVerfyOAuthRule<Result<object>>
    {
        public Result<object> Verfy(TokenContext context)
        {
            if (context.OAuthClient == null || context.OAuthClient.Status == OAuthClientStatus.Disabled.ToString())
            {
                return new Result<object> { Status = (int)OAuthTokenStatus.InvalidClientId, Message = OAuthTokenStatus.InvalidClientId.ToString() };
            }
            
            return new Result<object> { Status = (int)OAuthTokenStatus.Ok };
        }
    }
}
