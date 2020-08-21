using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.Owin.Security.OAuthClient.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class OAuthClientAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="OAuthClientAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">OAuthClient Access token</param>
        public OAuthClientAuthenticatedContext(IOwinContext context, JObject user, string accessToken, int expiresIn)
            : base(context)
        {
            AccessToken = accessToken;
            User = user;
			ExpiresIn = TimeSpan.FromSeconds(expiresIn);

            Id = TryGetValue(user, "id");
            Name = TryGetValue(user, "profile_img");
            Email = TryGetValue(user, "section");
            Alias = TryGetValue(user, "city");
        }

		/// <summary>
		/// Gets the JSON-serialized user
		/// </summary>
		/// <remarks>
        /// Contains the OAuthClient user obtained from the endpoint https://api.weibo.com/oauth2/get_token_info
		/// </remarks>
		public JObject User { get; private set; }

		/// <summary>
		/// Gets the OAuthClient OAuth access token
		/// </summary>
		public string AccessToken { get; private set; }

		/// <summary>
        /// Gets the OAuthClient access token expiration time
		/// </summary>
		public TimeSpan? ExpiresIn { get; set; }

		/// <summary>
		/// Get the user's id
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Get the user's displayName
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Get the user's email
		/// </summary>
		public string Email { get; private set; }

		/// <summary>
		/// Get the user's publicAlias
		/// </summary>
		public string Alias { get; private set; }

		/// <summary>
		/// Gets the <see cref="ClaimsIdentity"/> representing the user
		/// </summary>
		public ClaimsIdentity Identity { get; set; }

		/// <summary>
		/// Gets or sets a property bag for common authentication properties
		/// </summary>
		public AuthenticationProperties Properties { get; set; }

		private static string TryGetValue(JObject user, string propertyName) {
			JToken value;
			return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
		}
    }
}
