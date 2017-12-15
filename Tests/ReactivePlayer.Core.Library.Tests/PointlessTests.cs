using Xunit;

namespace ReactivePlayer.Core.Library.Tests
{
    public class PointlessTests
    {
        [Fact]
        void Pointless_test_with_many_lines_so_it_can_be_debugged_and_stepped()
        {
            var a = 12;
            var b = 4;

            Assert.True(a / b == 3);
        }
    }
}
