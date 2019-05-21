using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Models;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientValidator
    {
        Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default);
    }

    public class ClientValidator : IClientValidator
    {
        public virtual async Task<ClientValidationResult> ValidateClientAsync(ClientValidationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}