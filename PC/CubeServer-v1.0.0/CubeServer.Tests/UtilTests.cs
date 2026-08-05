using CubeServer.Data;
using Xunit;

namespace CubeServer.Tests
{
    public sealed class UtilTests
    {
        [Theory]
        [InlineData("127.0.0.1", true)]
        [InlineData("192.168.1.10", true)]
        [InlineData("256.1.1.1", false)]
        [InlineData("1.1.1", false)]
        [InlineData("abc.def.ghi.jkl", false)]
        public void ValidateIPv4Address_ReturnsExpected(string ip, bool expected)
        {
            Assert.Equal(expected, Util.ValidateIPv4Address(ip));
        }

        [Fact]
        public void EncryptPassword_MatchesSeededAdminPassword()
        {
            var hash = Util.EncryptPassword("Admin@123", 100);
            Assert.Equal("Jc6u3XG4ap4AyTJNe0eipGER2lCgFujskQ472nJy8ow=", hash);
        }

        [Theory]
        [InlineData(1.49, 1)]
        [InlineData(1.50, 2)]
        [InlineData(-1.49, -1)]
        [InlineData(-1.50, -2)]
        public void DoubleToInt_RoundsAsExpected(double input, int expected)
        {
            Assert.Equal(expected, Util.DoubleToInt(input));
        }

        [Theory]
        [InlineData("CubeTestReport", "Cube Test Report")]
        [InlineData("ABC", "A B C")]
        public void SplitCamelCase_InsertsSpaces(string input, string expected)
        {
            Assert.Equal(expected, Util.SplitCamelCase(input));
        }
    }
}
