using CubeServer.Models;
using Xunit;

namespace CubeServer.Tests
{
    public sealed class ModelTests
    {
        [Theory]
        [InlineData(1, "Pass")]
        [InlineData(2, "Fail")]
        [InlineData(3, "Void")]
        [InlineData(0, "Not Tested")]
        public void Cube_TestResultStr_GetMapsCorrectly(int testResult, string expected)
        {
            var cube = new Cube { TestResult = testResult };
            Assert.Equal(expected, cube.TestResultStr);
        }

        [Theory]
        [InlineData("Pass", 1)]
        [InlineData("Fail", 2)]
        [InlineData("Void", 3)]
        public void Cube_TestResultStr_SetMapsCorrectly(string value, int expected)
        {
            var cube = new Cube();
            cube.TestResultStr = value;
            Assert.Equal(expected, cube.TestResult);
        }

        [Fact]
        public void CubeTest_CalcAvgStrength_SetsBatchAvgStrength()
        {
            var cubeTest = new CubeTest
            {
                batch = new Batch(),
                cubes = new List<Cube>
                {
                    new Cube { MeasuredStrength = 10 },
                    new Cube { MeasuredStrength = 20 },
                    new Cube { MeasuredStrength = 30 }
                }
            };

            cubeTest.CalcAvgStrength();

            Assert.Equal(20, cubeTest.batch.AvgStrength);
        }
    }
}
