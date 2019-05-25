namespace MMS.IdentityManagement.Api.Models
{
    public class ClientValidationRequest
    {
        public string AuthenticationType { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}