using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CubeTester.GUI;
using FBase;

namespace CubeTester.Classes
{
    public static class Global
    {
        // simulation mode enable
        public static bool Simulation;

        // version
        public static Version version;

        // fixed values
        public static int testerMax = 6;

        // GUI sync context
        public static MainForm mainForm;
        public static SynchronizationContext UIContext;
        public static string mainFormTitle;

        // application objects
        public static App app;
        public static Stopwatch stopwatch;
        public static FLogger logger;
        public static AlarmManager alarmMgr;
        public static SoundManager soundMgr;

        // signals
        public static bool silenceAlarmBuzzer;  // turn off buzzer
        public static bool resetError;

        // mode
        public static RunMode runMode;
        public static SystemStatus systemStatus;

        // Database
        public static CubeTesterDB localDB = null;
        public static string localDBName = "cubetester.db";
        public static Dictionary<string, string> localSettings;
        public static Dictionary<string, double> localPar;

        // alarms
        public static string alarmsFile = "Alarms.csv";
        public static string warningsFile = "Warnings.csv";
        public static Dictionary<string, string> alarmCodes;
        public static Dictionary<string, string> warnCodes;
        public static string alarmActive = "";
        public static string warningActive = "";

        // templates

        // user management
        public static User curUser;
        public static byte[] passwordKey = new byte[]
        {
            0x94, 0x66, 0xE5, 0xCB, 0xC0, 0x06, 0xDB, 0x99
        };
        public static byte[] aesKey = new byte[]
        {
            0x9F, 0x86, 0x5E, 0x0E, 0x82, 0xD1, 0xAF, 0x4B, 
            0x86, 0x03, 0x93, 0x43, 0x32, 0x22, 0x39, 0x25
        };

        // Paths and Filenames
        public static string configPath = "C:\\CubeTester";
        public static string logPath = configPath + "\\Logs";
        public static string dataPath = configPath + "\\Data";
        public static string recipePath = configPath + "\\Recipes";
        public static string resultPath = configPath + "\\Results";
        public static string exceptionPath = configPath + "\\Exceptions";
        public static string appPath;
        public static string configFile = null;
        public static string witnessDisplayTemplateFile;
        public static string witnessDisplayFile;

        // display formats
        public static string speedAccelFormat = "0.0";
        public static string positionFormat = "0.000";
        public static string angleFormat = "0.000";
        public static string countFormat = "0";
        public static string datetimeFormatSec = "yyyy-MM-dd HH:mm:ss";
        public static string datetimeFormatMicrosec = "yyyy-MM-dd HH:mm:ss.ffd";
        public static string SqlDateFormat = "yyyy-MM-ddTHH:mm:ss";

        // motion settings
        public static double posTol = 0.005; // um, position tolerance
        public static double rotPosTol = 0.1; // deg, rotary position tolerance
        public static double speedPct = 100; // global speed %

        // run time info
        public static int processCycles;
        public static bool repeatabilityTest = false;

        public static void Init()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

    }
}
