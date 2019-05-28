using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Requests;

namespace MMS.IdentityManagement.Api.Data
{
    public interface IClientRepository
    {
        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);

        Task CreateClientAsync(Client client, CancellationToken cancellationToken = default);

        Task UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default);

        Task CreateClientSecretAsync(string clientId, Secret secret, CancellationToken cancellationToken = default);

        Task DeleteClientSecretAsync(string clientId, string secretId, CancellationToken cancellationToken = default);
    }

    public class ClientRepository : IClientRepository
    {
        public virtual Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task CreateClientAsync(Client client, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task CreateClientSecretAsync(string clientId, Secret secret, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteClientSecretAsync(string clientId, string secretId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}