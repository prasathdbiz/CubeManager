using CubeServer.Data;
using Xunit;

namespace CubeServer.Tests
{
    public sealed class NoOpReportHtmlPostProcessorTests
    {
        [Fact]
        public async Task ProcessAsync_ReturnsEmptyStringWhenNull()
        {
            var processor = new NoOpReportHtmlPostProcessor();
            var result = await processor.ProcessAsync(new ReportHtmlContext(), null);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ProcessAsync_ReturnsSameHtml()
        {
            var processor = new NoOpReportHtmlPostProcessor();
            var html = "<html><body>Hello</body></html>";
            var result = await processor.ProcessAsync(new ReportHtmlContext(), html);
            Assert.Equal(html, result);
        }
    }
}
