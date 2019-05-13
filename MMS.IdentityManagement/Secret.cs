using System;

namespace MMS.IdentityManagement
{
    public class Secret
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Expiration { get; set; }
    }
}