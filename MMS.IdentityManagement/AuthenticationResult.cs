using System;
using System.Security.Claims;

namespace MMS.IdentityManagement
{
    public class AuthenticationResult
    {
        // https://auth0.com/docs/tokens
        // https://auth0.com/docs/api-auth/why-use-access-tokens-to-secure-apis

        public string IdToken { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenType { get; set; }

        public DateTimeOffset AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset RefreshTokenExpiration { get; set; }

        public ClaimsIdentity Identity { get; set; }
    }
}