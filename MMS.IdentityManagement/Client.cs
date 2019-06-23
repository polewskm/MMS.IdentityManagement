using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMS.IdentityManagement
{
    public class Client
    {
        public string Id { get; set; }

        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }

        [JsonIgnore]
        public ICollection<Secret> Secrets { get; set; } = new HashSet<Secret>();

        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}