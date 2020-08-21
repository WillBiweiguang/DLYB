using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using DLYB.Owin.Security.OAuthClient.Provider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Owin;

namespace DLYB.Owin.Security.OAuthClient
{
    public class OAuthClientAuthenticationMiddleware : AuthenticationMiddleware<OAuthClientAuthenticationOptions>
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;


        public OAuthClientAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, OAuthClientAuthenticationOptions options)
			: base(next, options)
        {
            if (String.IsNullOrWhiteSpace(Options.AppId))
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					Resources.Exception_OptionMustBeProvided, "ClientId"));
            if (String.IsNullOrWhiteSpace(Options.AppSecret))
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                    Resources.Exception_OptionMustBeProvided, "ClientSecret"));

            logger = app.CreateLogger<OAuthClientAuthenticationMiddleware>();

            if (Options.Provider == null)
                Options.Provider = new OAuthClientAuthenticationProvider();

            if (Options.StateDataFormat == null)
            {
                IDataProtector dataProtector = app.CreateDataProtector(
                    typeof (OAuthClientAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v2");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            if (String.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();

            httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024*1024*10,
            };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OAuthClient middleware");
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        /// <summary>
        ///     Provides the <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> object for processing
        ///     authentication-related requests.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> configured with the
        ///     <see cref="T:DLYB.Owin.Security.OAuthClient.OAuthClientAuthenticationOptions" /> supplied to the constructor.
        /// </returns>
        protected override AuthenticationHandler<OAuthClientAuthenticationOptions> CreateHandler()
        {
            return new OAuthClientAuthenticationHandler(httpClient, logger);
        }

        private HttpMessageHandler ResolveHttpMessageHandler(OAuthClientAuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator != null)
            {
                // Set the cert validate callback
                var webRequestHandler = handler as WebRequestHandler;
                if (webRequestHandler == null)
                {
                    throw new InvalidOperationException(Resources.Exception_ValidatorHandlerMismatch);
                }
                webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            }

            return handler;
        }
    }
}
