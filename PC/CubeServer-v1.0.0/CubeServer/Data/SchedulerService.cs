using CubeServer.Models;
using System.Threading;

namespace CubeServer.Data
{
    public class ScheduledTask
    {
        public string Name; // Daily, Monthly
        public int DayOfMonth;
        public int Hour;
        public int Minute;
        public DateTime? LastRun;
        public DateTime? NextRun;
    }

    public class SchedulerService : BackgroundService
    {
        List<ScheduledTask> taskList;

        public SchedulerService()
        {
            taskList = new List<ScheduledTask>();

#region debug-point B:scheduler-ctor-start
            Global.DebugReport("post-fix", "B", "SchedulerService.cs:24", "SchedulerService ctor start", new { dbReady = Global.db != null });
#endregion
            try
            {
                bool isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
                if (!isDev && Global.db != null)
                {
                    bool status = Global.db.GetScheduledTasks(taskList);
                    if (!status)
                    {
                        Global.logger.LogMessageEx("Error", "Error getting scheduled tasks from database.");
                    }
                }
#region debug-point B:scheduler-ctor-ok
                Global.DebugReport("post-fix", "B", "SchedulerService.cs:37", "SchedulerService ctor completed", new { taskCount = taskList.Count });
#endregion
            }
            catch (Exception ex)
            {
                taskList.Clear();
                Global.logger?.LogMessageEx("Error", "Error getting scheduled tasks from database: {0}", ex.Message);
#region debug-point B:scheduler-ctor-fail
                Global.DebugReport("post-fix", "B", "SchedulerService.cs:44", "SchedulerService ctor failed", new { error = ex.Message });
#endregion
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool status;

            do
            {
                DateTime now = DateTime.Now;
                foreach(ScheduledTask task in taskList)
                {
                    switch(task.Name)
                    {
                        case "TimeInterval":
                            if (task.NextRun.HasValue)
                            {
                                if (now > task.NextRun)
                                {
                                    status = RunTimeIntervalTask();
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error running Time Interval scheduled task.");
                                    }

                                    task.LastRun = now;
                                    task.NextRun = ((DateTime)task.NextRun).AddMinutes(task.Minute);
                                    if (now > task.NextRun)
                                    {
                                        task.NextRun = null; // recalculate below
                                    }

                                    status = Global.db.UpdateScheduledRunTimes(task, "system");
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error updating next Time Interval run.");
                                    }
                                }
                            }
                            else
                            {
                                DateTime nextRun = now.AddMinutes(task.Minute - now.Minute % task.Minute);
                                nextRun.AddSeconds(-nextRun.Second);
                                TimeOnly timeNow = TimeOnly.FromDateTime(now);
                                if (now > nextRun)
                                {
                                    nextRun = nextRun.AddMinutes(1);
                                }

                                task.NextRun = nextRun;
                                status = Global.db.UpdateScheduledRunTimes(task, "system");
                                if (!status)
                                {
                                    Global.logger.LogMessageEx("Error", "Error updating next Time Interval run.");
                                }
                            }
                            break;

                        case "Daily":
                            if (task.NextRun.HasValue)
                            {
                                if (now > task.NextRun)
                                {
                                    status = await RunDailyTask(DateTime.Today);
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error running Daily scheduled task.");
                                    }

                                    task.LastRun = now;
                                    task.NextRun = ((DateTime)task.NextRun).AddDays(1);
                                    if (now > task.NextRun)
                                    {
                                        task.NextRun = null; // recalculate below
                                    }

                                    status = Global.db.UpdateScheduledRunTimes(task, "system");
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error updating next Daily run.");
                                    }
                                }
                            }
                            else
                            {
                                DateTime nextRun = new DateTime(now.Year, now.Month, now.Day, task.Hour, task.Minute, 0);
                                TimeOnly timeNow = TimeOnly.FromDateTime(now);
                                if (now > nextRun)
                                {
                                    nextRun = nextRun.AddDays(1);
                                }


                                task.NextRun = nextRun;
                                status = Global.db.UpdateScheduledRunTimes(task, "system");
                                if (!status)
                                {
                                    Global.logger.LogMessageEx("Error", "Error updating next Daily run.");
                                }
                            }
                            break;

                        case "Monthly":
                            if (task.NextRun.HasValue)
                            {
                                if (now > task.NextRun)
                                {
                                    status = await RunMonthlyTask();
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error running Monthly scheduled task.");
                                    }

                                    task.LastRun = now;
                                    task.NextRun = ((DateTime)task.NextRun).AddMonths(1);
                                    if (now > task.NextRun)
                                    {
                                        task.NextRun = null; // recalculate below
                                    }

                                    status = Global.db.UpdateScheduledRunTimes(task, "system");
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error updating Monthly run times.");
                                    }
                                }
                            }
                            else
                            {
                                DateTime nextRun = new DateTime(now.Year, now.Month, task.DayOfMonth, task.Hour, task.Minute, 0);
                                TimeOnly timeNow = TimeOnly.FromDateTime(now);
                                if (now > nextRun)
                                {
                                    nextRun = nextRun.AddMonths(1);
                                }

                                task.NextRun = nextRun;
                                status = Global.db.UpdateScheduledRunTimes(task, "system");
                                if (!status)
                                {
                                    Global.logger.LogMessageEx("Error", "Error updating next Monthly run.");
                                }
                            }
                            break;
                    }
                }

                // delays every 15 seconds
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        public bool RunTimeIntervalTask()
        {
            bool status;

            status = Global.dataAnalyzer.CalcRollingAvgStrength();
            status = Global.db.EvaluateBatchCompletion() && status;

            return status;
        }

        public async Task<bool> RunDailyTask(DateTime dt)
        {
            bool status = await Global.reportGenerator.GenerateDailyReportsForDay(DateTime.Today, "system");
            //status = Global.emailer.SendDailyReports() && status; // do not send automatically

            // delete all files in report directory
            status = Util.ClearDirectory(Global.reportPath);
            
            return status;
        }

        public async Task<bool> RunMonthlyTask()
        {
            bool status = false;
            DateTime now = DateTime.Now;

            // last month
            DateTime lastMonth = now.AddMonths(-1);
            DateTime dtStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            DateTime dtEnd = dtStart.AddMonths(1).AddTicks(-1);

            status = await Global.reportGenerator.GenerateAllMonthlyReportsForRange(dtStart, dtEnd, "system");
            status = await Global.reportGenerator.GenerateAllStatisticalReportsForRange(dtStart, dtEnd, "system") && status;
            // do no send automatically
            //status = Global.emailer.SendMonthlyReports() && status;
            //status = Global.emailer.SendStatisticalReports() && status;
            return status;
        }
    }
}
