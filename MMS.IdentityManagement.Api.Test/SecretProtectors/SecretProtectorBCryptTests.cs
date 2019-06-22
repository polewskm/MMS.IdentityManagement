using System;
using MMS.IdentityManagement.Api.SecretProtectors;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.SecretProtectors
{
    public class SecretProtectorBCryptTests
    {
        private static ISecretProtector Create() => new SecretProtectorBCrypt();

        [Fact]
        public void CipherType_IsValid()
        {
            var protector = Create();

            // ReSharper disable once StringLiteralTypo
            Assert.Equal("bcrypt", protector.CipherType);
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
            var protector = Create();

            var plainText = Guid.NewGuid().ToString("N");
            var cipherText = protector.Protect(plainText);
            var parts = cipherText.Split('$', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(3, parts.Length);

            var version = parts[0];
            var workFactor = parts[1];
            var saltAndHash = parts[2];
            Assert.Equal("2a", version);
            Assert.Equal("11", workFactor);
            Assert.Equal(53, saltAndHash.Length);
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