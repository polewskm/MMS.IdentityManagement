using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Requests
{
    public class TokenRefreshRequest : ClientRequest
    {
        [Required]
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}