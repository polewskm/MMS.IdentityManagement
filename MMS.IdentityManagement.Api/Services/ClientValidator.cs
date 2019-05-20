using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IClientValidator
    {
        Task<ClientValidationResult> ValidateClientAsync(string clientId, string clientSecret, CancellationToken cancellationToken = default);
    }

    public class ClientValidator : IClientValidator
    {
        public virtual async Task<ClientValidationResult> ValidateClientAsync(string clientId, string clientSecret, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}