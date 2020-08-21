// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace DLYB.Owin.Security.SAML
{
    /// <summary>
    /// Default values related to SAML authentication middleware
    /// </summary>
    public static class SAML2AuthenticationDefaults
    {
        /// <summary>
        /// The default value used for SAMLAuthenticationOptions.AuthenticationType
        /// </summary>
        public const string AuthenticationType = "SAML2";

        /// <summary>
        /// The prefix used to provide a default SAMLAuthenticationOptions.CookieName
        /// </summary>
        public const string CookiePrefix = "SAML2.";

        /// <summary>
        /// The prefix used to provide a default SAMLAuthenticationOptions.CookieName
        /// </summary>
        public const string CookieName = "SAML2Auth";

        /// <summary>
        /// The default value for SAMLAuthenticationOptions.Caption.
        /// </summary>
        public const string Caption = "SAML2";

        internal const string WctxKey = "SAML2OwinState";
    }
}
