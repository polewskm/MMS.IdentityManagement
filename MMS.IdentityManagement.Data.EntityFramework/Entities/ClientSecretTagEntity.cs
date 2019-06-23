namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class ClientSecretTagEntity : TagEntity
    {
        public int ClientSecretId { get; set; }

        public ClientSecretEntity ClientSecret { get; set; }
    }
}