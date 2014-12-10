using System;
using Xunit;

namespace Driven.Tests
{
    public class sequential_guid_generator
    {
        [Fact]
        public void can_generate_sequential_guid()
        {
            var guid = SequentialGuid.NewGuid();
            Assert.NotEqual(Guid.Empty, guid);
        }
    }
}
