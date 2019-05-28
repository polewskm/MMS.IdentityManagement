using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Data;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.SecretProtectors;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientValidator
    {
        Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default);
    }

    public class ClientValidator : IClientValidator
    {
        private static readonly IErrorFactory<ClientValidationResult> ErrorFactory = ErrorFactory<ClientValidationResult>.Instance;

        private readonly IClientRepository _repository;
        private readonly ISecretProtectorSelector _secretProtectorSelector;

        public ClientValidator(IClientRepository repository, ISecretProtectorSelector secretProtectorSelector)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _secretProtectorSelector = secretProtectorSelector ?? throw new ArgumentNullException(nameof(secretProtectorSelector));
        }

        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request?.ClientId))
                return ErrorFactory.InvalidRequest("Missing client_id");

            var client = await _repository.GetClientByIdAsync(request.ClientId, cancellationToken).ConfigureAwait(false);
            if (client == null)
                return ErrorFactory.InvalidClient("Unknown client_id");

            if (client.Disabled)
                return ErrorFactory.UnauthorizedClient("Disabled client");

            Secret secret = null;
            if (client.RequireSecret)
            {
                if (string.IsNullOrEmpty(request.ClientSecret))
                    return ErrorFactory.InvalidRequest("Missing client_secret");

                secret = client.Secrets.FirstOrDefault(s => _secretProtectorSelector
                    .Select(s.CipherType)
                    .Verify(request.ClientSecret, s.CipherText));

                if (secret == null)
                    return ErrorFactory.UnauthorizedClient("Invalid client_secret");
            }

            var result = new ClientValidationResult
            {
                Client = client,
                Secret = secret,
            };
            return result;
        }

    }
}