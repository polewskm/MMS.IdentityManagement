using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Api.Extensions;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.Options;
using MMS.IdentityManagement.Claims;

namespace MMS.IdentityManagement.Api.Services
{
    public interface ITokenService
    {
        Task<CreateTokenResult> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default);

        Task<TokenValidationResult> ValidateTokenAsync(TokenValidationRequest request, CancellationToken cancellationToken = default);
    }

    public class TokenService : ITokenService
    {
        private readonly SecurityTokenHandler _securityTokenHandler = CreateSecurityTokenHandler();
        private readonly TokenOptions _options;
        private readonly ISystemClock _systemClock;

        private static SecurityTokenHandler CreateSecurityTokenHandler()
        {
            var handler = new JwtSecurityTokenHandler
            {
                InboundClaimTypeMap =
                {
                    [TokenClaimTypes.MemberId] = IdentityClaimTypes.MemberId,
                    [TokenClaimTypes.DisplayName] = IdentityClaimTypes.DisplayName,
                    [TokenClaimTypes.FirstName] = IdentityClaimTypes.FirstName,
                    [TokenClaimTypes.LastName] = IdentityClaimTypes.LastName,
                    [TokenClaimTypes.EmailAddress] = IdentityClaimTypes.EmailAddress,
                    [TokenClaimTypes.PhoneNumber] = IdentityClaimTypes.PhoneNumber,
                    [TokenClaimTypes.MembershipCreatedWhen] = IdentityClaimTypes.MembershipCreatedWhen,
                    [TokenClaimTypes.MembershipExpiresWhen] = IdentityClaimTypes.MembershipExpiresWhen,
                    [TokenClaimTypes.Role] = IdentityClaimTypes.Role,
                    [TokenClaimTypes.AuthenticationMethod] = IdentityClaimTypes.AuthenticationMethod,
                    [TokenClaimTypes.AuthenticationTime] = IdentityClaimTypes.AuthenticationTime,
                },

                OutboundClaimTypeMap =
                {
                    [IdentityClaimTypes.MemberId] = TokenClaimTypes.MemberId,
                    [IdentityClaimTypes.DisplayName] = TokenClaimTypes.DisplayName,
                    [IdentityClaimTypes.FirstName] = TokenClaimTypes.FirstName,
                    [IdentityClaimTypes.LastName] = TokenClaimTypes.LastName,
                    [IdentityClaimTypes.EmailAddress] = TokenClaimTypes.EmailAddress,
                    [IdentityClaimTypes.PhoneNumber] = TokenClaimTypes.PhoneNumber,
                    [IdentityClaimTypes.MembershipCreatedWhen] = TokenClaimTypes.MembershipCreatedWhen,
                    [IdentityClaimTypes.MembershipExpiresWhen] = TokenClaimTypes.MembershipExpiresWhen,
                    [IdentityClaimTypes.Role] = TokenClaimTypes.Role,
                    [IdentityClaimTypes.AuthenticationMethod] = TokenClaimTypes.AuthenticationMethod,
                    [IdentityClaimTypes.AuthenticationTime] = TokenClaimTypes.AuthenticationTime,
                }
            };

            return handler;
        }

        public TokenService(IOptions<TokenOptions> options, ISystemClock systemClock)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public virtual Task<CreateTokenResult> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var createdWhen = _systemClock.UtcNow.TruncateMilliseconds();
            var accessTokenExpiresWhen = (createdWhen + _options.AccessTokenLifetime).TruncateMilliseconds();
            var refreshTokenExpiresWhen = (createdWhen + _options.RefreshTokenLifetime).TruncateMilliseconds();
            var authenticationTime = request.AuthenticationTime.TruncateMilliseconds().ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

            var client = request.Client;
            var audience = client.Id;
            var issuer = _options.IdentityProvider;

            var member = request.Member;
            var memberId = member.MemberId.ToString(CultureInfo.InvariantCulture);
            var membershipCreatedWhen = member.MembershipCreatedWhen.TruncateMilliseconds().ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var membershipExpiresWhen = member.MembershipExpiresWhen.TruncateMilliseconds().ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

            var claims = new List<Claim>();

            void AddClaim(string type, string value, string valueType = ClaimValueTypes.String)
            {
                if (!string.IsNullOrEmpty(value))
                    claims.Add(new Claim(type, value, valueType, issuer));
            }

            AddClaim(IdentityClaimTypes.MemberId, memberId, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.DisplayName, member.DisplayName);
            AddClaim(IdentityClaimTypes.FirstName, member.FirstName);
            AddClaim(IdentityClaimTypes.LastName, member.LastName);
            AddClaim(IdentityClaimTypes.EmailAddress, member.EmailAddress);
            AddClaim(IdentityClaimTypes.PhoneNumber, member.PhoneNumber);
            AddClaim(IdentityClaimTypes.MembershipCreatedWhen, membershipCreatedWhen, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.MembershipExpiresWhen, membershipExpiresWhen, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.AuthenticationMethod, request.AuthenticationType);
            AddClaim(IdentityClaimTypes.AuthenticationTime, authenticationTime, ClaimValueTypes.Integer);

            claims.AddRange(member.Roles.Select(role => new Claim(IdentityClaimTypes.Role, role, ClaimValueTypes.String, issuer)));

            var identity = new ClaimsIdentity(claims, request.AuthenticationType);

            cancellationToken.ThrowIfCancellationRequested();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = createdWhen.UtcDateTime,
                NotBefore = createdWhen.UtcDateTime,
                Expires = accessTokenExpiresWhen.UtcDateTime,
                SigningCredentials = _options.SigningCredentials,
            };

            var securityToken = _securityTokenHandler.CreateToken(tokenDescriptor);
            var token = _securityTokenHandler.WriteToken(securityToken);

            var result = new CreateTokenResult
            {
                Identity = identity,
                CreatedWhen = createdWhen,
                AccessToken = token,
                AccessTokenExpiresWhen = accessTokenExpiresWhen,
                RefreshToken = null, // TODO
                RefreshTokenExpiresWhen = refreshTokenExpiresWhen,
            };
            return Task.FromResult(result);
        }

        public virtual Task<TokenValidationResult> ValidateTokenAsync(TokenValidationRequest request, CancellationToken cancellationToken = default)
        {
            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                SaveSigninToken = false,
                ValidateActor = false,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = true,
                ValidateTokenReplay = false,

                ValidIssuer = _options.IdentityProvider,
                ValidAudiences = request.ValidAudiences,
                IssuerSigningKey = _options.SigningValidationKey,
                ClockSkew = _options.ClockSkew ?? TokenValidationParameters.DefaultClockSkew,

                AuthenticationType = request.AuthenticationType,
            };

            cancellationToken.ThrowIfCancellationRequested();

            var result = new TokenValidationResult();
            try
            {
                result.Principal = _securityTokenHandler.ValidateToken(request.Token, validationParameters, out _);
            }
            catch (SecurityTokenExpiredException exception)
            {
                result.Error = ErrorCodes.ExpiredToken;
                result.ErrorDescription = "Lifetime validation failed.";
                result.Exception = exception;
            }
            catch (Exception exception)
            {
                result.Error = ErrorCodes.InvalidGrant;
                result.ErrorDescription = exception.Message;
                result.Exception = exception;
            }

            return Task.FromResult(result);
        }

    }
}