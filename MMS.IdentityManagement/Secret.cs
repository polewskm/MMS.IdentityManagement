using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement
{
    public class Secret
    {
        public string Id { get; set; }

        public string CipherType { get; set; }

        public string CipherText { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }

        public IDictionary<string, string> Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}