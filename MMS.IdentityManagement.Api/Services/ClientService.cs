using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientService
    {
        Task<ClientValidationResult> ValidateClientAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default);
    }

    public class ClientService : IClientService
    {
        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientRequest clientRequest, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

    }
}