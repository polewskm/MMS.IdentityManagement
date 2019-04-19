using System;
using System.IdentityModel.Tokens.Jwt;

namespace MMS.IdentityManagement
{
    public interface ITokenConverter
    {
        MemberIdentity IdentityFromIdToken(string idToken);
    }

    public class TokenConverter : ITokenConverter
    {
        public MemberIdentity IdentityFromIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);

            return new MemberIdentity(jwtToken.Claims, "authType");
        }

    }
}