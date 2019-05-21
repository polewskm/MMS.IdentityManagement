using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement.Requests
{
    public class KeyCodeAuthenticationRequest : ClientRequest
    {
        [Required]
        public string KeyCode { get; set; }

        public string Nonce { get; set; }
    }
}