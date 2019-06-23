using System;

namespace MMS.IdentityManagement.Data.EntityFramework.Entities
{
    public class TagEntity
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }
    }
}