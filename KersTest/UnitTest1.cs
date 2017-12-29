using System;
using Xunit;
using KersDb;

namespace KersTest
{
    public class KersDbTests
    {
        [Fact]
        public void TestThing() {
            Assert.Equal(42, new Thing().Get(19, 23));
        }
    }
}
