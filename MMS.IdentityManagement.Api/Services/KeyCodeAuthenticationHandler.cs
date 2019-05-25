using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MMS.IdentityManagement.Api.Models;
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
        private static readonly IErrorFactory<KeyCodeAuthenticationResult> ErrorFactory = ErrorFactory<KeyCodeAuthenticationResult>.Instance;

        private readonly IClientValidator _clientValidator;
        private readonly IKeyCodeValidator _keyCodeValidator;
        private readonly ITokenService _tokenService;
        private readonly ISystemClock _systemClock;

        public KeyCodeAuthenticationHandler(IClientValidator clientValidator, IKeyCodeValidator keyCodeValidator, ITokenService tokenService, ISystemClock systemClock)
        {
            _clientValidator = clientValidator ?? throw new ArgumentNullException(nameof(clientValidator));
            _keyCodeValidator = keyCodeValidator ?? throw new ArgumentNullException(nameof(keyCodeValidator));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public virtual async Task<KeyCodeAuthenticationResult> AuthenticateAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default)
        {
            var clientValidationRequest = new ClientValidationRequest
            {
                AuthenticationType = AuthenticationTypes.KeyCode,
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
            };

            var clientValidationResult = await _clientValidator.ValidateClientAsync(clientValidationRequest, cancellationToken).ConfigureAwait(false);
            if (!clientValidationResult.Success)
                return ErrorFactory.Clone(clientValidationResult);

            var keyCodeValidationRequest = new KeyCodeValidationRequest
            {
                KeyCode = request.KeyCode,
            };

            var keyCodeValidationResult = await _keyCodeValidator.ValidateKeyCodeAsync(keyCodeValidationRequest, cancellationToken).ConfigureAwait(false);
            if (!keyCodeValidationResult.Success)
                return ErrorFactory.Clone(keyCodeValidationResult);

            var createTokenRequest = new CreateTokenRequest
            {
                AuthenticationType = AuthenticationTypes.KeyCode,
                AuthenticationTime = _systemClock.UtcNow,
                Client = clientValidationResult.Client,
                Member = keyCodeValidationResult.Member,
                Nonce = request.Nonce,
            };

            var createTokenResult = await _tokenService.CreateTokenAsync(createTokenRequest, cancellationToken).ConfigureAwait(false);
            if (!createTokenResult.Success)
                return ErrorFactory.Clone(createTokenResult);

            var result = new KeyCodeAuthenticationResult
            {
                TokenType = TokenTypes.BearerToken,
                IdentityToken = createTokenResult.AccessToken,
                AccessToken = createTokenResult.AccessToken,
                AccessTokenExpiresWhen = createTokenResult.AccessTokenExpiresWhen,
                RefreshToken = createTokenResult.RefreshToken,
            };

            return result;
        }

    }
}