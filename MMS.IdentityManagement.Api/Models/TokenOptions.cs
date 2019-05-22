using System;
using Microsoft.IdentityModel.Tokens;

namespace MMS.IdentityManagement.Api.Models
{
    public class TokenOptions
    {
        private SecurityKey _signingValidationKey;

        public string Issuer { get; set; } = "urn:milwaukeemakerspace.org";

        public string IdentityProvider { get; set; } = "MoMI";

        public TimeSpan TokenLifetime { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public SecurityKey SigningValidationKey
        {
            get => _signingValidationKey ?? SigningCredentials?.Key;
            set => _signingValidationKey = value;
        }

    }
}