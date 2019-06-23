using System;

namespace MMS.IdentityManagement.Api.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset TruncateMilliseconds(this DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Offset);
        }

    }
}