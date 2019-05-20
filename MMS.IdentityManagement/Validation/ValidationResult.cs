using System;

namespace MMS.IdentityManagement.Validation
{
    public class ValidationResult
    {
        private string _error;
        private string _errorDescription;

        public bool IsValid => string.IsNullOrEmpty(Error);

        public string Error
        {
            get => _error ?? Exception?.Message;
            set => _error = value;
        }

        public string ErrorDescription
        {
            get => _errorDescription ?? Exception?.ToString();
            set => _errorDescription = value;
        }

        public Exception Exception { get; set; }
    }
}