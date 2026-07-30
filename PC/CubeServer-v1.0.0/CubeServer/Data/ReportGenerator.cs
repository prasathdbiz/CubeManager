using CubeServer.Models;
using Weasyprint.Wrapped;
using Scriban;
using ScottPlot;
using Plotly.Blazor.LayoutLib.AnnotationLib;
using System.Drawing;
using static System.Collections.Specialized.BitVector32;
using CubeServer.Pages;
using Microsoft.Extensions.DependencyInjection;


namespace CubeServer.Data
{
    public class YData
    {
        public double Y1;
        public double Y2;
    }

    public class ReportResult
    {
        public string pdffile;
        public int resultCnt;
    }

    public class ReportState
    {
        public Project project;
        public string reportType;
        public DateTime? date1;
        public DateTime? date2;
        public int testAge;
        public int? filterGrade;
        public string filterSupplierId;
        public double? filterDimension;
    }

    public class ReportGenerator
    {
        Printer printer;
        Random rand = new Random();

        public ReportGenerator()
        {
            printer = new Printer();
            printer.Initialize();
        }

        async Task<string> PostProcessHtmlAsync(ReportHtmlContext context, string html)
        {
            IServiceProvider sp = Global.Services;
            if (sp == null)
            {
                return html;
            }

            var processors = sp.GetServices<IReportHtmlPostProcessor>();
            if (processors == null)
            {
                return html;
            }

            string current = html;
            foreach (var p in processors)
            {
                if (p == null) continue;
                current = await p.ProcessAsync(context, current);
            }

            return current;
        }

        public string GenerateChart(SortedDictionary<DateOnly, YData> data)
        {
            var plt = new ScottPlot.Plot(1000, 500);

            int size = data.Count;
            DateTime[] dates = new DateTime[size];
            double[] y1 = new double[size];
            double[] y2 = new double[size];

            int i = 0;
            double ymax = 0;
            foreach(KeyValuePair<DateOnly, YData> pair in data)
            {
                dates[i] = pair.Key.ToDateTime(new TimeOnly(0,0,0));
                y1[i] = pair.Value.Y1;
                y2[i] = pair.Value.Y2 > 0 ? pair.Value.Y2 : double.NaN;

                if (y1[i] > ymax) ymax = y1[i];
                if (y2[i] > ymax) ymax = y2[i];

                i++;
            }

            double[] x = dates.Select(x => x.ToOADate()).ToArray();

            plt.XAxis.TickLabelFormat("yyyy-MM-dd", dateTimeFormat: true);
            plt.XAxis.TickLabelStyle(rotation: 45);

            // plot the data
            var scatter1 = plt.AddScatter(x, y1, label: "Avg Strength");
            var scatter2 = plt.AddScatter(x, y2, label: "Rolling Avg Strength");
            scatter2.OnNaN = ScottPlot.Plottable.ScatterPlot.NanBehavior.Ignore;
            plt.XAxis.DateTimeFormat(true);
            plt.SetAxisLimitsY(0, Math.Round(ymax * 1.5));

            // customize the axis labels
            plt.Title("Cube Compressive Strength");
            plt.XAxis.Label("Test Date", bold: true, size: 14);
            plt.YAxis.Label("Strength (N/mm2)", bold: true, size: 14);

            var legend = plt.Legend(enable:false);
            legend.Orientation = Orientation.Horizontal;
            //legend.Location = Alignment.LowerRight;

            Bitmap bmpPlot = plt.GetBitmap();
            Bitmap bmpLegend = plt.RenderLegend();

            // create a large image and place the plot and legend images onto it
            Bitmap bmp = new Bitmap(bmpPlot.Width, bmpPlot.Height + bmpLegend.Height);
            using Graphics gfx = Graphics.FromImage(bmp);
            gfx.Clear(Color.White);
            gfx.DrawImage(bmpPlot, 0, 0);
            gfx.DrawImage(bmpLegend, 7*bmpPlot.Width/10, bmpPlot.Height);

            string file = Path.Combine(Global.htmlPath, Guid.NewGuid().ToString() + ".png");
            //plt.SaveFig(file, 1000, 500);
            bmp.Save(file);

            return file;
        }

