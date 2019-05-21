using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class KeyCodeValidationResult : CommonResult
    {
        public Member Member { get; set; }
    }
}