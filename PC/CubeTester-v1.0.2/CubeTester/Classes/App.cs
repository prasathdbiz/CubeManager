using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FBase;

namespace CubeTester.Classes
{
    public class App : BaseApp
    {
        public Dictionary<string, string> settings;
        public Dictionary<string, double> par;
        public TestServer testServer;
        public TcpClient[] clients;
        public string[] clientStatus;
        public Cube[] cubeAtTester;
        public TesterPar[] testerPar;
        public string[] testerReply;
        public bool[] testerStartAck;
        public double[] testerForce;
        public bool[] testerIsTesting;
        public bool[] testerResultReady;
        public bool serverStatus;
        public bool plcStatus;
        public KeyencePLC plc;
        public CubeMgrClient cmClient;

        public Dictionary<int, CubeSet> cubeSets;
        public Dictionary<string, Batch> batches;
        public Dictionary<string, List<Cube>> cubes;

        FLogger logger;

        object lckCubeData = 1;
        public bool newCubeData;

        public App()
        {
            settings = new Dictionary<string, string>();
            par = new Dictionary<string, double>();
            clients = new TcpClient[Global.testerMax];
            clientStatus = new string[Global.testerMax];
            cubeAtTester = new Cube[Global.testerMax];
            testerPar = new TesterPar[Global.testerMax];
            testerReply = new string[Global.testerMax];
            testerForce = new double[Global.testerMax];
            testerIsTesting = new bool[Global.testerMax]; 
            testerResultReady = new bool[Global.testerMax];
            testerStartAck = new bool[Global.testerMax];
            for (int i=0; i<cubeAtTester.Length; i++)
            {
                cubeAtTester[i] = new Cube();
                testerPar[i] = new TesterPar();
            }

            cubeSets = new Dictionary<int, CubeSet>();
            batches = new Dictionary<string, Batch>();
            cubes = new Dictionary<string, List<Cube>>();

            logger = Global.logger;
        }

        public bool LoadConfiguration(CubeTesterDB db)
        {
            try
            {
                settings.Add("LastConfigPath", "");
                settings.Add("TesterIpAddr1", "192.168.20.11");
                settings.Add("TesterIpAddr2", "192.168.20.12");
                settings.Add("TesterIpAddr3", "192.168.20.13");
                settings.Add("TesterIpAddr4", "192.168.20.14");
                settings.Add("TesterIpAddr5", "192.168.20.15");
                settings.Add("TesterIpAddr6", "192.168.20.16");
                settings.Add("PlcIpAddr", "192.168.10.10");
                //settings.Add("ServerIpAddr", "192.168.20.10");
                settings.Add("AdminPassword", "");

                settings.Add("CubeMgrUri", "https://localhost:44393");
                settings.Add("CubeMgrUserId", "webapi");
                settings.Add("CubeMgrPassword", "mAiXNx7J");
                settings.Add("CubeMgrLastDownload", "");
                settings.Add("CubeMgrLastUpload", "");
                settings.Add("CubeMgrWinUserId", ""); // if using windows auth
                settings.Add("CubeMgrWinPassword", ""); // if using windows auth, encrypted

                if (!db.ReadAllSettings(settings))
                {
                    logger.LogMessage("Error", "Error loading string settings.");
                    return false;
                }

                par.Add("ConfigVersion", 0);
                par.Add("LoadLastConfig", 1);
                par.Add("Simulation", 0);
                par.Add("MinDiskSpacePct", 10); // %
                par.Add("ServerPort", 6789);
                par.Add("WitnessScreenNum", 1); // typically 1 is secondary monitor
                par.Add("PlcPort", 8501); // keyence plc port
                par.Add("CubeMgrSyncInterval", 10000); // ms
                par.Add("WebApiWaitTime", 30000); // ms
                par.Add("StandaloneMode", 0); // 0:Production Mode, 1:Standalone Mode
                par.Add("RapidForceCutoff", 10); // kN


                if (!db.ReadAllNumericSettings(par))
                {
                    logger.LogMessage("Error", "Error loading numeric settings.");
                    return false;
                }

                LoadAlarmList();
                LoadWarningList();

                // create default users
                if (!db.ContainsTable("Users") || db.GetUserCount() == 0)
                {
                    db.CreateDefaultUsers();
                }
            }
            catch (Exception e)
            {
                logger.LogMessageEx("Error", "Error loading configuration: {0}", e.Message);
                Util.InfoMessageBox(e.StackTrace, "Exception Stack Trace");
                return false;
            }

            return true;
        }

