using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Principal;

namespace MMS.IdentityManagement
{
    public class MemberIdentity : ClaimsIdentity
    {
        public MemberIdentity()
        {
            // nothing
        }

        public MemberIdentity(IEnumerable<Claim> claims)
            : base(claims)
        {
            // nothing
        }

        public MemberIdentity(IEnumerable<Claim> claims, string authenticationType)
            : base(claims, authenticationType)
        {
            // nothing
        }

        public MemberIdentity(IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType)
            : base(claims, authenticationType, nameType, roleType)
        {
            // nothing
        }

        public MemberIdentity(BinaryReader reader)
            : base(reader)
        {
            // nothing
        }

        public MemberIdentity(IIdentity identity)
            : base(identity)
        {
            // nothing
        }

        public MemberIdentity(IIdentity identity, IEnumerable<Claim> claims)
            : base(identity, claims)
        {
            // nothing
        }

        public MemberIdentity(IIdentity identity, IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType)
            : base(identity, claims, authenticationType, nameType, roleType)
        {
            // nothing
        }

        public MemberIdentity(string authenticationType)
            : base(authenticationType)
        {
            // nothing
        }

        public MemberIdentity(string authenticationType, string nameType, string roleType)
            : base(authenticationType, nameType, roleType)
        {
            // nothing
        }

        protected MemberIdentity(ClaimsIdentity other)
            : base(other)
        {
            // nothing
        }

        protected MemberIdentity(SerializationInfo info)
            : base(info)
        {
            // nothing
        }

        protected MemberIdentity(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // nothing
        }

        // https://github.com/dotnet/corefx/blob/master/src/System.Security.Claims/src/System/Security/Claims/ClaimsIdentity.cs
        // https://www.future-processing.pl/blog/introduction-to-claims-based-authentication-and-authorization-in-net/
        // https://developer.okta.com/blog/2017/07/25/oidc-primer-part-1

        public int? MemberId => ParseFirst(MemberClaimTypes.MemberId, TryParseInt32);

        public string DisplayName => FindFirst(MemberClaimTypes.DisplayName)?.Value;

        public string FirstName => FindFirst(MemberClaimTypes.FirstName)?.Value;

        public string LastName => FindFirst(MemberClaimTypes.LastName)?.Value;

        public string EmailAddress => FindFirst(MemberClaimTypes.EmailAddress)?.Value;

        public DateTimeOffset? Expiration => ParseFirst(MemberClaimTypes.Expiration, TryParseDateTimeOffset);

        public bool IsBoardMember => HasClaim(MemberClaimTypes.Role, MemberRoles.BoardMember);

        public bool IsChampion => FindFirst(MemberClaimTypes.Champion) != null;

        public string[] Champion => FindAll(MemberClaimTypes.Champion).Select(c => c.Value).ToArray();

        #region Parsing

        private static readonly TryParseDelegate<int> TryParseInt32 = (string input, out int result) => int.TryParse(input, out result);
        private static readonly TryParseDelegate<DateTimeOffset> TryParseDateTimeOffset = (string input, out DateTimeOffset result) => DateTimeOffset.TryParse(input, out result);

        private delegate bool TryParseDelegate<T>(string input, out T result)
            where T : struct;

        private T? ParseFirst<T>(string claimType, TryParseDelegate<T> tryParse)
            where T : struct
        {
            var claim = FindFirst(claimType);
            if (claim != null && tryParse(claim.Value, out var result))
            {
                return result;
            }

            return null;
        }

        #endregion

        public override ClaimsIdentity Clone()
        {
            return new MemberIdentity(this);
        }

    }
}