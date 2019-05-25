namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public interface ISecretProtector
    {
        string CipherType { get; }

        string Protect(string plainText);

        bool Verify(string plainText, string cipherText);
    }
}