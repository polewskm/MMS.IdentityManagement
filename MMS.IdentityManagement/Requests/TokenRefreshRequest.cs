using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement.Requests
{
    public class TokenRefreshRequest : ClientRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}