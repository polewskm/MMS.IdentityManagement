namespace MMS.IdentityManagement.Api.SecretProtectors
{
    public class SecretProtectorBCrypt : ISecretProtector
    {
        public string CipherType => CipherTypes.BCrypt;

        public virtual string Protect(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public virtual bool Verify(string plainText, string cipherText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
        }

    }
}