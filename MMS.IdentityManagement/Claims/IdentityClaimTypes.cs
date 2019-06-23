using System.Security.Claims;

namespace MMS.IdentityManagement.Claims
{
    public static class IdentityClaimTypes
    {
        public const string LocalNamespace = "http://schemas.makerspace.local/2019/06/identity/claims";

        public const string MemberId = ClaimTypes.NameIdentifier;
        public const string DisplayName = ClaimTypes.Name;
        public const string FirstName = ClaimTypes.GivenName;
        public const string LastName = ClaimTypes.Surname;

        public const string EmailAddress = ClaimTypes.Email;
        public const string PhoneNumber = ClaimTypes.HomePhone;

        public const string MembershipCreatedWhen = LocalNamespace + "/member_created";
        public const string MembershipExpiresWhen = LocalNamespace + "/member_expires";

        public const string Role = ClaimTypes.Role;

        public const string AuthenticationMethod = ClaimTypes.AuthenticationMethod;
        public const string AuthenticationTime = ClaimTypes.AuthenticationInstant;
    }
}