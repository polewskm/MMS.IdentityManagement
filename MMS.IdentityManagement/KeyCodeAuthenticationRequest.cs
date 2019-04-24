using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement
{
    public class KeyCodeAuthenticationRequest : AuthenticationRequest
    {
        [Required]
        public string KeyCode { get; set; }
    }
}