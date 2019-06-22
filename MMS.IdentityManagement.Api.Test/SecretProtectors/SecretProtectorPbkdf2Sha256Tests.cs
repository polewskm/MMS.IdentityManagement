using System;
using MMS.IdentityManagement.Api.SecretProtectors;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.SecretProtectors
{
    public class SecretProtectorPbkdf2Sha256Tests
    {
        private static ISecretProtector Create(Action<SecretProtectorPbkdf2Sha256Options> configure = null)
        {
            var options = new SecretProtectorPbkdf2Sha256Options();
            configure?.Invoke(options);

            var optionsAccessor = Microsoft.Extensions.Options.Options.Create(options);
            var protector = new SecretProtectorPbkdf2Sha256(optionsAccessor);

            return protector;
        }

        [Fact]
        public void CipherType_IsValid()
        {
            var protector = Create();

            Assert.Equal("pbkdf2-sha256", protector.CipherType);
        }

        [Fact]
        public void Protect_GivenSamePlainTextTwice_ThenDifferentCipherTexts()
        {
            var protector = Create();

            var plainText = Guid.NewGuid().ToString("N");

            var cipherText1 = protector.Protect(plainText);
            Assert.NotNull(cipherText1);

            var cipherText2 = protector.Protect(plainText);
            Assert.NotNull(cipherText2);

            Assert.NotEqual(cipherText1, cipherText2);
        }

        [Fact]
        public void Protect_GivenPlainText_ThenPartsAreValid()
        {
            var protector = Create(options =>
            {
                options.IterationCount = 10001;
                options.SaltLength = 18;
                options.OutputLength = 34;
            });

            var plainText = Guid.NewGuid().ToString("N");
            var cipherText = protector.Protect(plainText);

            var parts = cipherText.Split('$', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(4, parts.Length);

            var version = parts[0];
            var parameters = parts[1];
            var saltEncoded = parts[2];
            var hashEncoded = parts[3];

            Assert.Equal("pbkdf2-sha256", version);
            Assert.Equal("t=10001", parameters);

            var saltBytes = Convert.FromBase64String(saltEncoded);
            Assert.Equal(18, saltBytes.Length);

            var hashBytes = Convert.FromBase64String(hashEncoded);
            Assert.Equal(34, hashBytes.Length);
        }

        [Fact]
        public void Verify_GivenGoodCipherText_ThenValid()
        {
            var protector = Create();

            var plainText = Guid.NewGuid().ToString("N");
            var cipherText = protector.Protect(plainText);

            var result = protector.Verify(plainText, cipherText);
            Assert.True(result);
        }

        [Fact]
        public void Verify_GivenBadCipherText_ThenInvalid()
        {
            var protector = Create();

            var plainText = Guid.NewGuid().ToString("N");
            var cipherText = Guid.NewGuid().ToString("N");

            var result = protector.Verify(plainText, cipherText);
            Assert.False(result);
        }

    }
}