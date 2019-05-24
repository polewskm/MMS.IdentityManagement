using System.Security.Claims;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class TokenValidationResult : CommonResult
    {
        public ClaimsPrincipal Principal { get; set; }
    }
}