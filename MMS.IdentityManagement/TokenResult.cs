using System;

namespace MMS.IdentityManagement
{
    public class TokenResult
    {
        // https://auth0.com/docs/tokens
        // https://auth0.com/docs/api-auth/why-use-access-tokens-to-secure-apis

        public string IdentityToken { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenType { get; set; }

        public DateTimeOffset? AccessTokenExpiresWhen { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset? RefreshTokenExpiresWhen { get; set; }
    }
}