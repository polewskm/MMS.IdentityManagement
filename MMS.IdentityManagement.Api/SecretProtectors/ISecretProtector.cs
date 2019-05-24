namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public interface ISecretProtector
    {
        string Protect(string plainText);

        bool Verify(string plainText, string cipherText);
    }
}