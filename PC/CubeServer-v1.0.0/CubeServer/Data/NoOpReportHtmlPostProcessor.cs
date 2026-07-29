using System.Threading;
using System.Threading.Tasks;

namespace CubeServer.Data
{
    public sealed class NoOpReportHtmlPostProcessor : IReportHtmlPostProcessor
    {
        public Task<string> ProcessAsync(ReportHtmlContext context, string html, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(html ?? string.Empty);
        }
    }
}
