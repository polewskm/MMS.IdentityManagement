using System;
using System.Collections.Generic;
using MMS.IdentityManagement.Api.SecretProtectors;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.SecretProtectors
{
    public class SecretProtectorSelectorTests
    {
        [Fact]
        public void Select_GivenGoodCipherType_ThenValid()
        {
            var protectors = new ISecretProtector[]
            {
                new TestSecretProtector("protector1"),
                new TestSecretProtector("protector2"),
                new TestSecretProtector("protector3"),
            };

            var selector = new SecretProtectorSelector(protectors);

            var protector = selector.Select("protector2");
            Assert.NotNull(protector);
            Assert.Equal("protector2", protector.CipherType);
        }

        [Fact]
        public void Select_GivenGoodCipherTypeDifferentCase_ThenValid()
        {
            var protectors = new ISecretProtector[]
            {
                new TestSecretProtector("protector1"),
                new TestSecretProtector("protector2"),
                new TestSecretProtector("protector3"),
            };

            var selector = new SecretProtectorSelector(protectors);

            // ReSharper disable once StringLiteralTypo
            var protector = selector.Select("PROtectOR2");
            Assert.NotNull(protector);
            Assert.Equal("protector2", protector.CipherType);
        }

        [Fact]
        public void Select_GivenBadCipherType_ThenException()
        {
            var protectors = new ISecretProtector[]
            {
                new TestSecretProtector("protector1"),
                new TestSecretProtector("protector2"),
                new TestSecretProtector("protector3"),
            };

            var selector = new SecretProtectorSelector(protectors);

            var cipherType = Guid.NewGuid().ToString();
            Assert.Throws<KeyNotFoundException>(() => selector.Select(cipherType));
        }

    }
}