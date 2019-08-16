using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MMS.IdentityManagement.Requests;

namespace MMS.IdentityManagement.Data
{
    // Microsoft.Extensions.Identity.Core
    // Microsoft.AspNetCore.Identity

    // Maker.Identity.Core
    // Maker.Identity.Stores
    // Maker.Identity.Api

    public class MakerUser
    {
        [PersonalData]
        public int SurrogateId { get; set; }

        public string Email { get; set; }
    }

    public class IdentityUser2 : IdentityUser
    {
    }

    public class ClientEntity
    {
        public int SurrogateId { get; set; }

        public string ClientId { get; set; }

        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }
    }

    public class SecretEntity
    {
        public int Id { get; set; }

        public string SecretId { get; set; }

        public string CipherType { get; set; }

        public string CipherText { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }
    }

    public class ClientSecretEntity : SecretEntity
    {
        public int ClientId { get; set; }

        public ClientEntity Client { get; set; }
    }

    public interface IClientStore
    {
        Task<ClientEntity> FindByIdAsync(string clientId, CancellationToken cancellationToken);

        Task CreateAsync(ClientEntity client, CancellationToken cancellationToken);

        Task UpdateAsync(ClientEntity client, CancellationToken cancellationToken);

        Task DeleteAsync(string clientId, CancellationToken cancellationToken);

        //

        Task<IEnumerable<ClientSecretEntity>> GetSecretsAsync(ClientEntity client, CancellationToken cancellationToken);

        Task AddSecretAsync(Client client, string secretId, CancellationToken cancellationToken);
    }

    public interface ISecretStore
    {
    }

    public interface IClientRepository
    {
        Task<IEnumerable<ClientReference>> GetClientsAsync(CancellationToken cancellationToken = default);

        Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken = default);

        Task CreateClientAsync(Client client, CancellationToken cancellationToken = default);

        Task UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default);

        Task CreateClientSecretAsync(string clientId, Secret secret, CancellationToken cancellationToken = default);

        Task DeleteClientSecretAsync(string clientId, string secretId, CancellationToken cancellationToken = default);
    }
}