        public void ExtractPlotData(SectionReport section, SortedDictionary<DateOnly, YData> plotData)
        {
            plotData.Clear();

            // find average strength for batches on same dates
            foreach (CubeSetReport cs in section.cubeSetReports)
            {
                foreach (BatchReport b in cs.batchReports)
                {
                    DateOnly date;
                    if (b.cubes[0].ActualTestDate.HasValue)
                    {
                        date = DateOnly.FromDateTime((DateTime) b.cubes[0].ActualTestDate);
                    }
                    else
                    {
                        continue;
                    }

                    if (plotData.ContainsKey(date))
                    {
                        plotData[date].Y1 = (plotData[date].Y1 + b.batch.AvgStrength) / 2.0;
                    }
                    else
                    {
                        plotData.Add(date, new YData { Y1 = b.batch.AvgStrength, Y2 = 0 });
                    }
                }
            }

            // now find rolling average of 4 values
            int i = 0;
            double[] val = new double[plotData.Count];
            foreach(YData yData in plotData.Values)
            {
                val[i] = yData.Y1;

                if (i >= 3)
                {
                    yData.Y2 = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        yData.Y2 += val[i - j];
                    }
                    yData.Y2 /= 4.0;

                }
                else
                {
                    yData.Y2 = 0;
                }

                i++;
            }
        }

        private List<object> DateArray(DateTime startDate, DateTime endDate)
        {
            List<object> list = new List<object>();
            int days = (endDate - startDate).Days;

            for (int i = days; i >= 0; i--)
            {
                DateTime d = endDate.AddDays(-i);
                list.Add(d.ToString("yyyy-MM-dd"));
            }

            return list;
        }

        public async Task<bool> GenerateAllStatisticalReportsForRange(DateTime dtStart, DateTime dtEnd, string userid)
        {
            int total;
            bool status = true;
            DateTime now = DateTime.Now;

            List<Project> projects = new List<Project>();
            status = Global.db.GetProjects(projects, out total);
            if (!status) return false;

            foreach (Project p in projects)
            {
                if (p.StatisticalReport)
                {
                    ReportResult result = await Global.reportGenerator.GenerateStatisticalReport(p.Id, dtStart, dtEnd, 28, userid);
                    if (result != null && result.resultCnt > 0)
                    {
                        string file = string.Format("Statistical\\{0}\\{1}", now.ToString("yyyyMMdd"), Path.GetFileName(result.pdffile));
                        string dest = Path.Combine(Global.reportStoragePath, file);
                        string destDir = Path.GetDirectoryName(dest);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                        Report report = new Report();
                        report.ProjectId = p.Id;
                        report.ReportType = "Monthly";
                        report.FileName = dest;
                        report.ReportDate = DateTime.Now;
                        report.StartDate = dtStart;
                        report.EndDate = dtEnd;
                        report.EmailTo = p.EmailAddr;
                        report.EmailCC = p.EmailCC;
                        report.Released = 0;
                        report.ReleaseUser = null;
                        report.ReleaseDate = null;

                        try
                        {
                            string directory = Path.GetDirectoryName(dest);
                            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                            File.Move(result.pdffile, dest);

                            status = Global.db.InsertReport(report, userid);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error inserting monthly report for project {0}.", p.ProjectCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.logger.LogMessageEx("Error", "Error moving file {0} to destination: {1}",
                                result.pdffile, ex.Message);
                            status = false;
                        }
                    }
                }
            }

