using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CubeServer.Features.ReportSignature
{
    public interface IReportSignatureStore
    {
        Task<string> GetSignatureDataUrlAsync(string userId, CancellationToken cancellationToken = default);
        Task<Uri> GetSignatureFileUriAsync(string userId, CancellationToken cancellationToken = default);
        Task SaveAsync(string userId, IBrowserFile file, CancellationToken cancellationToken = default);
        Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
    }
}
