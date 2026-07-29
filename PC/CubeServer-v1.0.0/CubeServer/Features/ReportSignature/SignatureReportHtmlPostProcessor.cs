using CubeServer.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CubeServer.Features.ReportSignature
{
    public sealed class SignatureReportHtmlPostProcessor : IReportHtmlPostProcessor
    {
        readonly IReportSignatureStore store;

        public SignatureReportHtmlPostProcessor(IReportSignatureStore store)
        {
            this.store = store;
        }

        public async Task<string> ProcessAsync(ReportHtmlContext context, string html, CancellationToken cancellationToken = default)
        {
            if (context == null || string.IsNullOrWhiteSpace(context.UserId))
            {
                return html;
            }

            if (!string.Equals(context.ReportType, "Daily", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(context.ReportType, "Monthly", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(context.ReportType, "Statistical", StringComparison.OrdinalIgnoreCase))
            {
                return html;
            }

            string signature = await store.GetSignatureDataUrlAsync(context.UserId, cancellationToken);
            if (string.IsNullOrWhiteSpace(signature))
            {
                return html;
            }

            string style = "<style>.report-signature{margin-top:40px;text-align:right;page-break-inside:avoid;}.report-signature img{max-width:180px;max-height:80px;object-fit:contain;}.signature-label{font-size:12px;margin-bottom:6px;color:#555;}</style>";
            html = InsertBeforeClosingTag(html, "head", style);

            string signatureHtml =
                "<div class=\"report-signature\">" +
                "<img src=\"" + signature + "\" alt=\"Signature\" />" +
                "</div>";

            html = InsertBeforeClosingTag(html, "body", signatureHtml);
            return html;
        }

        static string InsertBeforeClosingTag(string html, string tagName, string snippet)
        {
            if (string.IsNullOrWhiteSpace(html) || string.IsNullOrWhiteSpace(snippet) || string.IsNullOrWhiteSpace(tagName))
            {
                return html;
            }

            string close = "</" + tagName + ">";
            int idx = html.LastIndexOf(close, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                return html;
            }

            return html.Substring(0, idx) + snippet + html.Substring(idx);
        }
    }
}
