using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Models
{
    public class ClientValidationResult : CommonResult
    {
        public Client Client { get; set; }

        public Secret Secret { get; set; }
    }
}