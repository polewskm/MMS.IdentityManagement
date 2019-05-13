namespace MMS.IdentityManagement.Validation
{
    public class ValidationResult
    {
        public bool IsSuccess => string.IsNullOrEmpty(Error);

        public string Error { get; set; }

        public string ErrorDescription { get; set; }
    }
}