namespace MMS.IdentityManagement.Validation
{
    public class ClientValidationResult : ValidationResult
    {
        public Client Client { get; set; }

        public Secret Secret { get; set; }
    }
}