using System.Linq;
using System.Net;
using System.Web;
using EntityFramework.Extensions;
using Infrastructure.Core;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;


namespace DLYB.CA.Service.Interface
{
    public interface IOAuthService : IDependency
    {
        string GetToken(string clientId, string userId, string returnUrl);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="clientId"></param>
        ///// <param name="userId"></param>
        ///// <param name="returnUrl"></param>
        ///// <param name="expiredTime">minites</param>
        ///// <returns></returns>
        //string GetToken(string clientId, string userId, string returnUrl, int expiredTime);

        Result<object> GetUserId(string clientId, string token);

        TokenContext TokenContext { get; }

        string GenerateToken(string clientId, string userId);
    }

    public interface IVerfyOAuthRule<out T> : IDependency
    {
        T Verfy(TokenContext context);
    }

    public class TokenContext
    {
        public OAuthClientsEntity OAuthClient { get; set; }

        public OAuthTokenEntity AuthToken { get; set; }

        public string TargetUrl { get; set; }

        public string TargetToken { get; set; }

        public string ClientDomain { get; set; }
    }

    public enum OAuthClientStatus
    {
        Enabled,
        Disabled
    }

    public enum OAuthTokenStatus
    {
        InvalidClientId = 100,

        InvalidToken = 101,

        ExpiredToken = 102,

        InvalidClientDomain=103,

        Reused=104,

        Ok = 200,
    }

    public abstract class OAuthBaseService
    {
        protected IOauthClientDataService ClientDataService;
        protected IOauthTokenDataService OauthTokenDataService;
        protected string Identity;
        private TokenContext _context;

        protected OAuthBaseService(IOauthClientDataService clientDataService, IOauthTokenDataService oauthTokenDataService)
        {
            ClientDataService = clientDataService;
            OauthTokenDataService = oauthTokenDataService;
        }

        public virtual TokenContext GetTokenContext()
        {
            _context = new TokenContext();


            if (string.IsNullOrEmpty(Identity))
            {
                return _context;
            }

            //var client = ClientDataService.Repository.Entities.Where(x => x.ClientId == Identity).Select(x => new OAuthClientsEntity { ClientCallBackUrl = x.ClientCallBackUrl }).Include(x => x.Tokens.Select(y => new OAuthTokenEntity { Token = y.Token, UserId = y.UserId }).OrderByDescending(y => y.CreatedDateUtc).FirstOrDefault()).FirstOrDefault();
            var clientQuery = ClientDataService.Repository.Entities.Where(x => x.ClientId == Identity).FutureFirstOrDefault();
            var tokenQuery =
                OauthTokenDataService.Repository.Entities.Where(x => x.ClientId == Identity).OrderByDescending(x => x.Id).FutureFirstOrDefault();

            var client = clientQuery.Value;
            var token = tokenQuery.Value;

            _context.OAuthClient = client;
            _context.AuthToken = token;

            return _context;
        }

        public TokenContext TokenContext
        {
            get
            {
                return _context ?? GetTokenContext();
            }
        }
        
    }

    public static class HttpRequestBaseExtension
    {
        public static string GetDomain(this HttpRequestBase httpRequest)
        {
            if (httpRequest == null || httpRequest.UserHostAddress == null)
            {
                return string.Empty;
            }

            var ip = IPAddress.Parse(httpRequest.UserHostAddress);
            var host = Dns.GetHostEntry(ip);

            return host.HostName;
        }
    }
}
