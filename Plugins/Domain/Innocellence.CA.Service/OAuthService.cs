using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using EntityFramework.Extensions;
using Infrastructure.Core.Logging;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Service.Interface;


namespace DLYB.CA.Service
{
    public class DefaultOAuthService : OAuthBaseService, IOAuthService
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(OAuthBaseService).Name);
        /// <summary>
        /// second
        /// </summary>
        protected int MaxExpireDate = 5 * 60;

        private readonly IList<IVerfyOAuthRule<Result<object>>> _clientRules = new List<IVerfyOAuthRule<Result<object>>>
        {
            new OAuthClientRule(),
            new OAuthClientDomainRule()
        };

        private readonly IList<IVerfyOAuthRule<Result<object>>> _tokenRules = new List<IVerfyOAuthRule<Result<object>>>
        {
            new OAuthClientRule(),
            new DefaultOAuthExpireRule()
        };


        public DefaultOAuthService(IOauthClientDataService oauthClientDataService, IOauthTokenDataService oauthTokenDataService)
            : base(oauthClientDataService, oauthTokenDataService)
        {

        }

        public virtual string GetToken(string clientId, string userId, string returnUrl)
        {
            Identity = clientId;
            TokenContext.TargetUrl = returnUrl;

            _logger.Debug(string.Format("clientid: {0},returnUrl: {1}", clientId, returnUrl));

            if (_clientRules.Select(clientRule => clientRule.Verfy(TokenContext)).Any(r => r.Status != (int)OAuthTokenStatus.Ok))
            {
                return string.Empty;
            }

            var entity = new OAuthTokenEntity
            {
                ClientId = clientId,
                CreatedDateUtc = DateTime.UtcNow,
                UserId = userId,
                Token = Guid.NewGuid().ToString().Replace("-", ""),
                ExpiredDateUtc = DateTime.UtcNow.AddSeconds(MaxExpireDate)
            };

            try
            {
                using (var tran = new TransactionScope())
                {
                    //clear timeout token for current user to reduce data set size
                    OauthTokenDataService.Repository.Entities.Where(x => x.ClientId == clientId && x.UserId == userId).Delete();
                    OauthTokenDataService.Repository.Insert(entity);

                    tran.Complete();
                }
            }
            catch (Exception e)
            {
                _logger.Error<string>(e, e.InnerException.Message);
            }

            return entity.Id > 0 ? entity.Token : string.Empty;
        }

        //TODO:token should be used only once and as soon as possible, overdue
        public virtual Result<object> GetUserId(string clientId, string token)
        {
            Identity = clientId;
            TokenContext.TargetToken = token;

            if (TokenContext.AuthToken.IsUsed)
            {
                return new Result<object> { Status = (int)OAuthTokenStatus.Reused, Message = OAuthTokenStatus.Reused.ToString() };
            }

            foreach (var r in _tokenRules.Select(tokenRule => tokenRule.Verfy(TokenContext)).Where(r => r.Status != (int)OAuthTokenStatus.Ok))
            {
                return r;
            }

            var returnNumber = OauthTokenDataService.Repository.Entities.Where(x => x.Id == TokenContext.AuthToken.Id && x.IsUsed == false).Update(x => new OAuthTokenEntity { IsUsed = true });

            return returnNumber <= 0 ? new Result<object> { Status = (int)OAuthTokenStatus.Reused, Message = OAuthTokenStatus.Reused.ToString() } : new Result<object> { Status = (int)OAuthTokenStatus.Ok, Data = TokenContext.AuthToken.UserId };
        }

        //public string GetToken(string clientId, string userId, string returnUrl, int expiredTime)
        //{
        //    if (expiredTime > 0)
        //    {
        //        MaxExpireDate = expiredTime * 60;
        //    }

        //    return GetToken(clientId, userId, returnUrl);
        //}

        public string GenerateToken(string clientId, string userId)
        {
            Identity = clientId;
            if (TokenContext.OAuthClient == null)
            {
                return string.Empty;
            }

            var targetUrl = TokenContext.OAuthClient.ClientCallBackUrl;

            return GetToken(clientId, userId, targetUrl);
        }
    }
}
