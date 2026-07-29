using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator.WiFi;
using static QRCoder.PayloadGenerator;

namespace CubeServer.Data
{
    public class CubeServerApp
    {
        public Dictionary<string, string> settings { get; set; }
        public Dictionary<string, double> par { get; set; }

        Database db;
        FLogger logger;
        FLogFileWriter logFileWriter;

        public List<long> deleteCubeIds; // for deletion at cube pc
        public object lckDeletedCubeIds = 1;

        public CubeServerApp()
        {
#region debug-point A:ctor-start
            Global.DebugReport("post-fix", "A", "CubeServerApp.cs:28", "CubeServerApp ctor start");
#endregion
            // loggers
            InitLoggers();

#region debug-point D:after-loggers
            Global.DebugReport("post-fix", "D", "CubeServerApp.cs:33", "Loggers initialized", new { logPath = Global.logPath, rootPath = Global.rootPath });
#endregion

            Global.db = new Database();
            db = Global.db;

#region debug-point A:after-db-ctor
            Global.DebugReport("post-fix", "A", "CubeServerApp.cs:39", "Database instance created");
#endregion

            settings = new Dictionary<string, string>();
            par = new Dictionary<string, double>();
            deleteCubeIds = new List<long>();

            // string settings
            settings.Add("SmtpServer", "");
            settings.Add("SenderName", "TUV SUD");
            settings.Add("SenderEmail", "");
            settings.Add("SenderPassword", "");
            settings.Add("EmailBcc", "");

            bool isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
            if (!isDev)
            {
                try
                {
#region debug-point A:settings-read-start
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:55", "Reading default users and string settings");
#endregion
                    db.CreateDefaultUsers();
                    db.ReadAllSettings(settings);
#region debug-point A:settings-read-ok
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:59", "String settings read completed", new { settingCount = settings.Count });
#endregion
                }
                catch (Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Database init/settings read failed: {0}", ex.Message);
#region debug-point A:settings-read-fail
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:64", "String settings read failed", new { error = ex.Message });
#endregion
                }
            }

            // theme
            Global.CubeMgrTheme = new MudTheme()
            {
                Palette = new PaletteLight()
                {
                    Primary = "#0046AD", // Colors.Blue.Darken2,
                    Secondary = Colors.Red.Darken2,
                    AppbarBackground = Colors.Blue.Darken4,                    
                },
                PaletteDark = new PaletteDark()
                {
                    Primary = Colors.Blue.Lighten1
                },                
                LayoutProperties = new LayoutProperties()
                {
                    DrawerWidthLeft = "250px",
                    DrawerWidthRight = "300px",                    
                }
            };

            // numeric settings
            par.Add("WebPageRefreshInterval", 5); // seconds
            par.Add("WebApiTokenValidity", 3600); // seconds
            par.Add("HoldReports", 1); // let admin release emails
            par.Add("SmtpPort", 1025);
            par.Add("Enable2FA", 1); // 2 - factor authentification
            par.Add("EnableComplexPasswords", 1); // Complex passwords include Uppercase, lowercase and a number
            par.Add("UseSSO", 1);

            if (!isDev)
            {
                try
                {
#region debug-point A:numeric-read-start
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:99", "Reading numeric settings");
#endregion
                    db.ReadAllNumericSettings(par);
#region debug-point A:numeric-read-ok
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:103", "Numeric settings read completed", new { numericCount = par.Count });
#endregion
                }
                catch (Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Database numeric settings read failed: {0}", ex.Message);
#region debug-point A:numeric-read-fail
                    Global.DebugReport("post-fix", "A", "CubeServerApp.cs:108", "Numeric settings read failed", new { error = ex.Message });
#endregion
                }
            }

            // application path
            Global.assyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //Global.appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            Global.appPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Global.reportPath = Path.Combine(Global.appPath, "reports");

            // report generator
            Global.reportGenerator = new ReportGenerator();

            // data analyzer
            Global.dataAnalyzer = new DataAnalyzer();

            // emailer
            Global.emailer = new Emailer();
            Global.emailer.SetSmtpServer(settings["SmtpServer"], (int)par["SmtpPort"], 
                settings["SenderName"], settings["SenderEmail"], settings["SenderPassword"]);
            Global.emailer.SetBcc(settings["EmailBcc"]);

            // 2FA
            Global.twoFA = new TwoFactorAuthNet.TwoFactorAuth("Cube Manager", qrcodeprovider:new CubeQRProvider());

#region debug-point A:ctor-end
            Global.DebugReport("post-fix", "A", "CubeServerApp.cs:133", "CubeServerApp ctor finished");
#endregion
        }

        public void InitLoggers()
        {
            // init logger
            Global.logger = new FLogger();
            logger = Global.logger;

            logger.AddDomain("Fatal");
            logger.AddDomain("Error");
            logger.AddDomain("Info");

            // main log file
            logFileWriter = new FLogFileWriter(Global.logPath + "\\cubemgr", 1000);
            logger.AddLogWriter(logFileWriter, "Fatal,Error,Info");
        }
    }
}
