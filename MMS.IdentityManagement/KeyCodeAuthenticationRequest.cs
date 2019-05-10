using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement
{
    public class KeyCodeAuthenticationRequest : ClientRequest
    {
        [Required]
        public string KeyCode { get; set; }
    }
}