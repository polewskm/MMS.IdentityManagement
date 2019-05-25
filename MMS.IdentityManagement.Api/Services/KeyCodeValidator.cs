using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMS.IdentityManagement.Api.Data;
using MMS.IdentityManagement.Api.Models;
using MMS.IdentityManagement.Api.SecretProtectors;
using MMS.IdentityManagement.Validation;

namespace MMS.IdentityManagement.Api.Services
{
    public interface IKeyCodeValidator
    {
        Task<KeyCodeValidationResult> ValidateKeyCodeAsync(KeyCodeValidationRequest request, CancellationToken cancellationToken = default);
    }

    public class KeyCodeValidator : IKeyCodeValidator
    {
        private static readonly IErrorFactory<KeyCodeValidationResult> ErrorFactory = ErrorFactory<KeyCodeValidationResult>.Instance;

        private readonly IMemberRepository _memberRepository;
        private readonly ISecretProtectorSelector _secretProtectorSelector;

        public KeyCodeValidator(IMemberRepository memberRepository, ISecretProtectorSelector secretProtectorSelector)
        {
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            _secretProtectorSelector = secretProtectorSelector ?? throw new ArgumentNullException(nameof(secretProtectorSelector));
        }

        public virtual async Task<KeyCodeValidationResult> ValidateKeyCodeAsync(KeyCodeValidationRequest request, CancellationToken cancellationToken = default)
        {
            // Since all keycodes are stored in the database using a password
            // hash (plus salt) cipher, we cannot use an optimized index. Instead
            // we must scan the entire collection and verify each cipher individually.

            var memberKeyCodes = await _memberRepository.LoadMemberKeyCodesAsync(cancellationToken).ConfigureAwait(false);

            var memberKeyCodeResult = memberKeyCodes.FirstOrDefault(
                memberKeyCode => _secretProtectorSelector
                    .Select(memberKeyCode.CipherType)
                    .Verify(request.KeyCode, memberKeyCode.CipherValue));

            if (memberKeyCodeResult == null)
                return ErrorFactory.InvalidGrant("Invalid KeyCode");

            var member = await _memberRepository.GetMemberByIdAsync(memberKeyCodeResult.MemberId, cancellationToken).ConfigureAwait(false);
            if (member == null)
                return ErrorFactory.InvalidGrant("Member missing");

            var result = new KeyCodeValidationResult
            {
                Member = member,
            };
            return result;
        }

    }
}