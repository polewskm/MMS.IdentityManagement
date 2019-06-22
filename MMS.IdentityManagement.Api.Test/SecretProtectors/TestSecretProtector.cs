using MMS.IdentityManagement.Api.SecretProtectors;

namespace MMS.IdentityManagement.Api.Test.SecretProtectors
{
    public class TestSecretProtector : ISecretProtector
    {
        public TestSecretProtector(string cipherType)
        {
            CipherType = cipherType;
        }

        public string CipherType { get; }

        public string Protect(string plainText)
        {
            throw new System.NotImplementedException();
        }

        public bool Verify(string plainText, string cipherText)
        {
            throw new System.NotImplementedException();
        }

    }
}