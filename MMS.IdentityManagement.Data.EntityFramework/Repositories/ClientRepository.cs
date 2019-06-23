using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMS.IdentityManagement.Requests;

namespace MMS.IdentityManagement.Data.EntityFramework.Repositories
{
    public class ClientRepository : IClientRepository
    {
        public virtual async Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            using (var context = new ConfigurationContext())
            {
                var results = await context.Clients
                    .Select(_ => new ClientReference { Id = _.ClientId })
                    .AsNoTracking()
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                return results;
            }
        }

        public virtual async Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            using (var context = new ConfigurationContext())
            {
                var result = await context.Clients
                    .Include(_ => _.Secrets)
                        .ThenInclude(_ => _.Tags)
                    .Include(_ => _.Tags)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(_ => _.ClientId == clientId, cancellationToken)
                    .ConfigureAwait(false);

                if (result == null)
                    return null;

                return new Client
                {
                    ClientId = result.ClientId,
                    Disabled = result.Disabled,
                    RequireSecret = result.RequireSecret,
                    CreatedWhen = result.CreatedWhen,
                    UpdatedWhen = result.UpdatedWhen,

                    Secrets = new HashSet<Secret>(result.Secrets.Select(secret => new Secret
                    {
                        SecretId = secret.SecretId,
                        CipherType = secret.CipherType,
                        CipherText = secret.CipherText,
                        CreatedWhen = secret.CreatedWhen,
                        UpdatedWhen = secret.UpdatedWhen,
                        Tags = secret.Tags.ToDictionary(tag => tag.Key, tag => tag.Value, StringComparer.OrdinalIgnoreCase),
                    })),
                    Tags = result.Tags.ToDictionary(tag => tag.Key, tag => tag.Value, StringComparer.OrdinalIgnoreCase),
                };
            }
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