using System;
using System.Security.Claims;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenResult : CommonResult
    {
        public string Token { get; set; }

        public ClaimsIdentity Subject { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset ExpiresWhen { get; set; }
    }
}