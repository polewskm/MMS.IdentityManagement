using System;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenResult : CommonResult
    {
        public string Token { get; set; }

        public SecurityToken SecurityToken { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset ExpiresWhen { get; set; }
    }
}