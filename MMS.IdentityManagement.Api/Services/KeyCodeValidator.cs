using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Models;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IKeyCodeValidator
    {
        Task<KeyCodeValidationResult> ValidateKeyCodeAsync(KeyCodeValidationRequest request, CancellationToken cancellationToken = default);
    }

    public class KeyCodeValidator : IKeyCodeValidator
    {
        public virtual async Task<KeyCodeValidationResult> ValidateKeyCodeAsync(KeyCodeValidationRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}