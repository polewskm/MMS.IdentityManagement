using System;
using System.Linq;
using System.Security.Claims;

namespace MMS.IdentityManagement.Claims
{
    public static class ClaimsIdentityExtensions
    {
        // https://github.com/dotnet/corefx/blob/master/src/System.Security.Claims/src/System/Security/Claims/ClaimsIdentity.cs
        // https://www.future-processing.pl/blog/introduction-to-claims-based-authentication-and-authorization-in-net/
        // https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1

        #region Parsing

        private delegate bool TryParseDelegate<T>(string input, out T result) where T : struct;

        private static readonly TryParseDelegate<int> TryParseInt32 = (string input, out int result) => int.TryParse(input, out result);
        private static readonly TryParseDelegate<DateTimeOffset> TryParseDateTimeOffset = (string input, out DateTimeOffset result) => DateTimeOffset.TryParse(input, out result);
        private static readonly TryParseDelegate<DateTimeOffset> TryParseUnixTimeSeconds = (string input, out DateTimeOffset result) =>
        {
            if (long.TryParse(input, out var seconds))
            {
                result = DateTimeOffset.FromUnixTimeSeconds(seconds);
                return true;
            }
            result = DateTimeOffset.MinValue;
            return false;
        };

        private static T? ParseFirst<T>(ClaimsIdentity identity, string claimType, TryParseDelegate<T> tryParse)
            where T : struct
        {
            var claim = identity.FindFirst(claimType);
            if (claim != null && tryParse(claim.Value, out var result))
            {
                return result;
            }

            return null;
        }

        #endregion

        private static void NullGuard(ClaimsIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
        }

        public static int? GetMemberId(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return ParseFirst(identity, IdentityClaimTypes.MemberId, TryParseInt32);
        }

        public static string GetDisplayName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(IdentityClaimTypes.DisplayName)?.Value;
        }

        public static string GetFirstName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(IdentityClaimTypes.FirstName)?.Value;
        }

        public static string GetLastName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(IdentityClaimTypes.LastName)?.Value;
        }

        public static string GetEmailAddress(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(IdentityClaimTypes.EmailAddress)?.Value;
        }

        public static string GetPhoneNumber(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(IdentityClaimTypes.PhoneNumber)?.Value;
        }

        public static DateTimeOffset? GetMemberSince(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return ParseFirst(identity, IdentityClaimTypes.MemberSince, TryParseUnixTimeSeconds);
        }

        public static DateTimeOffset? GetRenewalDue(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return ParseFirst(identity, IdentityClaimTypes.RenewalDue, TryParseUnixTimeSeconds);
        }

        public static bool IsSystemAdministrator(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.HasClaim(IdentityClaimTypes.Role, MemberRoles.SystemAdministrator);
        }

        public static bool IsBoardMember(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(IdentityClaimTypes.BoardMemberType);
            return claim != null && !string.IsNullOrEmpty(claim.Value) && Enum.IsDefined(typeof(BoardMemberType), claim.Value);
        }

        public static BoardMemberType GetBoardMemberType(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(IdentityClaimTypes.BoardMemberType);
            if (claim != null && !string.IsNullOrEmpty(claim.Value) && Enum.TryParse(claim.Value, out BoardMemberType type))
                return type;

            return BoardMemberType.None;
        }

        public static bool IsChampion(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(IdentityClaimTypes.ChampionArea);
            return claim != null;
        }

        public static string[] GetChampionAreas(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindAll(IdentityClaimTypes.ChampionArea).Select(c => c.Value).ToArray();
        }

    }
}