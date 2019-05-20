using System.Security.Claims;

namespace MMS.IdentityManagement.Validation
{
    public class KeyCodeValidationResult : ValidationResult
    {
        public ClaimsIdentity ClaimsIdentity { get; set; }
    }
}