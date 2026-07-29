namespace CubeServer.Models
{
    public class BatchReport
    {
        public Batch batch;
        public List<Cube> cubes = new List<Cube>();

        public void CalcAvgStrength()
        {
            double avg = 0;

            foreach (Cube c in cubes)
            {
                avg += c.MeasuredStrength;
            }

            batch.AvgStrength = avg / cubes.Count;
        }
    }

    public class CubeSetReport
    {
        public CubeSet cubeSet;
        public List<BatchReport> batchReports = new List<BatchReport>();
    }

    public class SectionReport
    {
        public int scoNum;
        public string supplier;
        public string specification;
        public string testStandard;
        public int concreteGrade;
        public int age;
        public int cubeCnt;
        public string plotImage;

        public List<CubeSetReport> cubeSetReports = new List<CubeSetReport>();

    }

    public partial class DailyReport
    {
        public DateTime testDate;
        public string projectId;
        public string projectName;
        public string companyName;
        public string companyAddress;
        public string attnTo;

        public List<SectionReport> sections = new List<SectionReport>();
    }

    public class ReportData
    {
        public DateTime startDate;
        public DateTime endDate;
        public string projectId;
        public string projectName;
        public string companyName;
        public string companyAddress;
        public string attnTo;

        public List<SectionReport> sections = new List<SectionReport>();
    }

    public class CostSummaryReport
    {
        public DateTime startDate;
        public DateTime endDate;
        public SortedDictionary<string, List<ProjectReport>> clientProjs = 
            new SortedDictionary<string, List<ProjectReport>>();

    }

    public class ProjectReport
    {
        public Project project;
        public List<CubeSet> cubeSets = new List<CubeSet>();
        public List<Batch> batches = new List<Batch>();
        public List<Cube> cubes = new List<Cube>();
    }
}
