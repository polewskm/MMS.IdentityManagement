using System;
using System.Collections.Generic;
using System.Linq;

namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public interface ISecretProtectorSelector
    {
        ISecretProtector Select(string cipherType);
    }

    public class SecretProtectorSelector : ISecretProtectorSelector
    {
        private readonly IReadOnlyDictionary<string, ISecretProtector> _protectors;

        public SecretProtectorSelector(IEnumerable<ISecretProtector> protectors)
        {
            if (protectors == null)
                throw new ArgumentNullException(nameof(protectors));

            _protectors = protectors.ToDictionary(
                protector => protector.CipherType,
                protector => protector,
                StringComparer.OrdinalIgnoreCase);
        }

        public virtual ISecretProtector Select(string cipherType)
        {
            if (_protectors.TryGetValue(cipherType, out var protector))
                return protector;

            throw new KeyNotFoundException();
        }

    }
}