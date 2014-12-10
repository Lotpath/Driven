using System;
using Xunit;

namespace Driven.Tests
{
    public class system_clock
    {
        [Fact]
        public void can_be_set_to_a_utc_offset()
        {
            Assert.DoesNotThrow(() =>
                {
                    SystemClock.UtcNowFunc = () => DateTimeOffset.UtcNow.AddDays(-7);
                });
        }

        [Fact]
        public void cannot_be_set_to_non_utc_offset()
        {
            Assert.Throws<InvalidOperationException>(() =>
                {
                    SystemClock.UtcNowFunc = () => DateTimeOffset.Now.AddDays(-7);
                });
        }
    }
}