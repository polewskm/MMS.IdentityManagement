using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class ClientEntity
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }

        public List<ClientSecretEntity> Secrets { get; set; }

        public List<ClientTagEntity> Tags { get; set; }
    }
}