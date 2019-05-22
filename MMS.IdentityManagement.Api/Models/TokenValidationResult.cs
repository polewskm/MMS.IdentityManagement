using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class TokenValidationResult : CommonResult
    {
        public ClaimsPrincipal Principal { get; set; }

        public SecurityToken SecurityToken { get; set; }
    }
}