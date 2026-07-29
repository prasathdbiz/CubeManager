using System;

namespace CubeServer.Data
{
    public sealed class ReportHtmlContext
    {
        public string ReportType { get; init; }
        public int ProjectId { get; init; }
        public string ProjectCode { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string UserId { get; init; }
        public string HtmlFileName { get; init; }
    }
}
