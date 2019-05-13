using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement
{
    public abstract class ClientRequest
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
    }
}