using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MMS.IdentityManagement.Api.Services
{
    public interface ITokenConverter
    {
        ClaimsIdentity IdentityFromToken(string token, string authenticationType);
    }

    public class TokenConverter : ITokenConverter
    {
        public virtual ClaimsIdentity IdentityFromToken(string token, string authenticationType)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(authenticationType))
                throw new ArgumentNullException(nameof(authenticationType));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, authenticationType);

            return identity;
        }

    }
}