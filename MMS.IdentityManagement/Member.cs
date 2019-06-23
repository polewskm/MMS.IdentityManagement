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

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public DateTimeOffset MembershipCreatedWhen { get; set; }

        public DateTimeOffset MembershipExpiresWhen { get; set; }

        public ICollection<string> Roles { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}