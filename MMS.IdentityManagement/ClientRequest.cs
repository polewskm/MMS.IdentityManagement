using System.ComponentModel.DataAnnotations;

namespace MMS.IdentityManagement
{
    public abstract class ClientRequest
    {
        [Required]
        public string ClientId { get; set; }
    }
}