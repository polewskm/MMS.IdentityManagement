using System;
using MMS.IdentityManagement.Api.Extensions;
using Xunit;

namespace MMS.IdentityManagement.Api.Test.Extensions
{
    public class DateTimeOffsetExtensionsTests
    {
        [Fact]
        public void TruncateMilliseconds_GivenNow_ThenNoMilliseconds()
        {
            var now = DateTimeOffset.Now;
            var value = now.TruncateMilliseconds();

            Assert.Equal(now.Date, value.Date);
            Assert.Equal(now.Second, value.Second);
            Assert.Equal(0, value.Millisecond);
        }

    }
}