            return status;
        }

        public async Task<ReportResult> GenerateStatisticalReport(int projectId, DateTime dtStart, DateTime dtEnd, int testAge, string userId,
            int? filterGrade = null, string filterSupplierId = null, double? filterDimension = null)
        {
            bool status;
            string pdfFile = null;
            List<string> filesToDelete = new List<string>();

            // get project
            Project p = Global.db.GetProject(projectId);
            if (p == null) return null;

            ReportData report = new ReportData();

            report.startDate = dtStart;
            report.endDate = dtEnd;
            report.projectId = p.ProjectCode;
            report.projectName = p.ProjectName;
            report.companyName = p.SoldToCompany;
            report.companyAddress = p.SoldToAddress;
            report.attnTo = p.ApplicantName;

            List<Supplier> suppliers = new List<Supplier>();
            status = Global.db.GetSuppliersForProject(projectId, suppliers);
            if (!status) return null;

            foreach (Supplier supplier in suppliers)
            {
                if (filterSupplierId != null && supplier.Id != filterSupplierId) continue;

                List<TestSpec> testSpecs = new List<TestSpec>();
                status = Global.db.GetTestSpecsForSupplier(projectId, supplier.Id, testSpecs);
                if (!status) continue;

                foreach (TestSpec ts in testSpecs)
                {
                    List<CubeSet> cubeSets = new List<CubeSet>();

                    // get all cube sets with project id and supplier
                    status = Global.db.GetCubeSetsForProjectSupplierTestSpec(projectId, supplier.Id, ts.SpecId, cubeSets);
                    if (!status) continue;

                    int cubeCnt = 0;
                    int concreteGrade = 0;
                    SectionReport section = new SectionReport();

                    foreach (CubeSet cs in cubeSets)
                    {
                        if (filterGrade.HasValue && cs.ConcreteGrade != filterGrade.Value) continue;

                        if (cs.ConcreteGrade != concreteGrade)
                        {
                            concreteGrade = cs.ConcreteGrade;
                            section = new SectionReport();
                        }

                        section.scoNum = p.CurSCONum;
                        section.supplier = supplier.Name;
                        section.specification = ts.Description;
                        section.testStandard = ts.TestStandard;
                        section.concreteGrade = concreteGrade;

                        List<Batch> batches = new List<Batch>();
                        status = Global.db.GetBatchesForCubeSet(cs.Id, batches, testAge);
                        if (!status) continue;

                        CubeSetReport csr = new CubeSetReport();
                        csr.cubeSet = cs;

                        foreach (Batch b in batches)
                        {
                            if (filterDimension.HasValue && b.Dimension != filterDimension.Value) continue;

                            BatchReport br = new BatchReport();
                            br.batch = b;

                            status = Global.db.GetTestedCubes(br.cubes, b.ScoNum, b.Id, dtStart, dtEnd);
                            if (status && br.cubes.Count > 0)
                            {
                                br.CalcAvgStrength();
                                csr.batchReports.Add(br);

                                cubeCnt += br.cubes.Count;
                            }
                        }

                        if (csr.batchReports.Count > 0)
                        {
                            section.cubeSetReports.Add(csr);
                        }
                    }

                    if (section.cubeSetReports.Count > 0)
                    {
                        SortedDictionary<DateOnly, YData> plotData = new SortedDictionary<DateOnly, YData>();
                        ExtractPlotData(section, plotData);

                        // generate chart
                        string plotFile = GenerateChart(plotData);
                        section.plotImage = "file:///" + plotFile;
                        filesToDelete.Add(plotFile);

                        section.age = testAge;
                        section.cubeCnt = cubeCnt;
                        report.sections.Add(section);
                    }
                }
            }

            // generate html
            string tmplFile = Path.Combine(Global.assyPath, "ScribanReports/MonthlyStatisticalReport.tpl");
            string tmplContents = File.ReadAllText(tmplFile);
            Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

            string fn = string.Format("StatisticalReport_{0}_{1}_{2}-{3}", p.ProjectCode, dtStart.ToString("yyyyMMdd"),
                dtEnd.ToString("yyyyMMdd"), rand.Next(65535).ToString("X4"));
            string content = tmpl.Render(new { Report = report });
            content = await PostProcessHtmlAsync(new ReportHtmlContext
            {
                ReportType = "Statistical",
                ProjectId = p.Id,
                ProjectCode = p.ProjectCode,
                StartDate = dtStart,
                EndDate = dtEnd,
                UserId = userId,
                HtmlFileName = fn
            }, content);

            string path = Path.Combine(Global.htmlPath, fn + ".html");
            if (!Directory.Exists(Global.htmlPath)) Directory.CreateDirectory(Global.htmlPath);
            File.WriteAllText(path, content);

            // ensure css and images are there
            string cssFile = Path.Combine(Global.htmlPath, "monthly_report.css");
            string cssFileSrc = Path.Combine(Global.assyPath, "ScribanReports/monthly_report.css");
            //if (!File.Exists(cssFile)) File.Copy(cssFileSrc, cssFile);
            Util.CopyFileIfNewer(cssFileSrc, cssFile);

            string imgFile = Path.Combine(Global.htmlPath, "tuvsud_logo.png");
            string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports/tuvsud_logo.png");
            //if (!File.Exists(imgFile)) File.Copy(imgFileSrc, imgFile);
            Util.CopyFileIfNewer(imgFileSrc, imgFile);

            // print to pdf
            PrintResult presult = await printer.Print(content, $"-u \"{Global.htmlPath}\"");
            if (presult != null && !presult.HasError)
            {
                string dest = Path.Combine(Global.reportPath, fn + ".pdf");
                if (!Directory.Exists(Global.reportPath)) Directory.CreateDirectory(Global.reportPath);
                File.WriteAllBytes(dest, presult.Bytes);

                pdfFile = fn + ".pdf";
            }

            if (!Global.DebugKeepGenFiles)
            {
                try
                {
                    File.Delete(path); // html
                    foreach (string f in filesToDelete)
                    {
                        File.Delete(f);
                    }
                }
                catch(Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Error deleting generated files in GenerateStatisticalReport(): {0}", ex.Message);
                }
            }

            ReportResult result = new ReportResult()
            {
                pdffile = pdfFile,
                resultCnt = report.sections.Count
            };

            return result;
        }

        public async Task<bool> GenerateAllMonthlyReportsForRange(DateTime dtStart, DateTime dtEnd, string userid)
        {
            int total;
            bool status = true;
            DateTime now = DateTime.Now;

            List<Project> projects = new List<Project>();
            status = Global.db.GetProjects(projects, out total);
            if (!status) return false;

            foreach (Project p in projects)
            {
                if (p.MonthlyReport)
                {

                    ReportResult result = await Global.reportGenerator.GenerateMonthlyReport(p.Id, dtStart, dtEnd, userid);
                    if (result != null && result.resultCnt > 0)
                    {
                        string file = string.Format("Monthly\\{0}\\{1}", now.ToString("yyyyMMdd"), Path.GetFileName(result.pdffile));
                        string dest = Path.Combine(Global.reportStoragePath, file);
                        string destDir = Path.GetDirectoryName(dest);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                        Report report = new Report();
                        report.ProjectId = p.Id;
                        report.ReportType = "Monthly";
                        report.FileName = file;
                        report.ReportDate = DateTime.Now;
                        report.StartDate = dtStart;
                        report.EndDate = dtEnd;
                        report.EmailTo = p.EmailAddr;
                        report.EmailCC = p.EmailCC;
                        report.Released = 0;
                        report.ReleaseUser = null;
                        report.ReleaseDate = null;

                        try
                        {
                            string directory = Path.GetDirectoryName(dest);
                            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory); File.Move(result.pdffile, dest);

                            status = Global.db.InsertReport(report, "system");
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error inserting monthly report for project {0}.", p.ProjectCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.logger.LogMessageEx("Error", "Error moving file {0} to destination: {1}",
                                result.pdffile, ex.Message);
                            status = false;
                        }
                    }
                }

            }

            return status;
        }


        public async Task<ReportResult> GenerateMonthlyReport(int projectId, DateTime dtStart, DateTime dtEnd, string userId,
            int? filterGrade = null, string filterSupplierId = null, double? filterDimension = null, int filterTestAge = 0)
        {
            bool status;
            string pdfFile = null;

            // get project
            Project p = Global.db.GetProject(projectId);
            if (p == null) return null;

            ReportData report = new ReportData();

            report.startDate = dtStart;
            report.endDate = dtEnd;
            report.projectId = p.ProjectCode;
            report.projectName = p.ProjectName;
            report.companyName = p.SoldToCompany;
            report.companyAddress = p.SoldToAddress;
            report.attnTo = p.ApplicantName;

            List<Supplier> suppliers = new List<Supplier>();
            status = Global.db.GetSuppliersForProject(projectId, suppliers);
            if (!status) return null;

            foreach (Supplier supplier in suppliers)
            {
                if (filterSupplierId != null && supplier.Id != filterSupplierId) continue;

                List<TestSpec> testSpecs = new List<TestSpec>();
                status = Global.db.GetTestSpecsForSupplier(projectId, supplier.Id, testSpecs);
                if (!status) continue;

                foreach(TestSpec ts in testSpecs)
                {
                    List<CubeSet> cubeSets = new List<CubeSet>();

                    // get all cube sets with project id and supplier
                    status = Global.db.GetCubeSetsForProjectSupplierTestSpec(projectId, supplier.Id, ts.SpecId, cubeSets);
                    if (!status) continue;

                    int cubeCnt = 0;
                    int concreteGrade = 0;
                    SectionReport section = new SectionReport();

                    foreach (CubeSet cs in cubeSets)
                    {
                        if (filterGrade.HasValue && cs.ConcreteGrade != filterGrade.Value) continue;

                        if (cs.ConcreteGrade != concreteGrade)
                        {
                            concreteGrade = cs.ConcreteGrade;
                            section = new SectionReport();
                        }

                        section.scoNum = p.CurSCONum;
                        section.supplier = supplier.Name;
                        section.specification = ts.Description;
                        section.testStandard = ts.TestStandard;
                        section.concreteGrade = concreteGrade;

                        List<Batch> batches = new List<Batch>();
                        status = Global.db.GetBatchesForCubeSet(cs.Id, batches, filterTestAge);
                        if (!status) continue;

                        CubeSetReport csr = new CubeSetReport();
                        csr.cubeSet = cs;

                        foreach (Batch b in batches)
                        {
                            if (filterDimension.HasValue && b.Dimension != filterDimension.Value) continue;

                            b.CastingDate = cs.CastingDate;

                            BatchReport br = new BatchReport();
                            br.batch = b;

                            status = Global.db.GetTestedCubes(br.cubes, b.ScoNum, b.Id, dtStart, dtEnd);
                            if (status && br.cubes.Count > 0)
                            {
                                br.CalcAvgStrength();
                                csr.batchReports.Add(br);

                                cubeCnt += br.cubes.Count;
                            }
                        }

                        if (csr.batchReports.Count > 0)
                        {
                            section.cubeSetReports.Add(csr);
                        }
                    }

                    if (section.cubeSetReports.Count > 0)
                    {
                        section.cubeCnt = cubeCnt;
                        report.sections.Add(section);
                    }
                }
            }

            // generate html
            string tmplFile = Path.Combine(Global.assyPath, "ScribanReports/MonthlyReport.tpl");
            string tmplContents = File.ReadAllText(tmplFile);
            Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

            string fn = string.Format("MonthlyReport_{0}_{1}_{2}-{3}", p.ProjectCode, dtStart.ToString("yyyyMMdd"),
                dtEnd.ToString("yyyyMMdd"), rand.Next(65535).ToString("X4"));
            string content = tmpl.Render(new { Report = report });
            content = await PostProcessHtmlAsync(new ReportHtmlContext
            {
                ReportType = "Monthly",
                ProjectId = p.Id,
                ProjectCode = p.ProjectCode,
                StartDate = dtStart,
                EndDate = dtEnd,
                UserId = userId,
                HtmlFileName = fn
            }, content);

            string path = Path.Combine(Global.htmlPath, fn + ".html");
            if (!Directory.Exists(Global.htmlPath)) Directory.CreateDirectory(Global.htmlPath);
            File.WriteAllText(path, content);

            // ensure css and images are there
            string cssFile = Path.Combine(Global.htmlPath, "monthly_report.css");
            string cssFileSrc = Path.Combine(Global.assyPath, "ScribanReports/monthly_report.css");
            //if (!File.Exists(cssFile)) File.Copy(cssFileSrc, cssFile);
            Util.CopyFileIfNewer(cssFileSrc, cssFile);

            string imgFile = Path.Combine(Global.htmlPath, "tuvsud_logo.png");
            string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports/tuvsud_logo.png");
            //if (!File.Exists(imgFile)) File.Copy(imgFileSrc, imgFile);
            Util.CopyFileIfNewer(imgFileSrc, imgFile);

            // print to pdf
            PrintResult presult = await printer.Print(content, $"-u \"{Global.htmlPath}\"");
            if (presult != null && !presult.HasError)
            {
                string dest = Path.Combine(Global.reportPath, fn + ".pdf");
                if (!Directory.Exists(Global.reportPath)) Directory.CreateDirectory(Global.reportPath);
                File.WriteAllBytes(dest, presult.Bytes);

                pdfFile = fn + ".pdf";
            }

            if (!Global.DebugKeepGenFiles)
            {
                try
                {
                    File.Delete(path); // html
                }
                catch (Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Error deleting generated files in GenerateMonthlyReport(): {0}", ex.Message);
                }

            }

            ReportResult result = new ReportResult()
            {
                pdffile = pdfFile,
                resultCnt = report.sections.Count
            };

            return result;
        }

        public async Task<bool> GenerateDailyReportsForDay(DateTime dt, string userid)
        {
            int total;
            bool status = false;

            List<Project> projects = new List<Project>();
            status = Global.db.GetProjects(projects, out total);
            if (!status) return false;

            foreach (Project p in projects)
            {
                if (p.DailyReport)
                {
                    ReportResult result = await GenerateDailyReport(p.Id, dt, userid);
                    if (result != null && result.resultCnt > 0)
                    {
                        string file = string.Format("Daily\\{0}\\{1}", dt.ToString("yyyyMMdd"), Path.GetFileName(result.pdffile));
                        string dest = Path.Combine(Global.reportStoragePath, file);
                        string destDir = Path.GetDirectoryName(dest);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                        Report report = new Report();
                        report.ProjectId = p.Id;
                        report.ReportType = "Daily";
                        report.FileName = file;
                        report.ReportDate = DateTime.Now;
                        report.StartDate = dt;
                        report.EmailTo = p.EmailAddr;
                        report.EmailCC = p.EmailCC;
                        report.Released = 0;
                        report.ReleaseUser = null;
                        report.ReleaseDate = null;

                        try
                        {
                            string src = Path.Combine(Global.reportPath, result.pdffile);
                            File.Move(src, dest);

                            status = Global.db.InsertReport(report, userid);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error inserting daily report for project {0}.", p.ProjectCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.logger.LogMessageEx("Error", "Error moving file {0} to destination: {1}",
                                result.pdffile, ex.Message);
                        }
                    }
                }
            }

            return status;
        }

        public async Task<ReportResult> GenerateDailyReport(int projectId, DateTime dt, string userId,
            int? filterGrade = null, string filterSupplierId = null, double? filterDimension = null, int filterTestAge = 0)
        {
            bool status = false;
            int total;
            string pdffile = null;

            // get project
            Project p = Global.db.GetProject(projectId);
            if (p == null) return null;

            DailyReport report = new DailyReport();

            report.testDate = dt;
            report.projectId = p.ProjectCode;
            report.projectName = p.ProjectName;
            report.companyName = p.SoldToCompany;
            report.companyAddress = p.SoldToAddress;
            report.attnTo = p.ApplicantName;

            List<Supplier> suppliers = new List<Supplier>();
            status = Global.db.GetSuppliersForProject(projectId, suppliers);
            if (!status) return null;

            foreach (Supplier supplier in suppliers)
            {
                if (filterSupplierId != null && supplier.Id != filterSupplierId) continue;

                List<TestSpec> testSpecs = new List<TestSpec>();
                status = Global.db.GetTestSpecsForSupplier(projectId, supplier.Id, testSpecs);
                if (!status) continue;

                foreach (TestSpec ts in testSpecs)
                {
                    List<CubeSet> cubeSets = new List<CubeSet>();

                    // get all cube sets with project id and supplier
                    status = Global.db.GetCubeSetsForProjectSupplier(projectId, supplier.Id, cubeSets, out total);
                    if (!status) continue;

                    int cubeCnt = 0;
                    int concreteGrade = 0;
                    SectionReport section = new SectionReport();

                    foreach (CubeSet cs in cubeSets)
                    {
                        if (filterGrade.HasValue && cs.ConcreteGrade != filterGrade.Value) continue;

                        if (cs.ConcreteGrade != concreteGrade)
                        {
                            concreteGrade = cs.ConcreteGrade;
                            section = new SectionReport();
                        }

                        section.scoNum = p.CurSCONum;
                        section.supplier = supplier.Name;
                        section.specification = ts.Description;
                        section.testStandard = ts.TestStandard;
                        section.concreteGrade = concreteGrade;

                        List<Batch> batches = new List<Batch>();
                        status = Global.db.GetBatchesForCubeSet(cs.Id, batches, filterTestAge);
                        if (!status) continue;

                        CubeSetReport csr = new CubeSetReport();
                        csr.cubeSet = cs;

                        foreach (Batch b in batches)
                        {
                            if (filterDimension.HasValue && b.Dimension != filterDimension.Value) continue;

                            b.CastingDate = cs.CastingDate;
                            BatchReport br = new BatchReport();
                            br.batch = b;

                            status = Global.db.GetTestedCubes(br.cubes, b.ScoNum, b.Id, dt, dt);
                            if (status && br.cubes.Count > 0)
                            {
                                br.CalcAvgStrength();
                                csr.batchReports.Add(br);

                                cubeCnt += br.cubes.Count;
                            }
                        }

                        if (csr.batchReports.Count > 0)
                        {
                            section.cubeSetReports.Add(csr);
                        }
                    }

                    if (section.cubeSetReports.Count > 0)
                    {
                        section.cubeCnt = cubeCnt;
                        report.sections.Add(section);
                    }
                }
            }

            if (report.sections.Count > 0)
            {
                // generate html
                string tmplFile = Path.Combine(Global.assyPath, "ScribanReports/CubeTestReport.tpl");
                string tmplContents = File.ReadAllText(tmplFile);
                Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);
                string fn = string.Format("CubeDailyReport_{0}_{1}-{2}", dt.ToString("yyyyMMdd"), p.ProjectCode, rand.Next(65535).ToString("X4"));
                string content = tmpl.Render(new { Report = report });
                content = await PostProcessHtmlAsync(new ReportHtmlContext
                {
                    ReportType = "Daily",
                    ProjectId = p.Id,
                    ProjectCode = p.ProjectCode,
                    StartDate = dt,
                    EndDate = null,
                    UserId = userId,
                    HtmlFileName = fn
                }, content);

                string path = Path.Combine(Global.htmlPath, fn + ".html");
                if (!Directory.Exists(Global.htmlPath)) Directory.CreateDirectory(Global.htmlPath);
                File.WriteAllText(path, content);

                // ensure css and images are there
                string cssFile = Path.Combine(Global.htmlPath, "cube_test_report.css");
                string cssFileSrc = Path.Combine(Global.assyPath, "ScribanReports/cube_test_report.css");
                Util.CopyFileIfNewer(cssFileSrc, cssFile);

                string imgFile = Path.Combine(Global.htmlPath, "tuvsud_logo.png");
                string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports/tuvsud_logo.png");
                Util.CopyFileIfNewer(imgFileSrc, imgFile);

                string imgFile2 = Path.Combine(Global.htmlPath, "footer.png");
                string imgFileSrc2 = Path.Combine(Global.assyPath, "ScribanReports/footer.png");
                Util.CopyFileIfNewer(imgFileSrc2, imgFile2);

                PrintResult presult = await printer.Print(content, $"-u \"{Global.htmlPath}\"");
                if (presult != null && presult.Bytes.Length > 0 &&
                    (!presult.HasError || presult.ExitCode == 0))
                {
                    string dest = Path.Combine(Global.reportPath, fn + ".pdf");
                    if (!Directory.Exists(Global.reportPath)) Directory.CreateDirectory(Global.reportPath);
                    File.WriteAllBytes(dest, presult.Bytes);

                    pdffile = fn + ".pdf";
                }

                if (!Global.DebugKeepGenFiles)
                {
                    try
                    {
                        File.Delete(path); // html
                    }
                    catch (Exception ex)
                    {
                        Global.logger.LogMessageEx("Error", "Error deleting generated files in GenerateDailyReport(): {0}", ex.Message);
                    }

                }
            }

            ReportResult result = new ReportResult()
            {
                pdffile = pdffile,
                resultCnt = report.sections.Count
            };

            return result;
        }


        public async Task<string> GenerateCostSummary(DateTime dtStart, DateTime dtEnd)
        {
            bool status;
            string pdffile = null;

            CostSummaryReport report = new CostSummaryReport();
            report.startDate = dtStart;
            report.endDate = dtStart;

            // get all projects with cubes tested within the period
            List<Project> projects = new List<Project>();
            status = Global.db.GetProjectsWithTestedCubes(dtStart, dtEnd, projects);
            if (!status) return null;

            // get the tested cubes
            List<ProjectReport> projReports = new List<ProjectReport>();
            foreach (Project p in projects)
            {
                ProjectReport pr = new ProjectReport();
                pr.project = p;

                status = Global.db.GetTestedCubesForProject(p.Id, pr.cubes, dtStart, dtEnd);
                if (!status) continue;

                status = Global.db.GetTestedCubeSetsForProject(p.Id, pr.cubeSets, dtStart, dtEnd);
                if (!status) continue;

                projReports.Add(pr);
            }

            // sort by customer
            foreach (ProjectReport pr in projReports)
            {
                string key = pr.project.SoldToCompany;
                if (report.clientProjs.ContainsKey(key))
                {
                    report.clientProjs[key].Add(pr);
                }    
                else
                {
                    report.clientProjs.Add(key, new List<ProjectReport>());
                    report.clientProjs[key].Add(pr);
                }
            }

            // generate html
            string tmplFile = Path.Combine(Global.assyPath, "ScribanReports/CostSummaryReport.tpl");
            string tmplContents = File.ReadAllText(tmplFile);
            Scriban.Template tmpl = Scriban.Template.Parse(tmplContents);

            string content = tmpl.Render(new { Report = report });

            string fn = string.Format("CostSummaryReport_{0}_{1}-{2}", dtStart.ToString("yyyyMMdd"), 
                dtEnd.ToString("yyyyMMdd"), rand.Next(65535).ToString("X4"));
            string path = Path.Combine(Global.htmlPath, fn + ".html");
            if (!Directory.Exists(Global.htmlPath)) Directory.CreateDirectory(Global.htmlPath);
            File.WriteAllText(path, content);

            // ensure css and images are there
            string cssFile = Path.Combine(Global.htmlPath, "cost_summary_report.css");
            string cssFileSrc = Path.Combine(Global.assyPath, "ScribanReports/cost_summary_report.css");
            Util.CopyFileIfNewer(cssFileSrc, cssFile);

            string imgFile = Path.Combine(Global.htmlPath, "tuvsud_logo.png");
            string imgFileSrc = Path.Combine(Global.assyPath, "ScribanReports/tuvsud_logo.png");
            Util.CopyFileIfNewer(imgFileSrc, imgFile);

            PrintResult presult = await printer.Print(content, $"-u \"{Global.htmlPath}\"");
            if (presult != null && !presult.HasError)
            {
                string dest = Path.Combine(Global.reportPath, fn + ".pdf");
                if (!Directory.Exists(Global.reportPath)) Directory.CreateDirectory(Global.reportPath);
                File.WriteAllBytes(dest, presult.Bytes);

                pdffile = fn + ".pdf";
            }
            return pdffile;
        }
    }
}
