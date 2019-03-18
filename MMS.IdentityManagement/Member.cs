using System;

namespace MMS.IdentityManagement
{
    public class Member
    {
        public int MemberId { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public DateTimeOffset JoinedWhen { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }
    }
}