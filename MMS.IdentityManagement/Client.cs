using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement
{
    public class Client
    {
        public string Id { get; set; }

        public string Name
        {
            get => Tags[nameof(Name)];
            set => Tags[nameof(Name)] = value;
        }

        public string Description
        {
            get => Tags[nameof(Description)];
            set => Tags[nameof(Description)] = value;
        }

        public bool Disabled { get; set; }

        public DateTimeOffset Registered { get; set; }

        public DateTimeOffset? Expiration { get; set; }

        public bool RequireSecret { get; set; } = true;

        public ICollection<Secret> Secrets { get; set; } = new HashSet<Secret>();

        public IDictionary<string, string> Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}