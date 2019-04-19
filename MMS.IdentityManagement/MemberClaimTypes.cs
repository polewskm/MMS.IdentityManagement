using System.Security.Claims;

namespace MMS.IdentityManagement
{
    public static class MemberClaimTypes
    {
        public const string LocalNamespace = "http://schemas.milwaukeemakerspace.org/ws/2019/04/identity/claims";

        public const string MemberId = ClaimTypes.NameIdentifier;
        public const string DisplayName = ClaimTypes.Name;
        public const string FirstName = ClaimTypes.GivenName;
        public const string LastName = ClaimTypes.Surname;
        public const string EmailAddress = ClaimTypes.Email;
        public const string Expiration = ClaimTypes.Expiration;
        public const string Role = ClaimTypes.Role;

        public const string Champion = LocalNamespace + "/champion";
    }
}