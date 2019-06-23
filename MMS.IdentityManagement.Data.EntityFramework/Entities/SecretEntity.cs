using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class SecretEntity<TTag>
        where TTag : TagEntity
    {
        public int Id { get; set; }

        public string SecretId { get; set; }

        public string CipherType { get; set; }

        public string CipherText { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }

        public List<TTag> Tags { get; set; }
    }
}