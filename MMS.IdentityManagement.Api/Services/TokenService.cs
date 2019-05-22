﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Claims;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public class TokenValidationRequest
    {
        public string Token { get; set; }
    }

    public class TokenValidationResult : CommonResult
    {
        public ClaimsPrincipal Subject { get; set; }
        public SecurityToken SecurityToken { get; set; }
    }

    public interface ITokenService
    {
        Task<CreateTokenResult> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default);

        Task<TokenValidationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    }

    // SigningCredentials

    public class TokenService : ITokenService
    {
        private readonly ISystemClock _systemClock;
        private readonly SecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        public TokenService(ISystemClock systemClock)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public virtual Task<CreateTokenResult> CreateTokenAsync(CreateTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            const string issuer = "urn:milwaukeemakerspace.org";

            var createdWhen = _systemClock.UtcNow;
            var expiresWhen = createdWhen.AddHours(24);

            var authenticationTime = createdWhen.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var issuedAt = createdWhen.UtcDateTime;
            var notBefore = issuedAt.AddMilliseconds(-100);
            var expires = expiresWhen.UtcDateTime;

            var client = request.Client;
            var audience = client.Id;

            var member = request.Member;
            var memberId = member.MemberId.ToString(CultureInfo.InvariantCulture);
            var memberSince = member.MemberSince.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var renewalDue = member.RenewalDue.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var boardMemberType = member.BoardMemberType.ToString();

            var claims = new List<Claim>
            {
                new Claim(MemberClaimTypes.MemberId, memberId, ClaimValueTypes.Integer, issuer),
                new Claim(MemberClaimTypes.DisplayName, member.DisplayName, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.FirstName, member.FirstName, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.LastName, member.LastName, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.EmailAddress, member.EmailAddress, ClaimValueTypes.Email, issuer),
                new Claim(MemberClaimTypes.PhoneNumber, member.PhoneNumber, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.MemberSince, memberSince, ClaimValueTypes.Integer, issuer),
                new Claim(MemberClaimTypes.RenewalDue, renewalDue, ClaimValueTypes.Integer, issuer),
                new Claim(MemberClaimTypes.BoardMemberType, boardMemberType, ClaimValueTypes.String, issuer),
                //
                new Claim(MemberClaimTypes.AuthenticationMethod, request.AuthenticationType, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.AuthenticationTime, authenticationTime, ClaimValueTypes.Integer, issuer),
                new Claim(MemberClaimTypes.IdentityProvider, "mms", ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.ClientId, client.Id, ClaimValueTypes.String, issuer),
            };

            if (!string.IsNullOrEmpty(request.Nonce))
                claims.Add(new Claim(MemberClaimTypes.Nonce, request.Nonce, ClaimValueTypes.String, issuer));

            claims.AddRange(member.Roles.Select(role => new Claim(MemberClaimTypes.Role, role, ClaimValueTypes.String, issuer)));
            claims.AddRange(member.ChampionAreas.Select(area => new Claim(MemberClaimTypes.ChampionArea, area, ClaimValueTypes.String, issuer)));

            var subject = new ClaimsIdentity(claims, request.AuthenticationType);

            cancellationToken.ThrowIfCancellationRequested();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = subject,
                IssuedAt = issuedAt,
                NotBefore = notBefore,
                Expires = expires,
                SigningCredentials = null,
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

    }
}