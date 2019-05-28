using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement.Requests
{
    public class UpdateClientRequest
    {
        public bool Disabled { get; set; }

        public bool RequireSecret { get; set; }

        public IDictionary<string, string> Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}