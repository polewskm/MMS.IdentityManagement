namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class ClientTagEntity : TagEntity
    {
        public int ClientId { get; set; }

        public ClientEntity Client { get; set; }
    }
}