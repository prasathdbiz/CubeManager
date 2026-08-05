using CubeServer.Authentication;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace CubeServer.Data
{
    public static class Global
    {
        public static FLogger logger;
        static readonly HttpClient debugHttpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };

        public static IServiceProvider Services;
        public static Database db;
        public static CubeServerApp app;

        public static readonly IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static string rootPath = config["CubeMgr:RootPath"] ?? config["RootPath"] ?? Directory.GetCurrentDirectory();
        public static string logPath = Path.Combine(rootPath, "logs");
        public static string quoPath = Path.Combine(rootPath, "Quotations");
        public static string htmlPath = Path.Combine(rootPath, "html");
        public static string reportStoragePath = Path.Combine(rootPath, "Reports");

        public static string assyPath;
        public static string appPath;
        public static string reportPath;

        public static string userName;

        public static ReportGenerator reportGenerator;
        public static DataAnalyzer dataAnalyzer;
        public static Emailer emailer;
        public static TwoFactorAuth twoFA;
        public static CancellationToken schedulerToken = new CancellationToken();

        public const bool testingMode = false;

        // encryption key
        public static byte[] aesKey =
            {
                0xF6, 0xEC, 0xC9, 0xE5, 0x0A, 0xE2, 0xC3, 0x24, 
                0x5C, 0xE0, 0x05, 0xBA, 0x2C, 0x84, 0x4C, 0xCC
            };
        public static byte[] aesIV =
            {
                0xDC, 0x65, 0xB0, 0xEA, 0x59, 0x6E, 0x71, 0xF9,
                0xBC, 0xC7, 0xE6, 0x3E, 0x70, 0x8B, 0x02, 0xA0
            };

        // options
        public static bool DebugKeepGenFiles = false;

        // ui
        public static MudTheme CubeMgrTheme;
        public static string contentMaxWidth = "1000px";

        // display formats
        public static string dateFormat = "yyyy-MM-dd";
        public static string dateTimeFormatFine = "yyyy-MM-dd HH:mm:ss.ff";
        public static string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string SqlDateFormat = "yyyy-MM-ddTHH:mm:ss";
        public static string numericFormat = "0.####";
        public static string priceFormat = "0.00";

        // separators
        public static string[] spaceSeparator = { " " };
        public static string[] dotSeparator = { "." };
        public static string[] commaSeparator = { "," };
        public static string[] crSeparator = { "\r", "\n" };

        public static void DebugReport(string runId, string hypothesisId, string location, string msg, object data = null)
        {
#region debug-point A:debug-report
            try
            {
                string enabled = Environment.GetEnvironmentVariable("CUBESERVER_DEBUG_REPORT") ?? "";
                if (!string.Equals(enabled, "1", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? Directory.GetCurrentDirectory();
                string envPath = Path.Combine(projectRoot, ".dbg", "server-startup-fail.env");
                string url = "http://127.0.0.1:7777/event";
                string sessionId = "server-startup-fail";

                if (File.Exists(envPath))
                {
                    foreach (string line in File.ReadAllLines(envPath))
                    {
                        if (line.StartsWith("DEBUG_SERVER_URL=", StringComparison.OrdinalIgnoreCase))
                            url = line.Substring("DEBUG_SERVER_URL=".Length).Trim();
                        else if (line.StartsWith("DEBUG_SESSION_ID=", StringComparison.OrdinalIgnoreCase))
                            sessionId = line.Substring("DEBUG_SESSION_ID=".Length).Trim();
                    }
                }

                string payload = JsonSerializer.Serialize(new
                {
                    sessionId,
                    runId,
                    hypothesisId,
                    location,
                    msg = "[DEBUG] " + msg,
                    data,
                    ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });

                using StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                _ = debugHttpClient.PostAsync(url, content);
            }
            catch
            {
            }
#endregion
        }
    }
}
