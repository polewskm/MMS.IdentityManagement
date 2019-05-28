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
                    [TokenClaimTypes.MemberSince] = IdentityClaimTypes.MemberSince,
                    [TokenClaimTypes.RenewalDue] = IdentityClaimTypes.RenewalDue,
                    [TokenClaimTypes.BoardMemberType] = IdentityClaimTypes.BoardMemberType,
                    [TokenClaimTypes.ChampionArea] = IdentityClaimTypes.ChampionArea,
                    [TokenClaimTypes.Role] = IdentityClaimTypes.Role,
                    [TokenClaimTypes.AuthenticationMethod] = IdentityClaimTypes.AuthenticationMethod,
                    [TokenClaimTypes.AuthenticationTime] = IdentityClaimTypes.AuthenticationTime,
                    [TokenClaimTypes.IdentityProvider] = IdentityClaimTypes.IdentityProvider,
                    [TokenClaimTypes.ClientId] = IdentityClaimTypes.ClientId,
                    [TokenClaimTypes.Nonce] = IdentityClaimTypes.Nonce
                },

                OutboundClaimTypeMap =
                {
                    [IdentityClaimTypes.MemberId] = TokenClaimTypes.MemberId,
                    [IdentityClaimTypes.DisplayName] = TokenClaimTypes.DisplayName,
                    [IdentityClaimTypes.FirstName] = TokenClaimTypes.FirstName,
                    [IdentityClaimTypes.LastName] = TokenClaimTypes.LastName,
                    [IdentityClaimTypes.EmailAddress] = TokenClaimTypes.EmailAddress,
                    [IdentityClaimTypes.PhoneNumber] = TokenClaimTypes.PhoneNumber,
                    [IdentityClaimTypes.MemberSince] = TokenClaimTypes.MemberSince,
                    [IdentityClaimTypes.RenewalDue] = TokenClaimTypes.RenewalDue,
                    [IdentityClaimTypes.BoardMemberType] = TokenClaimTypes.BoardMemberType,
                    [IdentityClaimTypes.ChampionArea] = TokenClaimTypes.ChampionArea,
                    [IdentityClaimTypes.Role] = TokenClaimTypes.Role,
                    [IdentityClaimTypes.AuthenticationMethod] = TokenClaimTypes.AuthenticationMethod,
                    [IdentityClaimTypes.AuthenticationTime] = TokenClaimTypes.AuthenticationTime,
                    [IdentityClaimTypes.IdentityProvider] = TokenClaimTypes.IdentityProvider,
                    [IdentityClaimTypes.ClientId] = TokenClaimTypes.ClientId,
                    [IdentityClaimTypes.Nonce] = TokenClaimTypes.Nonce
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

            var createdWhen = _systemClock.UtcNow;
            var accessTokenExpiration = createdWhen + _options.AccessTokenLifetime;
            var refreshTokenExpiration = createdWhen + _options.RefreshTokenLifetime;
            var authenticationTime = request.AuthenticationTime.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

            var client = request.Client;
            var audience = client.Id;
            var issuer = _options.Issuer;

            var member = request.Member;
            var memberId = member.MemberId.ToString(CultureInfo.InvariantCulture);
            var memberSince = member.MemberSince.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var renewalDue = member.RenewalDue.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var boardMemberType = member.BoardMemberType.ToString();

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
            AddClaim(IdentityClaimTypes.EmailAddress, member.EmailAddress, ClaimValueTypes.Email);
            AddClaim(IdentityClaimTypes.PhoneNumber, member.PhoneNumber);
            AddClaim(IdentityClaimTypes.MemberSince, memberSince, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.RenewalDue, renewalDue, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.BoardMemberType, boardMemberType);

            AddClaim(IdentityClaimTypes.AuthenticationMethod, request.AuthenticationType);
            AddClaim(IdentityClaimTypes.AuthenticationTime, authenticationTime, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.IdentityProvider, _options.IdentityProvider);
            AddClaim(IdentityClaimTypes.ClientId, client.Id);
            AddClaim(IdentityClaimTypes.Nonce, request.Nonce);

            claims.AddRange(member.Roles.Select(role => new Claim(IdentityClaimTypes.Role, role, ClaimValueTypes.String, issuer)));
            claims.AddRange(member.ChampionAreas.Select(area => new Claim(IdentityClaimTypes.ChampionArea, area, ClaimValueTypes.String, issuer)));

            var subject = new ClaimsIdentity(claims, request.AuthenticationType);

            cancellationToken.ThrowIfCancellationRequested();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = subject,
                IssuedAt = createdWhen.UtcDateTime,
                NotBefore = createdWhen.UtcDateTime,
                Expires = accessTokenExpiration.UtcDateTime,
                SigningCredentials = _options.SigningCredentials,
            };

            var securityToken = _securityTokenHandler.CreateToken(tokenDescriptor);
            var token = _securityTokenHandler.WriteToken(securityToken);

            var result = new CreateTokenResult
            {
                Subject = subject,
                CreatedWhen = createdWhen,
                AccessToken = token,
                AccessTokenExpiresWhen = accessTokenExpiration,
                RefreshToken = null, // TODO
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

                ValidIssuer = _options.Issuer,
                ValidAudiences = request.ValidAudiences,
                IssuerSigningKey = _options.SigningValidationKey,
            };

            cancellationToken.ThrowIfCancellationRequested();

            var result = new TokenValidationResult();
            try
            {
                result.Principal = _securityTokenHandler.ValidateToken(request.Token, validationParameters, out var securityToken);
            }
            catch (SecurityTokenValidationException exception)
            {
                result.Exception = exception;
            }
            return Task.FromResult(result);
        }

    }
}