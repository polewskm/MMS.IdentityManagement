using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Requests
{
    public abstract class ClientRequest
    {
        [Required]
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [Required]
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}