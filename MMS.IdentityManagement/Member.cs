using System;

namespace MMS.IdentityManagement
{
    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset JoinedWhen { get; set; }
        public DateTimeOffset ExpiresWhen { get; set; }

        public SensitiveValue<string> EmailAddress { get; set; }
        public SensitiveValue<string> PhoneNumber { get; set; }
    }

    public class SensitiveValue<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public bool IsSpecified { get; set; }
        public bool CanRead { get; set; }
    }
}