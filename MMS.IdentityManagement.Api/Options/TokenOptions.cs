using System;
using Microsoft.IdentityModel.Tokens;

namespace MMS.IdentityManagement.Api.Options
{
    public class TokenOptions
    {
        private SecurityKey _signingValidationKey;

        public string IdentityProvider { get; set; } = "MakerIdMgmt";

        public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromHours(1.0);

        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);

        public TimeSpan? ClockSkew { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public SecurityKey SigningValidationKey
        {
            get => _signingValidationKey ?? SigningCredentials?.Key;
            set => _signingValidationKey = value;
        }

    }
}