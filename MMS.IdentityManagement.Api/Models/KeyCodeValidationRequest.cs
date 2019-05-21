namespace MMS.IdentityManagement.Api.Models
{
    public class KeyCodeValidationRequest
    {
        public Client Client { get; set; }

        public string KeyCode { get; set; }
    }
}