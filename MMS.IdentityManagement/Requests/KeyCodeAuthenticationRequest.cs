using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Requests
{
    public class KeyCodeAuthenticationRequest : ClientRequest
    {
        [Required]
        [JsonProperty("keycode")]
        public string KeyCode { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}