using CubeServer.Data;
using System.ComponentModel.DataAnnotations;

namespace CubeServer.Models
{
    public enum ReportStatus
    {
        Pending,
        ManualReleased,
        AutoReleased,
        Sent
    }

    public class Report
    {
        [Required]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ReportType { get; set; } // Daily, Monthly, Statistical
        public string FileName { get; set; }
        public DateTime? ReportDate { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> EmailTo { get; set; }
        public List<string> EmailCC { get; set; }
        public byte Released { get; set; } // 0: Not released, 1:manual-released, 2:auto-released
        public string ReleaseUser { get; set; }
        public DateTime? ReleaseDate { get; set; }

        // derived
        public string ProjectCode;

        public static string ReleaseStr(int release)
        {
            foreach(ReportStatus rs in Enum.GetValues(typeof(ReportStatus)))
            {
                if (release == (int)rs) return Util.SplitCamelCase(rs.ToString());
            }

            return "None";
        }
    }
}
