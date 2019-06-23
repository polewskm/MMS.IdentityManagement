using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMS.IdentityManagement
{
    public class Secret
    {
        public string Id { get; set; }

        public string CipherType { get; set; }

        [JsonIgnore]
        public string CipherText { get; set; }

        public DateTimeOffset CreatedWhen { get; set; }

        public DateTimeOffset UpdatedWhen { get; set; }

        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}