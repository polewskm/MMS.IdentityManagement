using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default);

        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    }

    public class ClientService : IClientService
    {
        public virtual Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}