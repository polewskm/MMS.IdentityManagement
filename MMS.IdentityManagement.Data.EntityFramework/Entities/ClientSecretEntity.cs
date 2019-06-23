namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class ClientSecretEntity : SecretEntity<ClientSecretTagEntity>
    {
        public int ClientId { get; set; }

        public ClientEntity Client { get; set; }
    }
}