using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement
{
    public class TokenRefreshRequest : ClientRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}