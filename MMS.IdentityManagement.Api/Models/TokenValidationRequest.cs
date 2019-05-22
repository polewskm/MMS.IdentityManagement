using System.Collections.Generic;

namespace MMS.IdentityManagement.Api.Models
{
    public class TokenValidationRequest
    {
        public string Token { get; set; }

        public IEnumerable<string> ValidAudiences { get; set; }
    }
}