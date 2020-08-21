using System;
using Infrastructure.Core.Logging;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;


namespace DLYB.CA.Service
{
    public class DefaultOAuthExpireRule : IVerfyOAuthRule<Result<object>>
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(DefaultOAuthExpireRule).Name);

        public Result<object> Verfy(TokenContext context)
        {
            if (context.AuthToken == null || string.Compare(context.AuthToken.Token, context.TargetToken, StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                _logger.Debug(context.AuthToken != null
                    ? string.Format("target token is {0}, db token is {1}", context.TargetToken, context.AuthToken.Token)
                    : string.Format("target token is {0}, db token is null", context.TargetToken));

                return new Result<object> { Status = (int)OAuthTokenStatus.InvalidToken, Message = OAuthTokenStatus.InvalidToken.ToString() };
            }

            if (DateTime.UtcNow >= context.AuthToken.ExpiredDateUtc)
            {
                return new Result<object> { Status = (int)OAuthTokenStatus.ExpiredToken, Message = OAuthTokenStatus.ExpiredToken.ToString() };
            }

            return new Result<object> { Status = (int)OAuthTokenStatus.Ok };
        }
    }
}
