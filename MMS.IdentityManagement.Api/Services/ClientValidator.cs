using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using MMS.IdentityManagement.Api.Models;

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

        public ClientValidator(IClientRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.ClientId))
            {
                return new ClientValidationResult
                {
                    Error = "invalid_request",
                    ErrorDescription = "Invalid client_id",
                };
            }

            var client = await _repository.GetClientByIdAsync(request.ClientId, cancellationToken).ConfigureAwait(false);
            if (client == null)
            {
                return new ClientValidationResult
                {
                    Error = "unauthorized_client",
                    ErrorDescription = "Unknown client",
                };
            }

            if (client.Disabled)
            {
                return new ClientValidationResult
                {
                    Error = "unauthorized_client",
                    ErrorDescription = "Disabled client",
                };
            }

            if (client.RequireSecret)
            {
                if (string.IsNullOrEmpty(request.ClientSecret))
                {
                    return new ClientValidationResult
                    {
                        Error = "invalid_client",
                        ErrorDescription = "Disabled client",
                    };
                }
            }

            throw new NotImplementedException();
        }

    }
}