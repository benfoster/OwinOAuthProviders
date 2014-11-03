// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Owin.Security.Providers.WordPress
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class WordPressAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="WordPressAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">WordPress Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public WordPressAuthenticatedContext(IOwinContext context, JObject user, string accessToken, string expires)
            : base(context)
        {
            User = user;
            Id = TryGetValue(user, "ID");
            Name = TryGetValue(user, "display_name");
            Email = TryGetValue(user, "email");
            AccessToken = accessToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }
        }

        /// <summary>
        /// The email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the WordPress user obtained from the endpoint https://public-api.wordpress.com/rest/v1/me
        /// </remarks>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the WordPress OAuth access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the WordPress access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the WordPress user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }
    }
}
