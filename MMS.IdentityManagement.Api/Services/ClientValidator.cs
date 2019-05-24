using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.SecretProtectors;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientValidator
    {
        Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default);
    }

    public interface IClientRepository
    {
        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    }

    public class ClientValidator : IClientValidator
    {
        private readonly IClientRepository _repository;
        private readonly ISecretProtector _secretProtector;

        public ClientValidator(IClientRepository repository, ISecretProtector secretProtector)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
        }

        private static ClientValidationResult Invalid(string description) => Error("invalid_request", description);

        private static ClientValidationResult Unauthorized(string description) => Error("unauthorized_client", description);

        private static ClientValidationResult Error(string error, string description)
        {
            return new ClientValidationResult
            {
                Error = error,
                ErrorDescription = description,
            };
        }

        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request?.ClientId))
                return Invalid("Invalid client_id");

            var client = await _repository.GetClientByIdAsync(request.ClientId, cancellationToken).ConfigureAwait(false);
            if (client == null)
                return Invalid("Unknown client_id");

            if (client.Disabled)
                return Unauthorized("Disabled client");

            Secret secret = null;
            if (client.RequireSecret)
            {
                if (string.IsNullOrEmpty(request.ClientSecret))
                    return Unauthorized("Missing client_secret");

                secret = client.Secrets.FirstOrDefault(_ => _secretProtector.Verify(request.ClientSecret, _.CipherValue));
                if (secret == null)
                    return Unauthorized("Invalid client_secret");
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