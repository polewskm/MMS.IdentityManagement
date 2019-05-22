using System.Security.Claims;

namespace MMS.IdentityManagement.Claims
{
    public static class IdentityClaimTypes
    {
        public const string LocalNamespace = "http://schemas.milwaukeemakerspace.org/identity/claims";

        public const string MemberId = ClaimTypes.NameIdentifier;
        public const string DisplayName = ClaimTypes.Name;
        public const string FirstName = ClaimTypes.GivenName;
        public const string LastName = ClaimTypes.Surname;

        public const string EmailAddress = ClaimTypes.Email;
        public const string PhoneNumber = ClaimTypes.HomePhone;

        public const string MemberSince = LocalNamespace + "/member_since";
        public const string RenewalDue = LocalNamespace + "/renewal_due";
        public const string BoardMemberType = LocalNamespace + "/board_member";
        public const string ChampionArea = LocalNamespace + "/champion_area";

        public const string Role = ClaimTypes.Role;

        public const string AuthenticationMethod = ClaimTypes.AuthenticationMethod;
        public const string AuthenticationTime = ClaimTypes.AuthenticationInstant;
        public const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";
        public const string ClientId = LocalNamespace + "/client_id";
        public const string Nonce = LocalNamespace + "/nonce";
    }
}