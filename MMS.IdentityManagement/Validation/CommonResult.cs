using System;
using Newtonsoft.Json;

namespace MMS.IdentityManagement.Validation
{
    public class CommonResult
    {
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _error;

        [JsonProperty("error_description", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
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

        [JsonIgnore]
        public Exception Exception { get; set; }
    }
}