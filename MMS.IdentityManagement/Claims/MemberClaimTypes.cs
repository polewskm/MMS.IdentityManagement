namespace MMS.IdentityManagement.Claims
{
    // https://www.iana.org/assignments/jwt/jwt.xhtml#claims
    // https://medium.com/@darutk/understanding-id-token-5f83f50fa02e

    public static class MemberClaimTypes
    {
        public const string MemberId = "sub";
        public const string DisplayName = "name";
        public const string FirstName = "given_name";
        public const string LastName = "family_name";
        public const string EmailAddress = "email";
        public const string PhoneNumber = "phone_number";

        public const string Role = "role";

        public const string MemberSince = "mms_member_since";
        public const string RenewalDue = "mms_renewal_due";
        public const string BoardMemberType = "mms_board_member";
        public const string ChampionArea = "mms_champion_area";

        public const string AuthenticationMethod = "amr";
        public const string AuthenticationTime = "auth_time";
        public const string IdentityProvider = "idp";
        public const string ClientId = "client_id";
        public const string Nonce = "nonce";
    }
}