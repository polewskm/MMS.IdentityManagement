using System;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Validation
{
    public class CommonResult
    {
        [JsonProperty("Error", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _error;

        [JsonProperty("ErrorDescription", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _errorDescription;

        [JsonIgnore]
        public bool Success => string.IsNullOrEmpty(Error);

        [JsonIgnore]
        public string Error
        {
            get => _error ?? Exception?.Message;
            set => _error = value;
        }

        [JsonIgnore]
        public string ErrorDescription
        {
            get => _errorDescription ?? Exception?.ToString();
            set => _errorDescription = value;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Exception Exception { get; set; }
    }
}