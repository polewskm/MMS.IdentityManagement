using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Requests;

namespace MMS.IdentityManagement.Data
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default);

        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);

        Task CreateClientAsync(Client client, CancellationToken cancellationToken = default);

        Task UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default);

        Task CreateClientSecretAsync(string clientId, Secret secret, CancellationToken cancellationToken = default);

        Task DeleteClientSecretAsync(string clientId, string secretId, CancellationToken cancellationToken = default);
    }
}