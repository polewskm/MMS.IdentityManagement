using System;

namespace MMS.IdentityManagement
{
    public class Secret
    {
        public string Id { get; set; }

        public string CipherType { get; set; }

        public string CipherValue { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset? ExpiresWhen { get; set; }
    }
}
