using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.Owin.Security.OAuthClient
{
    public static class OAuthClientAuthenticationExtensions
    {
        public static IAppBuilder UseOAuthClientAuthentication(this IAppBuilder app, OAuthClientAuthenticationOptions options)
        { 
            if (app == null) throw new ArgumentNullException("app");
            if (options == null) throw new ArgumentNullException("options");

            app.Use(typeof(OAuthClientAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseOAuthClientAuthentication(this IAppBuilder app, string appId, string appSecret)
        {
            return app.UseOAuthClientAuthentication(new OAuthClientAuthenticationOptions
            {
                AppId = appId,
                AppSecret = appSecret
            });
        }
    }
}
