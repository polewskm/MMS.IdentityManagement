using System;
using Microsoft.IdentityModel.Tokens;

namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenResult
    {
        public string Token { get; set; }

        public SecurityToken SecurityToken { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset ExpiresWhen { get; set; }
    }
}