using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Claims;
using MMS.IdentityManagement.Requests;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IKeyCodeAuthenticationHandler
    {
        Task<KeyCodeAuthenticationResult> AuthenticateAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default);
    }

    public class KeyCodeAuthenticationHandler : IKeyCodeAuthenticationHandler
    {
        private readonly IClientValidator _clientValidator;
        private readonly IKeyCodeValidator _keyCodeValidator;
        private readonly SecurityTokenHandler _securityTokenHandler;

        public KeyCodeAuthenticationHandler(IClientValidator clientValidator, IKeyCodeValidator keyCodeValidator)
        {
            _clientValidator = clientValidator ?? throw new ArgumentNullException(nameof(clientValidator));
            _keyCodeValidator = keyCodeValidator ?? throw new ArgumentNullException(nameof(keyCodeValidator));
        }

        public virtual async Task<KeyCodeAuthenticationResult> AuthenticateAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default)
        {
            var clientValidationRequest = new ClientValidationRequest
            {
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
            };
            var clientValidationResult = await _clientValidator.ValidateClientAsync(clientValidationRequest, cancellationToken).ConfigureAwait(false);
            if (!clientValidationResult.Success)
                return clientValidationResult.AsError<KeyCodeAuthenticationResult>();

            var keyCodeValidationRequest = new KeyCodeValidationRequest
            {
                Client = clientValidationResult.Client,
                KeyCode = request.KeyCode,
            };
            var keyCodeValidationResult = await _keyCodeValidator.ValidateKeyCodeAsync(keyCodeValidationRequest, cancellationToken).ConfigureAwait(false);
            if (!keyCodeValidationResult.Success)
                return keyCodeValidationResult.AsError<KeyCodeAuthenticationResult>();

            var member = keyCodeValidationResult.Member;

            const string issuer = "urn:milwaukeemakerspace.org";
            const string authenticationType = "keycode";

            var now = DateTimeOffset.Now;
            var authenticationTime = now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            var issuedAt = now.UtcDateTime;
            var notBefore = issuedAt.AddMilliseconds(-100);
            var expires = issuedAt.AddHours(24);

            var audience = clientValidationResult.Client.Id;
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
                new Claim(MemberClaimTypes.AuthenticationMethod, authenticationType, ClaimValueTypes.String, issuer),
                new Claim(MemberClaimTypes.AuthenticationTime, authenticationTime, ClaimValueTypes.Integer, issuer),
                new Claim(MemberClaimTypes.IdentityProvider, "mms", ClaimValueTypes.String, issuer),
            };

            if (!string.IsNullOrEmpty(request.Nonce))
                claims.Add(new Claim(MemberClaimTypes.Nonce, request.Nonce, ClaimValueTypes.String, issuer));

            claims.AddRange(member.Roles.Select(role => new Claim(MemberClaimTypes.Role, role, ClaimValueTypes.String, issuer)));
            claims.AddRange(member.ChampionAreas.Select(area => new Claim(MemberClaimTypes.ChampionArea, area, ClaimValueTypes.String, issuer)));

            var identity = new ClaimsIdentity(claims, authenticationType);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                IssuedAt = issuedAt,
                NotBefore = notBefore,
                Expires = expires,
                SigningCredentials = null,
            };
            var securityToken = _securityTokenHandler.CreateToken(tokenDescriptor);
            var token = _securityTokenHandler.WriteToken(securityToken);

            throw new NotImplementedException();
        }

    }
}