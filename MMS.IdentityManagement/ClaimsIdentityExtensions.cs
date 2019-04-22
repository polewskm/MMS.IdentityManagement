using System;
using System.Linq;
using System.Security.Claims;

namespace MMS.IdentityManagement
{
    public static class ClaimsIdentityExtensions
    {
        // https://github.com/dotnet/corefx/blob/master/src/System.Security.Claims/src/System/Security/Claims/ClaimsIdentity.cs
        // https://www.future-processing.pl/blog/introduction-to-claims-based-authentication-and-authorization-in-net/
        // https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1

        #region Parsing

        private static readonly TryParseDelegate<int> TryParseInt32 = (string input, out int result) => int.TryParse(input, out result);
        private static readonly TryParseDelegate<DateTimeOffset> TryParseDateTimeOffset = (string input, out DateTimeOffset result) => DateTimeOffset.TryParse(input, out result);

        private delegate bool TryParseDelegate<T>(string input, out T result)
            where T : struct;

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

            return ParseFirst(identity, MemberClaimTypes.MemberId, TryParseInt32);
        }

        public static string GetDisplayName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(MemberClaimTypes.DisplayName)?.Value;
        }

        public static string GetFirstName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(MemberClaimTypes.FirstName)?.Value;
        }

        public static string GetLastName(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(MemberClaimTypes.LastName)?.Value;
        }

        public static string GetEmailAddress(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindFirst(MemberClaimTypes.EmailAddress)?.Value;
        }

        public static DateTimeOffset? GetExpiration(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return ParseFirst(identity, MemberClaimTypes.Expiration, TryParseDateTimeOffset);
        }

        public static bool IsSystemAdministrator(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.HasClaim(MemberClaimTypes.Role, MemberRoles.SystemAdministrator);
        }

        public static bool IsBoardMember(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(MemberClaimTypes.BoardMember);
            return claim != null && !string.IsNullOrEmpty(claim.Value) && Enum.IsDefined(typeof(BoardMemberType), claim.Value);
        }

        public static BoardMemberType GetBoardMemberType(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(MemberClaimTypes.BoardMember);
            if (claim != null && !string.IsNullOrEmpty(claim.Value) && Enum.TryParse(claim.Value, out BoardMemberType type))
                return type;

            return BoardMemberType.None;
        }

        public static bool IsChampion(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            var claim = identity.FindFirst(MemberClaimTypes.Champion);
            return claim != null;
        }

        public static string[] GetChampionAreas(this ClaimsIdentity identity)
        {
            NullGuard(identity);

            return identity.FindAll(MemberClaimTypes.Champion).Select(c => c.Value).ToArray();
        }

    }
}