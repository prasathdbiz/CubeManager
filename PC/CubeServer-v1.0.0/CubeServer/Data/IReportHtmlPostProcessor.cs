using System.Threading;
using System.Threading.Tasks;

namespace CubeServer.Data
{
    public interface IReportHtmlPostProcessor
    {
        Task<string> ProcessAsync(ReportHtmlContext context, string html, CancellationToken cancellationToken = default);
    }
}
