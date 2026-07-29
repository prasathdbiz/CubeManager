using CubeTester.GUI;
using CubeTester.Classes;
using FBase;
using System.Windows.Forms;
using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace CubeTester
{
    public partial class MainForm : Form
    {
        SplitterPanel pnlDisplay;
        OverviewPanel pnlOverview;
        TestersPanel pnlTesters;
        ServerPanel pnlServer;
        PlcPanel pnlPLC;
        SettingsPanel pnlSettings;
        UsersPanel pnlUsers;
        AboutPanel pnlAbout;

        WitnessDisplay frmWitnessDisplay;

        FLogger logger;
        FLogFileWriter logFileWriter;
        FLogTextBoxWriter mainLogMsgWriter;
        FLogTextBoxWriter tester1LogMsgWriter;
        FLogTextBoxWriter tester2LogMsgWriter;
        FLogTextBoxWriter tester3LogMsgWriter;
        FLogTextBoxWriter tester4LogMsgWriter;
        FLogTextBoxWriter tester5LogMsgWriter;
        FLogTextBoxWriter tester6LogMsgWriter;

        public MainForm()
        {
            InitializeComponent();

            pnlDisplay = splitContainer3.Panel1;
            tbLog.Text = "";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // version
            Global.version = new Version(Application.ProductVersion);

            // get current synchronization context
            Global.mainForm = this;
            Global.UIContext = SynchronizationContext.Current;
            Global.mainFormTitle = this.Text;

            Global.Init();

            // init app paths
            InitPaths();

            // setup loggers
            InitLoggers();

            // load local configuration
            if (!InitDBs())
            {
                this.Close();   // Exit application
                return;
            }

            // init GUI
            InitGUI();

            // init system objects
            InitSystem();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Pop on top
            this.TopMost = true;
            this.TopMost = false;

            // output version
            string msg = string.Format("{0} v{1}.{2}.{3}.{4}", Application.ProductName,
                Global.version.Major, Global.version.Minor, Global.version.Build, Global.version.Revision);
            Global.logger.LogMessage("Info", msg);

            // load configuration
            Global.app = new App();
            if (!Global.app.LoadConfiguration(Global.localDB))
            {
                Util.ErrorMessageBox("Error opening configuration. Fatal error. Please restart.", "Initialization");
                return;
            }

            // simulation indicator
            Global.Simulation = Global.app.par["Simulation"] > 0;
            lblSimulation.Visible = Global.Simulation;
            if (Global.Simulation)
            {
                Global.logger.LogMessage("Info", "Running in Simulation mode.");
            }

            // default user
            Global.curUser = Global.localDB.GetUser(Global.Simulation ? "admin" : "operator");

            splitContainer1.SplitterDistance = 90;

            ShowOverview(null);

            // init timer
            timerUpdate.Enabled = true;

            // Start operations
            Global.app.Start();
        }

        private void InitPaths()
        {
            Global.appPath = Path.GetFullPath(Application.StartupPath + "/../../");
            Global.witnessDisplayTemplateFile = Path.Combine(Application.StartupPath, "html/cubes_tmpl.html");
            Global.witnessDisplayFile = Path.Combine(Application.StartupPath, "html/cubes.html");
        }

        private bool InitDBs()
        {
            Global.localDB = new CubeTesterDB();
            string dbPath = Path.Combine(Application.StartupPath, Global.localDBName);
            if (!Global.localDB.Open(dbPath))
            {
                Util.ErrorMessageBox("Error connecting to database", "Settings Database");
                return false;
            }

            return true;
        }

        private void InitLoggers()
        {
            // init logger
            Global.logger = new FLogger();
            logger = Global.logger;

            logger.AddDomain("Fatal");
            logger.AddDomain("Error");
            logger.AddDomain("Info");
            logger.AddDomain("Comm");
            logger.AddDomain("Tester1");
            logger.AddDomain("Tester2");
            logger.AddDomain("Tester3");
            logger.AddDomain("Tester4");
            logger.AddDomain("Tester5");
            logger.AddDomain("Tester6");

            // main log file
            logFileWriter = new FLogFileWriter(Global.logPath + "\\cube", 1000);
            logger.AddLogWriter(logFileWriter, "Fatal,Error,Info");

            // main textbox log
            mainLogMsgWriter = new FLogTextBoxWriter(tbLog, Global.UIContext, 100);
            logger.AddLogWriter(mainLogMsgWriter, "Fatal,Error,Info,Comm");

            // tester comm logs
            tester1LogMsgWriter = new FLogTextBoxWriter(tbTester1Log, Global.UIContext, 100);
            logger.AddLogWriter(tester1LogMsgWriter, "Tester1");
            tester2LogMsgWriter = new FLogTextBoxWriter(tbTester2Log, Global.UIContext, 100);
            logger.AddLogWriter(tester2LogMsgWriter, "Tester2");
            tester3LogMsgWriter = new FLogTextBoxWriter(tbTester3Log, Global.UIContext, 100);
            logger.AddLogWriter(tester3LogMsgWriter, "Tester3");
            tester4LogMsgWriter = new FLogTextBoxWriter(tbTester4Log, Global.UIContext, 100);
            logger.AddLogWriter(tester4LogMsgWriter, "Tester4");
            tester5LogMsgWriter = new FLogTextBoxWriter(tbTester5Log, Global.UIContext, 100);
            logger.AddLogWriter(tester5LogMsgWriter, "Tester5");
            tester6LogMsgWriter = new FLogTextBoxWriter(tbTester6Log, Global.UIContext, 100);
            logger.AddLogWriter(tester6LogMsgWriter, "Tester6");

            // exception handler which will log to error
            Util.InitExceptionHandler();
        }

        private void InitSystem()
        {
            Util.Init(Global.logger);

            // sequence and alarm manager
            Global.alarmMgr = new AlarmManager();
        }

        private void InitGUI()
        {
            pnlOverview = new OverviewPanel();
            pnlTesters = new TestersPanel();
            pnlServer = new ServerPanel();
            pnlPLC = new PlcPanel();
            pnlSettings = new SettingsPanel();
            pnlUsers = new UsersPanel();
            pnlAbout = new AboutPanel();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult res = Util.QuestionMessageBox("Close Cube Tester Application?", "Close Application");
                if (res == DialogResult.Yes)
                {
                    timerUpdate.Enabled = false;
                    Task task1 = Task.Run(() => Global.app.testServer?.StopServer());
                    Task task2 = Task.Run(() => Global.app.plc?.StopThread());
                    await task1;
                    await task2;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void UpdateUserAccess()
        {
            btnTesters.Enabled = Global.curUser.privilege >= UserPrivilege.Administrator;
            btnServer.Enabled = Global.curUser.privilege >= UserPrivilege.Administrator;
            btnSettings.Enabled = Global.curUser.privilege >= UserPrivilege.Administrator;
        }

        private void UpdateDisplay()
        {
            lblUser.Text = Global.curUser.id;

            if (Global.app != null)
            {
                lblStandaloneMode.Visible = Global.app.par["StandaloneMode"] > 0;
            }

            UpdateUserAccess();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        public void SetAlarm(object obj)
        {
            string code = (string)obj;
            if (Global.alarmCodes.ContainsKey(code))
            {
                lblAlarms.Text = Global.alarmCodes[code];
                lblAlarms.BackColor = Color.DarkRed;
                lblAlarms.ForeColor = Color.Yellow;
                Global.alarmActive = code;
            }
        }

        public void ClearAlarm(object obj)
        {
            string code = (string)obj;

            if (Global.alarmActive == code || Global.warningActive == code ||
                code == "All")
            {
                lblAlarms.Text = "System Normal";
                lblAlarms.BackColor = Color.FromArgb(128, 255, 128);
                lblAlarms.ForeColor = Color.Black;
                Global.alarmActive = "";
                Global.warningActive = "";
            }
        }

        public void ClearAlarmPrefix(object obj)
        {
            string code = (string)obj;

            if (Global.alarmActive.StartsWith(code) || Global.warningActive.StartsWith(code))
            {
                lblAlarms.Text = "System Normal";
                lblAlarms.BackColor = Color.FromArgb(128, 255, 128);
                lblAlarms.ForeColor = Color.Black;
                Global.alarmActive = "";
                Global.warningActive = "";
            }
        }

        public void SetWarning(object obj)
        {
            string code = (string)obj;
            if (Global.warnCodes.ContainsKey(code))
            {
                lblAlarms.Text = Global.warnCodes[code];
                lblAlarms.BackColor = Color.Orange;
                lblAlarms.ForeColor = Color.Yellow;
                Global.warningActive = code;
            }
        }

        public void ShowOverview(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlOverview.Setup();
            pnlDisplay.Controls.Add(pnlOverview);

            HighlightButton(btnOverview);
        }

        private void ShowTesters(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlTesters.Setup();
            pnlDisplay.Controls.Add(pnlTesters);

            HighlightButton(btnTesters);
        }

        private void ShowServer(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlServer.Setup();
            pnlDisplay.Controls.Add(pnlServer);

            HighlightButton(btnServer);
        }

        private void ShowPLC(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlPLC.Setup();
            pnlDisplay.Controls.Add(pnlPLC);

            HighlightButton(btnPLC);
        }

        private void ShowSettings(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlSettings.Setup();
            pnlDisplay.Controls.Add(pnlSettings);

            HighlightButton(btnSettings);
        }

        private void ShowUsers(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlUsers.Setup();
            pnlDisplay.Controls.Add(pnlUsers);

            HighlightButton(btnUsers);
        }

        private void ShowAbout(object obj)
        {
            pnlDisplay.Controls.Clear();
            pnlAbout.Setup();
            pnlDisplay.Controls.Add(pnlAbout);

            HighlightButton(btnAbout);
        }

        private void HighlightButton(Button btn)
        {
            btnOverview.BackColor = Color.White;
            btnTesters.BackColor = Color.White;
            btnServer.BackColor = Color.White;
            btnPLC.BackColor = Color.White;
            btnSettings.BackColor = Color.White;
            btnUsers.BackColor = Color.White;
            btnAbout.BackColor = Color.White;

            if (btn == btnOverview) btnOverview.BackColor = Color.Yellow;
            else if (btn == btnTesters) btnTesters.BackColor = Color.Yellow;
            else if (btn == btnServer) btnServer.BackColor = Color.Yellow;
            else if (btn == btnPLC) btnPLC.BackColor = Color.Yellow;
            else if (btn == btnSettings) btnSettings.BackColor = Color.Yellow;
            else if (btn == btnUsers) btnUsers.BackColor = Color.Yellow;
            else if (btn == btnAbout) btnAbout.BackColor = Color.Yellow;
        }

        private void btnOverview_Click(object sender, EventArgs e)
        {
            ShowOverview(null);
        }

        private void btnTesters_Click(object sender, EventArgs e)
        {
            ShowTesters(null);
        }

        private void btnWitnessDisplay_Click(object sender, EventArgs e)
        {
            if (frmWitnessDisplay == null || frmWitnessDisplay.IsDisposed)
            {
                frmWitnessDisplay = new WitnessDisplay();
            }
            frmWitnessDisplay.Setup((int)Global.app.par["WitnessScreenNum"]);
            frmWitnessDisplay.Show();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            ShowServer(null);
        }

        private void btnPLC_Click(object sender, EventArgs e)
        {
            ShowPLC(null);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ShowSettings(null);
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ShowUsers(null);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            ShowAbout(null);
        }

        private void lblAlarms_DoubleClick(object sender, EventArgs e)
        {
            DialogResult res = Util.QuestionMessageBox("Reset Alarm? Please ensure the error has been rectified.", 
                "Reset Alarm");
            if (res == DialogResult.Yes)
            {
                ClearAlarm("All");
            }
        }
    }
}