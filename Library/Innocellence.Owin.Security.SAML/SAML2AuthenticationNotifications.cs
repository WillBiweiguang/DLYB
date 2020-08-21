// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Notifications;

namespace DLYB.Owin.Security.SAML
{
    /// <summary>
    /// Specifies events which the <see cref="SAMLAuthenticationMiddleware"></see> invokes to enable developer control over the authentication process. />
    /// </summary>
    public class SAML2AuthenticationNotifications
    {
        /// <summary>
        /// Creates a new set of notifications. Each notification has a default no-op behavior unless otherwise documented.
        /// </summary>
        public SAML2AuthenticationNotifications()
        {
            AuthenticationFailed = notification => Task.FromResult(0);
            MessageReceived = notification => Task.FromResult(0);
            SecurityTokenReceived = notification => Task.FromResult(0);
            SecurityTokenValidated = notification => Task.FromResult(0);
            RedirectToIdentityProvider = notification => Task.FromResult(0);
        }

        /// <summary>
        /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public Func<AuthenticationFailedNotification<SAML2Message, SAML2AuthenticationOptions>, Task> AuthenticationFailed { get; set; }

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public Func<MessageReceivedNotification<SAML2Message, SAML2AuthenticationOptions>, Task> MessageReceived { get; set; }

        /// <summary>
        /// Invoked to manipulate redirects to the identity provider for SignIn, SignOut, or Challenge.
        /// </summary>
        public Func<RedirectToIdentityProviderNotification<SAML2Message, SAML2AuthenticationOptions>, Task> RedirectToIdentityProvider { get; set; }

        /// <summary>
        /// Invoked with the security token that has been extracted from the protocol message.
        /// </summary>
        public Func<SecurityTokenReceivedNotification<SAML2Message, SAML2AuthenticationOptions>, Task> SecurityTokenReceived { get; set; }

        /// <summary>
        /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        public Func<SecurityTokenValidatedNotification<SAML2Message, SAML2AuthenticationOptions>, Task> SecurityTokenValidated { get; set; }
    }
}
