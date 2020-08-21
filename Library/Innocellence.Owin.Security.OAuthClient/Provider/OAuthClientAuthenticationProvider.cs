using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.Owin.Security.OAuthClient.Provider
{
    /// <summary>
    /// Default <see cref="IOAuthClientAuthenticationProvider"/> implementation.
    /// </summary>
    public class OAuthClientAuthenticationProvider:IOAuthClientAuthenticationProvider
    {
        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<OAuthClientAuthenticatedContext, Task> OnAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the function that is invoked when the ReturnEndpoint method is invoked.
        /// </summary>
        public Func<OAuthClientReturnEndpointContext, Task> OnReturnEndpoint { get; set; }


        /// <summary>
        /// Initializes a <see cref="OAuthClientAuthenticationProvider"/>
        /// </summary>
        public OAuthClientAuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
        }

        /// <summary>
        /// Invoked whenever OAuthClient succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task Authenticated(OAuthClientAuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task ReturnEndpoint(OAuthClientReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }
    }
}
