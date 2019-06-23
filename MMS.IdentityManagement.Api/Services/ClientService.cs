using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Data;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default);

        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    }

    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public virtual async Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            return await _clientRepository.GetClientsAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            return await _clientRepository.GetClientByIdAsync(clientId, cancellationToken).ConfigureAwait(false);
        }

    }
}