using System;
using System.Threading;
using System.Threading.Tasks;

namespace MMS.IdentityManagement.Api.Services
{
    public interface ITokenService
    {
        Task<AuthenticationResult> AuthenticateKeyCodeAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken = default);
    }

    public class TokenService : ITokenService
    {
        private readonly ITokenConverter _tokenConverter;

        public TokenService(ITokenConverter tokenConverter)
        {
            _tokenConverter = tokenConverter ?? throw new ArgumentNullException(nameof(tokenConverter));
        }

        public virtual Task<AuthenticationResult> AuthenticateKeyCodeAsync(KeyCodeAuthenticationRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}