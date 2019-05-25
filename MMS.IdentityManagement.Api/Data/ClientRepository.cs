using System;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.IdentityManagement.Api.Data
{
    public interface IClientRepository
    {
        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);
    }

    public class ClientRepository : IClientRepository
    {
        public virtual Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}