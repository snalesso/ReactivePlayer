using Xunit;

namespace ReactivePlayer.Core.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(4, 0)]
        [InlineData(0, 4)]
        [InlineData(-2, 6)]
        [InlineData(6, -2)]
        public void PassingTest(int x, int y)
        {
            Assert.Equal(Add(x, y), 4);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.NotEqual(Add(2, 2), 5);
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}