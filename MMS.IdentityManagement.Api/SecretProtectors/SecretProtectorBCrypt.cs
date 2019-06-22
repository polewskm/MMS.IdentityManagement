using BCrypt.Net;

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
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
            }
            catch (SaltParseException)
            {
                return false;
            }
        }

    }
}