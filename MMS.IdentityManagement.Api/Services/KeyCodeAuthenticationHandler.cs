﻿using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IClientValidator _clientValidator;
        private readonly IKeyCodeValidator _keyCodeValidator;
        private readonly ITokenService _tokenService;

        public KeyCodeAuthenticationHandler(IClientValidator clientValidator, IKeyCodeValidator keyCodeValidator, ITokenService tokenService)
        {
            _clientValidator = clientValidator ?? throw new ArgumentNullException(nameof(clientValidator));
            _keyCodeValidator = keyCodeValidator ?? throw new ArgumentNullException(nameof(keyCodeValidator));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
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

            var createTokenRequest = new CreateTokenRequest
            {
                TokenType = TokenTypes.AccessToken,
                AuthenticationType = AuthenticationTypes.KeyCode,
                Client = clientValidationResult.Client,
                Member = keyCodeValidationResult.Member,
                Nonce = request.Nonce,
            };
            var createTokenResult = await _tokenService.CreateTokenAsync(createTokenRequest, cancellationToken).ConfigureAwait(false);

            var result = new KeyCodeAuthenticationResult
            {
                IdentityToken = createTokenResult.Token,
                AccessToken = createTokenResult.Token,
                TokenType = TokenTypes.BearerToken,
                AccessTokenExpiresWhen = createTokenResult.ExpiresWhen,
            };

            return result;
        }

    }
}