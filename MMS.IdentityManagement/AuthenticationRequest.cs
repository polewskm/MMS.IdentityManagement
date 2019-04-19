namespace MMS.IdentityManagement
{
    // POST /api/v1/tokens/keycode
    // POST /api/v1/tokens/refresh

    public abstract class AuthenticationRequest
    {
        public string ClientId { get; set; }
    }
}