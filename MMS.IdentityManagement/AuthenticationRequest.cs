using System;
using System.Security.Claims;

namespace MMS.IdentityManagement
{
    // POST /api/v1/tokens/keycode
    // POST /api/v1/tokens/refresh

    public abstract class AuthenticationRequest
    {
        public string ClientId { get; set; }
    }

    public class KeyCodeAuthenticationRequest : AuthenticationRequest
    {
        public string KeyCode { get; set; }
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public string AccessTokenType { get; set; }
        public DateTimeOffset IssuedWhen { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }
        public Member Member { get; set; }
        public ClaimsIdentity Identity { get; set; }
    }
}