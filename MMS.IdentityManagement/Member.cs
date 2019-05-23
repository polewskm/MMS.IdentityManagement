using System;
using System.Collections.Generic;

namespace MMS.IdentityManagement
{
    public class Member
    {
        public int MemberId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public DateTimeOffset MemberSince { get; set; }

        public DateTimeOffset RenewalDue { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public BoardMemberType BoardMemberType { get; set; }

        public ICollection<string> ChampionAreas { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ICollection<string> Roles { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}