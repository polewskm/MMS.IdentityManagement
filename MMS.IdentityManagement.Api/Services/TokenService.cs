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
        private readonly TokenOptions _options;
        private readonly ISystemClock _systemClock;
        private readonly SecurityTokenHandler _securityTokenHandler = CreateSecurityTokenHandler();

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
            var expiresWhen = createdWhen + _options.TokenLifetime;

            var authenticationTime = createdWhen.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var issuedAt = createdWhen.UtcDateTime;
            var expires = expiresWhen.UtcDateTime;

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
                claims.Add(new Claim(type, value, valueType, issuer));
            }

            void AddClaimIfNotEmpty(string type, string value, string valueType = ClaimValueTypes.String)
            {
                if (!string.IsNullOrEmpty(value))
                    AddClaim(type, value, valueType);
            }

            AddClaim(IdentityClaimTypes.MemberId, memberId, ClaimValueTypes.Integer);
            AddClaimIfNotEmpty(IdentityClaimTypes.DisplayName, member.DisplayName);
            AddClaimIfNotEmpty(IdentityClaimTypes.FirstName, member.FirstName);
            AddClaimIfNotEmpty(IdentityClaimTypes.LastName, member.LastName);
            AddClaimIfNotEmpty(IdentityClaimTypes.EmailAddress, member.EmailAddress, ClaimValueTypes.Email);
            AddClaimIfNotEmpty(IdentityClaimTypes.PhoneNumber, member.PhoneNumber);
            AddClaim(IdentityClaimTypes.MemberSince, memberSince, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.RenewalDue, renewalDue, ClaimValueTypes.Integer);
            AddClaim(IdentityClaimTypes.BoardMemberType, boardMemberType);

            AddClaimIfNotEmpty(IdentityClaimTypes.AuthenticationMethod, request.AuthenticationType);
            AddClaim(IdentityClaimTypes.AuthenticationTime, authenticationTime, ClaimValueTypes.Integer);
            AddClaimIfNotEmpty(IdentityClaimTypes.IdentityProvider, _options.IdentityProvider);
            AddClaim(IdentityClaimTypes.ClientId, client.Id);
            AddClaimIfNotEmpty(IdentityClaimTypes.Nonce, request.Nonce);

            claims.AddRange(member.Roles.Select(role => new Claim(IdentityClaimTypes.Role, role, ClaimValueTypes.String, issuer)));
            claims.AddRange(member.ChampionAreas.Select(area => new Claim(IdentityClaimTypes.ChampionArea, area, ClaimValueTypes.String, issuer)));

            var subject = new ClaimsIdentity(claims, request.AuthenticationType);

            cancellationToken.ThrowIfCancellationRequested();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = subject,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = expires,
                SigningCredentials = _options.SigningCredentials,
            };

            var securityToken = _securityTokenHandler.CreateToken(tokenDescriptor);
            var token = _securityTokenHandler.WriteToken(securityToken);

            var result = new CreateTokenResult
            {
                Token = token,
                SecurityToken = securityToken,
                CreatedWhen = createdWhen,
                ExpiresWhen = expiresWhen,
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

            var principal = _securityTokenHandler.ValidateToken(request.Token, validationParameters, out var securityToken);

            var result = new TokenValidationResult
            {
                Principal = principal,
                SecurityToken = securityToken,
            };
            return Task.FromResult(result);
        }

    }
}