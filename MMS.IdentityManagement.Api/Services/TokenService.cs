using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface ITokenService
    {
        Task<TokenValidationResult> ValidateKeyCodeAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default);
    }

    public class TokenService : ITokenService
    {
        private readonly IClientService _clientService;
        private readonly ITokenConverter _tokenConverter;

        public TokenService(IClientService clientService, ITokenConverter tokenConverter)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _tokenConverter = tokenConverter ?? throw new ArgumentNullException(nameof(tokenConverter));
        }

        public virtual async Task<TokenValidationResult> ValidateKeyCodeAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var validateClient = await _clientService.ValidateClientAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validateClient.IsSuccess)
            {
                return new TokenValidationResult
                {
                    Error = validateClient.Error,
                    ErrorDescription = validateClient.ErrorDescription,
                };
            }

            var tokenResult = new TokenResponse
            {
            };

            throw new NotImplementedException();
        }

    }
}