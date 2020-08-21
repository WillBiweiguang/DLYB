using System;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;


namespace DLYB.CA.Service
{
    public class OAuthClientDomainRule : IVerfyOAuthRule<Result<object>>
    {
        public Result<object> Verfy(TokenContext context)
        {
            context.ClientDomain = new Uri(context.TargetUrl).Host;

            var configedDomain = new Uri(context.OAuthClient.ClientCallBackUrl).Host;

            if (string.Compare(configedDomain, context.ClientDomain, StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                return new Result<object> { Status = (int)OAuthTokenStatus.InvalidClientDomain, Message = OAuthTokenStatus.InvalidClientDomain.ToString() };
            }

            return new Result<object> { Status = (int)OAuthTokenStatus.Ok };
        }
    }
}
