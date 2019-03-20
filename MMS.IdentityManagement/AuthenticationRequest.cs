using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MMS.IdentityManagement
{
    public class AuthenticationRequest
    {
        public string ClientId { get; set; }
        public int? MemberId { get; set; }
        public string KeyCode { get; set; }
        public string[] Scope { get; set; }
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public string AccessTokenType { get; set; }
        public DateTimeOffset IssuedWhen { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }
        public Member Member { get; set; }
        public ClaimsIdentity Identity { get; set; }
    }
}