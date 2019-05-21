namespace MMS.IdentityManagement.Api.Models
{
    public class CreateTokenRequest
    {
        public string TokenType { get; set; }
        public string AuthenticationType { get; set; }
        public Client Client { get; set; }
        public Member Member { get; set; }
        public string Nonce { get; set; }
    }
}