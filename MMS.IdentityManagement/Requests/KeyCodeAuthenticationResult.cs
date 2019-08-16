using System;
using MMS.IdentityManagement.Validation;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Requests
{
    public class KeyCodeAuthenticationResult : CommonResult
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("id_token")]
        public string IdentityToken { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        protected long? AccessTokenExpiresIn
        {
            get => AccessTokenExpiresWhen?.ToUnixTimeSeconds();
            set => AccessTokenExpiresWhen = value.HasValue ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeSeconds(value.Value) : null;
        }

        [JsonIgnore]
        public DateTimeOffset? AccessTokenExpiresWhen { get; set; }

        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshToken { get; set; }
    }
}