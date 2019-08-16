using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Validation
{
    public class CommonResult
    {
        [JsonIgnore]
        public bool Success => string.IsNullOrEmpty(Error);

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty("error_description", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorDescription { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
    }
}