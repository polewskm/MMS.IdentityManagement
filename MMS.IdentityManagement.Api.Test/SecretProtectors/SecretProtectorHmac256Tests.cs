using System;
using System.Security.Cryptography;
using MMS.IdentityManagement.Api.SecretProtectors;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.SecretProtectors
{
    public class SecretProtectorHmac256Tests
    {
        private static ISecretProtector Create(Action<SecretProtectorHmacSha256Options> configure = null)
        {
            var options = new SecretProtectorHmacSha256Options();
            configure?.Invoke(options);

            if (options.Key == null)
            {
                options.Key = new byte[64];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(options.Key);
                }
            }

            var optionsAccessor = Microsoft.Extensions.Options.Options.Create(options);
            var protector = new SecretProtectorHmac256(optionsAccessor);

            return protector;
        }

        [Fact]
        public void CipherType_IsValid()
        {
            var protector = Create();

            Assert.Equal("hmac-sha256", protector.CipherType);
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
                options.SaltLength = 18;
            });

            var plainText = Guid.NewGuid().ToString("N");
            var cipherText = protector.Protect(plainText);

            var parts = cipherText.Split('$', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(3, parts.Length);

            var version = parts[0];
            var saltEncoded = parts[1];
            var hashEncoded = parts[2];

            Assert.Equal("hmac-sha256", version);

            var saltBytes = Convert.FromBase64String(saltEncoded);
            Assert.Equal(18, saltBytes.Length);

            var hashBytes = Convert.FromBase64String(hashEncoded);
            Assert.Equal(32, hashBytes.Length);
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