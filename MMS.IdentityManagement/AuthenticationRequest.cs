namespace MMS.IdentityManagement
{
    public class AuthenticationRequest
    {
        public string ReaderId { get; set; }
        public int? MemberId { get; set; }
        public string KeyCode { get; set; }
    }
}