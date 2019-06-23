namespace MMS.IdentityManagement.Claims
{
    // https://www.iana.org/assignments/jwt/jwt.xhtml#claims
    // https://medium.com/@darutk/understanding-id-token-5f83f50fa02e

    public static class TokenClaimTypes
    {
        public const string MemberId = "sub";
        public const string DisplayName = "name";
        public const string FirstName = "given_name";
        public const string LastName = "family_name";

        public const string EmailAddress = "email";
        public const string PhoneNumber = "phone_number";

        public const string MembershipCreatedWhen = "mbr_created";
        public const string MembershipExpiresWhen = "mbr_expires";

        public const string Role = "role";

        public const string AuthenticationMethod = "auth_method";
        public const string AuthenticationTime = "auth_time";
    }
}