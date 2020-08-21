// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Microsoft.IdentityModel.Extensions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace DLYB.Owin.Security.SAML
{
    /// <summary>
    /// OWIN middleware for obtaining identities using SAML protocol.
    /// </summary>
    public class SAML2AuthenticationMiddleware : AuthenticationMiddleware<SAML2AuthenticationOptions>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a <see cref="SAMLAuthenticationMiddleware"/>
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="app">The OWIN application</param>
        /// <param name="options">Configuration options for the middleware</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "A reference is maintained.")]
        public SAML2AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, SAML2AuthenticationOptions options)
            : base(next, options)
        {
            _logger = app.CreateLogger<SAML2AuthenticationMiddleware>();

            if (string.IsNullOrWhiteSpace(Options.TokenValidationParameters.AuthenticationType))
            {
                Options.TokenValidationParameters.AuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }

            if (Options.StateDataFormat == null)
            {
                var dataProtector = app.CreateDataProtector(
                    typeof(SAML2AuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            if (Options.SecurityTokenHandlers == null)
            {
                Options.SecurityTokenHandlers = SecurityTokenHandlerCollectionExtensions.GetDefaultHandlers();
            }

            if (Options.Notifications == null)
            {
                Options.Notifications = new SAML2AuthenticationNotifications();
            }

            Uri wreply;
            if (!Options.CallbackPath.Any() && !string.IsNullOrEmpty(Options.Wreply) && Uri.TryCreate(Options.Wreply, UriKind.Absolute, out wreply))
            {
                // Wreply must be a very specific, case sensitive value, so we can't generate it. Instead we generate CallbackPath from it.
                Options.CallbackPath.Add(PathString.FromUriComponent(wreply));
            }

            if (Options.ConfigurationManager == null)
            {
                if (Options.Configuration != null)
                {
                    Options.ConfigurationManager = new StaticConfigurationManager<WsFederationConfiguration>(Options.Configuration);
                }
                else
                {
                    HttpClient httpClient = new HttpClient(ResolveHttpMessageHandler(Options));
                    httpClient.Timeout = Options.BackchannelTimeout;
                    httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
                    Options.ConfigurationManager = new ConfigurationManager<WsFederationConfiguration>(Options.MetadataAddress, httpClient);
                }
            }
        }

        /// <summary>
        /// Provides the <see cref="AuthenticationHandler"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler"/> configured with the <see cref="SAMLAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<SAML2AuthenticationOptions> CreateHandler()
        {
            return new SAML2AuthenticationHandler(_logger);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Managed by caller")]
        private static HttpMessageHandler ResolveHttpMessageHandler(SAML2AuthenticationOptions options)
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