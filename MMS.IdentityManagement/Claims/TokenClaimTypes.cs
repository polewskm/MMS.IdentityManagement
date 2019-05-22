using System.IdentityModel.Tokens.Jwt;

namespace MMS.IdentityManagement.Claims
{
    // https://www.iana.org/assignments/jwt/jwt.xhtml#claims
    // https://medium.com/@darutk/understanding-id-token-5f83f50fa02e

    public static class TokenClaimTypes
    {
        public const string MemberId = JwtRegisteredClaimNames.Sub;
        public const string DisplayName = "name";
        public const string FirstName = JwtRegisteredClaimNames.GivenName;
        public const string LastName = JwtRegisteredClaimNames.FamilyName;

        public const string EmailAddress = JwtRegisteredClaimNames.Email;
        public const string PhoneNumber = "phone_number";

        public const string MemberSince = "mms_joined";
        public const string RenewalDue = "mms_renewal";
        public const string BoardMemberType = "mms_bod";
        public const string ChampionArea = "mms_champ";

        public const string Role = "role";

        public const string AuthenticationMethod = "auth_method";
        public const string AuthenticationTime = JwtRegisteredClaimNames.AuthTime;
        public const string IdentityProvider = "idp";
        public const string ClientId = "client_id";
        public const string Nonce = JwtRegisteredClaimNames.Nonce;
    }
}