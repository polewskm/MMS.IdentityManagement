using System;

namespace MMS.IdentityManagement
{
    public class Secret
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset? ExpiresWhen { get; set; }
    }
}