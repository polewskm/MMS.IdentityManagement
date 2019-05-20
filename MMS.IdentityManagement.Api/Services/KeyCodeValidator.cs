using System;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IKeyCodeValidator
    {
        Task<KeyCodeValidationResult> ValidateKeyCodeAsync(string keyCode, CancellationToken cancellationToken = default);
    }

    public class KeyCodeValidator : IKeyCodeValidator
    {
        public virtual async Task<KeyCodeValidationResult> ValidateKeyCodeAsync(string keyCode, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}