        public bool Start()
        {
            bool status;

            // start tester server
            testServer = new TestServer("Test Server", (int)par["ServerPort"]);
            if (!testServer.StartServer("Test Server", null))
            {
                logger.LogMessage("Error", "Error starting test server.");
                return false;
            }

            // connect to PLC
            if (!Global.Simulation)
            {
                plc = new KeyencePLC();
                status = plc.ConnectToServer(settings["PlcIpAddr"], (int)par["PlcPort"]);
                if (!status)
                {
                    logger.LogMessage("Error", "Error connecting to PLC.");
                    return false;
                }

                // start PLC interface
                status = plc.StartThread("PLC Interface", null);
                if (!status)
                {
                    logger.LogMessage("Error", "Error starting PLC Interface Driver.");
                    return false;
                }
            }

            // start cube manager client
            if (par["StandaloneMode"] == 0)
            {
                cmClient = new CubeMgrClient();
                status = cmClient.StartThread("Cube Manager Client", null);
                if (!status)
                {
                    logger.LogMessage("Error", "Error starting Cube Manager Client.");
                    return false;
                }
            }
            else
            {
                logger.LogMessage("Info", "Standalone Mode: Cube Manager Client not started.");
            }

            return true;
        }

        public bool LoadAlarmList()
        {
            int cnt = 0;
            try
            {
                Global.alarmCodes = new Dictionary<string, string>();
                using (StreamReader rd = new StreamReader(Global.alarmsFile))
                {
                    do
                    {
                        string line = rd.ReadLine();
                        if (line == null) break;

                        if (line.Length == 0 || line[0] == '#') continue; // comment

                        string[] s = line.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (s.Length > 1)
                        {
                            string code = s[0].Trim();
                            string text = s[1].Trim();
                            if (code.Length > 0)
                            {
                                Global.alarmCodes.Add(code, text);
                                if (text.Length == 0)
                                {
                                    Global.logger.LogMessageEx("Error", "Alarm code {0} has blank text.", code);
                                }
                                cnt++;
                            }
                        }
                    }
                    while (true);

                }

                Global.logger.LogMessageEx("Info", "Loaded {0} alarm codes.", cnt);
                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception reading Alarms.csv: {0}", ex.Message);
                return false;
            }
        }

        public bool LoadWarningList()
        {
            int cnt = 0;
            try
            {
                Global.warnCodes = new Dictionary<string, string>();
                using (StreamReader rd = new StreamReader(Global.warningsFile))
                {
                    do
                    {
                        string line = rd.ReadLine();
                        if (line == null) break;

                        if (line.Length == 0 || line[0] == '#') continue; // comment

                        string[] s = line.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (s.Length > 1)
                        {
                            string code = s[0].Trim();
                            string text = s[1].Trim();
                            if (code.Length > 0)
                            {
                                Global.warnCodes.Add(code, text);
                                if (text.Length == 0)
                                {
                                    Global.logger.LogMessageEx("Error", "Warning code {0} has blank text.", code);
                                }
                            }

                            cnt++;
                        }
                    }
                    while (true);

                }

                Global.logger.LogMessageEx("Info", "Loaded {0} warning codes.", cnt);
                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception reading Warnings.csv: {0}", ex.Message);
                return false;
            }
        }

        public bool LoadCubeData()
        {
            bool status;
            //if (!newCubeData) return false;

            lock (lckCubeData)
            {
                if (Global.app.par["StandaloneMode"] > 0)
                {
                    List<Cube> cl = new List<Cube>();
                    status = Global.localDB.GetCubes(cl);

                    if (!status) return false;

                    if (cubes.ContainsKey("0-0"))
                    {
                        cubes["0-0"] = cl;
                    }
                    else
                    {
                        cubes.Add("0-0", cl);
                    }

                    newCubeData = false;
                    return true;
                }
                else // production
                {
                    status = Global.localDB.GetBatches(batches);
                    if (!status) return false;

                    status = Global.localDB.GetCubeSets(cubeSets);
                    if (!status) return false;

                    cubes.Clear();
                    foreach (Batch b in batches.Values)
                    {
                        List<Cube> cl = new List<Cube>();
                        status = Global.localDB.GetCubes(cl, b.BatchNum);
                        if (!status) return false;

                        cubes.Add(b.BatchNum, cl);
                    }

                    newCubeData = false;
                    return true;
                }
            }
        }

        public bool CheckBarcode(int barcode)
        {
            lock (lckCubeData)
            {
                foreach (List<Cube> cl in cubes.Values)
                {
                    foreach (Cube c in cl)
                    {
                        if (c.Barcode == barcode) return true;
                    }
                }

                return false;
            }
        }
    }
}
