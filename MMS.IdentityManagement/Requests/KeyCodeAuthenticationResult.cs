using System;
using MMS.IdentityManagement.Validation;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Requests
{
    public class KeyCodeAuthenticationResult : CommonResult
    {
        [JsonProperty("id_token")]
        public string IdentityToken { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public DateTimeOffset? AccessTokenExpiresWhen { get; set; }
    }
}