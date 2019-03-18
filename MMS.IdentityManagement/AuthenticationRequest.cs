using System;

namespace MMS.IdentityManagement
{
    public class AuthenticationRequest
    {
        public string ReaderId { get; set; }
        public int? MemberId { get; set; }
        public string KeyCode { get; set; }
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public string AccessTokenType { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }
        public Member Member { get; set; }
        public string[] Roles { get; set; }
    }
}