using System;

namespace MMS.IdentityManagement
{
    public class Member
    {
        public int MemberId { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset JoinedWhen { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }
    }
}