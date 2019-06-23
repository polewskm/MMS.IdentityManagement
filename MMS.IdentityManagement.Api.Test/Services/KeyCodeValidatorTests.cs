using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Data;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.SecretProtectors;
using MMS.IdentityManagement.Api.Services;
using Moq;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Services
{
    public class KeyCodeValidatorTests
    {
        [Fact]
        public async Task ValidateKeyCodeAsync_GivenNoMembers_ThenInvalidGrant()
        {
            var mockMemberRepository = new Mock<IMemberRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);

            var cancellationToken = CancellationToken.None;
            var keyCodeValidator = new KeyCodeValidator(mockMemberRepository.Object, mockSecretProtectorSelector.Object);

            mockMemberRepository
                .Setup(_ => _.LoadMemberKeyCodesAsync(cancellationToken))
                .ReturnsAsync(Enumerable.Empty<MemberKeyCode>())
                .Verifiable();

            var request = new KeyCodeValidationRequest
            {
                KeyCode = Guid.NewGuid().ToString("N"),
            };

            var result = await keyCodeValidator.ValidateKeyCodeAsync(request, cancellationToken).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("invalid_grant", result.Error);
            Assert.Equal("Invalid KeyCode", result.ErrorDescription);
            Assert.Null(result.Member);

            mockMemberRepository.Verify();
        }

        [Fact]
        public async Task ValidateKeyCodeAsync_GivenInvalidKeyCode_ThenInvalidGrant()
        {
            var mockMemberRepository = new Mock<IMemberRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);
            var mockSecretProtector = new Mock<ISecretProtector>(MockBehavior.Strict);

            var cancellationToken = CancellationToken.None;
            var keyCodeValidator = new KeyCodeValidator(mockMemberRepository.Object, mockSecretProtectorSelector.Object);

            var now = DateTimeOffset.Now;
            const string cipherType = "test_cipher";

            var keyCode = Guid.NewGuid().ToString("N");
            var memberKeyCode = new MemberKeyCode
            {
                MemberId = 1,
                Secret = new Secret
                {
                    SecretId = "test",
                    CipherType = cipherType,
                    CipherText = Guid.NewGuid().ToString("N"),
                    CreatedWhen = now,
                    UpdatedWhen = now,
                }
            };

            mockMemberRepository
                .Setup(_ => _.LoadMemberKeyCodesAsync(cancellationToken))
                .ReturnsAsync(new[] { memberKeyCode })
                .Verifiable();

            mockSecretProtectorSelector
                .Setup(_ => _.Select(cipherType))
                .Returns(mockSecretProtector.Object)
                .Verifiable();

            mockSecretProtector
                .Setup(_ => _.Verify(keyCode, memberKeyCode.Secret.CipherText))
                .Returns(false)
                .Verifiable();

            var request = new KeyCodeValidationRequest
            {
                KeyCode = keyCode,
            };

            var result = await keyCodeValidator.ValidateKeyCodeAsync(request, cancellationToken).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("invalid_grant", result.Error);
            Assert.Equal("Invalid KeyCode", result.ErrorDescription);
            Assert.Null(result.Member);

            mockMemberRepository.Verify();
            mockSecretProtectorSelector.Verify();
            mockSecretProtector.Verify();
        }

        [Fact]
        public async Task ValidateKeyCodeAsync_GivenMissingMember_ThenInvalidGrant()
        {
            var mockMemberRepository = new Mock<IMemberRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);
            var mockSecretProtector = new Mock<ISecretProtector>(MockBehavior.Strict);

            var cancellationToken = CancellationToken.None;
            var keyCodeValidator = new KeyCodeValidator(mockMemberRepository.Object, mockSecretProtectorSelector.Object);

            var now = DateTimeOffset.Now;
            const string cipherType = "test_cipher";

            const int memberId = 1;
            var keyCode = Guid.NewGuid().ToString("N");
            var memberKeyCode = new MemberKeyCode
            {
                MemberId = memberId,
                Secret = new Secret
                {
                    SecretId = "test",
                    CipherType = cipherType,
                    CipherText = Guid.NewGuid().ToString("N"),
                    CreatedWhen = now,
                    UpdatedWhen = now,
                }
            };

            mockMemberRepository
                .Setup(_ => _.LoadMemberKeyCodesAsync(cancellationToken))
                .ReturnsAsync(new[] { memberKeyCode })
                .Verifiable();

            mockMemberRepository
                .Setup(_ => _.GetMemberByIdAsync(memberId, cancellationToken))
                .ReturnsAsync((Member)null)
                .Verifiable();

            mockSecretProtectorSelector
                .Setup(_ => _.Select(cipherType))
                .Returns(mockSecretProtector.Object)
                .Verifiable();

            mockSecretProtector
                .Setup(_ => _.Verify(keyCode, memberKeyCode.Secret.CipherText))
                .Returns(true)
                .Verifiable();

            var request = new KeyCodeValidationRequest
            {
                KeyCode = keyCode,
            };

            var result = await keyCodeValidator.ValidateKeyCodeAsync(request, cancellationToken).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("invalid_grant", result.Error);
            Assert.Equal("Member missing", result.ErrorDescription);
            Assert.Null(result.Member);

            mockMemberRepository.Verify();
            mockSecretProtectorSelector.Verify();
            mockSecretProtector.Verify();
        }

        [Fact]
        public async Task ValidateKeyCodeAsync_GivenValidKeyCode_ThenKnownMember()
        {
            var mockMemberRepository = new Mock<IMemberRepository>(MockBehavior.Strict);
            var mockSecretProtectorSelector = new Mock<ISecretProtectorSelector>(MockBehavior.Strict);
            var mockSecretProtector = new Mock<ISecretProtector>(MockBehavior.Strict);

            var cancellationToken = CancellationToken.None;
            var keyCodeValidator = new KeyCodeValidator(mockMemberRepository.Object, mockSecretProtectorSelector.Object);

            var now = DateTimeOffset.Now;
            const string cipherType = "test_cipher";

            const int memberId = 1;
            var keyCode = Guid.NewGuid().ToString("N");
            var memberKeyCode = new MemberKeyCode
            {
                MemberId = memberId,
                Secret = new Secret
                {
                    SecretId = "test",
                    CipherType = cipherType,
                    CipherText = Guid.NewGuid().ToString("N"),
                    CreatedWhen = now,
                    UpdatedWhen = now,
                }
            };
            var member = new Member
            {
                MemberId = memberId,
            };

            mockMemberRepository
                .Setup(_ => _.LoadMemberKeyCodesAsync(cancellationToken))
                .ReturnsAsync(new[] { memberKeyCode })
                .Verifiable();

            mockMemberRepository
                .Setup(_ => _.GetMemberByIdAsync(memberId, cancellationToken))
                .ReturnsAsync(member)
                .Verifiable();

            mockSecretProtectorSelector
                .Setup(_ => _.Select(cipherType))
                .Returns(mockSecretProtector.Object)
                .Verifiable();

            mockSecretProtector
                .Setup(_ => _.Verify(keyCode, memberKeyCode.Secret.CipherText))
                .Returns(true)
                .Verifiable();

            var request = new KeyCodeValidationRequest
            {
                KeyCode = keyCode,
            };

            var result = await keyCodeValidator.ValidateKeyCodeAsync(request, cancellationToken).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Null(result.ErrorDescription);
            Assert.Same(member, result.Member);

            mockMemberRepository.Verify();
            mockSecretProtectorSelector.Verify();
            mockSecretProtector.Verify();
        }

    }
}