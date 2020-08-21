using System.Web;
using Infrastructure.Web.MVC;
using Microsoft.Owin;
using Owin;
using DLYB.Owin.Security.SAML;
using Microsoft.IdentityModel.Protocols;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Metadata;
using System.Web.Configuration;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Core.Logging;

namespace DLYB.Authentication
{
    public partial class OWinAuthorizeProvider : IOWinAuthorizeProvider
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/app_data/logs/a.txt"),CommonService.GetSysConfig("certcode", ""));
            //Lilly SAML2
            app.UseSAML2Authentication(new SAML2AuthenticationOptions()
            {
                //remove to web.config

                CallbackPath = new List<PathString> { new PathString("/sso/ssoresult"),new PathString("/sso/oauth/oauthtoken") },
                LoginBackUrl = "/OWinLogin/SsoResult",
                //  CertFilePath =HttpContext.Current.Server.MapPath("~/cert.crt"),
                SigningKeys = new List<X509Certificate>() { new X509Certificate2(HttpContext.Current.Server.MapPath("~/cert.crt")) },
                AllowUnsolicitedAuthnResponse = true,
                //EntityId = new EntityId("lly-qa:saml2:idp"),
                EntityId = new EntityId(CommonService.GetSysConfig("certcode", "")),
                Configuration = new WsFederationConfiguration()
                {
                    Issuer = WebConfigurationManager.AppSettings["SSOUrl"],
                    // SigningKeys= "",
                    TokenEndpoint = WebConfigurationManager.AppSettings["SSOUrl"]

                }
            });





            ////丁香园OAuth2
            //app.UseOAuthClientAuthentication(
            //  new OAuthClientAuthenticationOptions
            //  {
            //      AppId = "228714892725",
            //      AppSecret = "f5947310789ab6c91f3b8fa8642deb00",
            //      Endpoints = new OAuthClientAuthenticationOptions.OAuthClientAuthenticationEndpoints()
            //      {
            //          AuthorizationEndpoint = "https://auth.dxy.cn/conn/oauth2/authorize",
            //          TokenEndpoint = "https://auth.dxy.cn/conn/oauth2/accessToken",
            //          UserInfoEndpoint = "https://auth.dxy.cn/conn/oauth2/profile"
            //      }
            //  });
        }
    }
}
