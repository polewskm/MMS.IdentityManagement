using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMS.IdentityManagement
{
    public class TokenResponse
    {
        // https://auth0.com/docs/tokens
        // https://auth0.com/docs/api-auth/why-use-access-tokens-to-secure-apis

        [JsonProperty("id_token")]
        public string IdentityToken { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string AccessTokenType { get; set; }

        [JsonProperty("expires_in")]
        public DateTimeOffset? AccessTokenExpiresWhen { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("")]
        public DateTimeOffset? RefreshTokenExpiresWhen { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}