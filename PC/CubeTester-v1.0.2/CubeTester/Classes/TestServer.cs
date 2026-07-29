using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using FBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CubeTester.Classes
{
    public class TestServer
    {
        public IPAddress ipaddr;

        App app;
        TcpListener server;
        int port;
        string name;

        bool running;
        bool newClient;
        bool disconnect;
        Thread thread;
        Thread[] clientThreads;
        TesterCmd[] testerCmds;
        string[] testerAddrs;

        public TestServer(string name, int port)
        {
            this.name = name;
            this.port = port;

            app = Global.app;

            clientThreads = new Thread[Global.testerMax];
            testerCmds = new TesterCmd[Global.testerMax];
            testerAddrs = new string[Global.testerMax];
            testerAddrs[0] = app.settings["TesterIpAddr1"];
            testerAddrs[1] = app.settings["TesterIpAddr2"];
            testerAddrs[2] = app.settings["TesterIpAddr3"];
            testerAddrs[3] = app.settings["TesterIpAddr4"];
            testerAddrs[4] = app.settings["TesterIpAddr5"];
            testerAddrs[5] = app.settings["TesterIpAddr6"];
        }

        public bool StartServer(string serverName, object par)
        {
            if (thread != null) return false;

            thread = new Thread(new ParameterizedThreadStart(Run));
            thread.Name = serverName;
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Normal;
            thread.Start(par);

            return true;
        }

        public void StopServer()
        {
            if (thread == null) return;

            running = false;

            //thread.Abort();
            if (thread.Join(5000))
            {
                thread = null;
            }
            else
            {
                Global.logger.LogMessage("Error", string.Format("[{0}] comm server shutdown timeout.", name));
            }
        }

        public bool ServerRunning()
        {
            if (thread == null) return false;

            return thread.IsAlive;
        }

        public void Run(object par)
        {
            int idx;

            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                running = true;
            }
            catch(Exception ex) 
            {
                Global.logger.LogMessageEx("Error", "Error starting server listening at port {0}: {1}", port, ex.Message);
                running = false;
            }

            while (running)
            {
                TcpClient client = server.AcceptTcpClient();
                IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                Global.logger.LogMessageEx("Info", "Tester connected from {0}.", remoteIpEndPoint.Address);

                // find out which tester
                idx = -1;
                for (int i=0; i<Global.testerMax; i++)
                {
                    if (remoteIpEndPoint.Address.ToString() == testerAddrs[i])
                    {
                        idx = i;
                        break;
                    }
                }

                // create thread to handle
                if (idx >= 0)
                {
                    app.clients[idx] = client;

                    ClientPar cp = new ClientPar()
                    {
                        client = client,
                        testerNum = idx
                    };

                    Thread t = new Thread(new ParameterizedThreadStart(TestHandler));
                    clientThreads[idx] = t;
                    t.Start(cp);
                }

                Thread.Sleep(50);
            }
        }

        public void TestHandler(object par)
        {
            bool connected = false;
            string recvMsg;
            string sendMsg;
            string logDomain;

            ClientPar cp = (ClientPar)par;
            TcpClient client = cp.client;
            int testerNum = cp.testerNum;
            logDomain = $"Tester{testerNum + 1}";
            Random rand = new Random();

            NetworkStream stream = client.GetStream();
            client.ReceiveBufferSize = 1024;
            client.ReceiveTimeout = 100;
            client.SendBufferSize = 1024;
            client.SendTimeout = 1000;

            StreamWriter wr = new StreamWriter(client.GetStream(), Encoding.ASCII);
            wr.NewLine = "\n";
            wr.AutoFlush = true;

            StreamReader rd = new StreamReader(client.GetStream(), Encoding.ASCII);
            

            Global.logger.LogMessageEx("Info", "Started Handler for Tester {0} at {1}.", testerNum+1, 
                client.Client.RemoteEndPoint.ToString());

            Global.app.clientStatus[testerNum] = "Connected";

            while (running)
            {
                if (client.GetState() != System.Net.NetworkInformation.TcpState.Established)
                {
                    Global.logger.LogMessageEx("Info", "Tester {0} at {1} disconnected.", 
                        testerNum+1, client.Client.RemoteEndPoint.ToString());
                    Global.app.clients[testerNum] = null;
                    Global.app.clientStatus[testerNum] = "Disconnected";
                    break;
                }

                while (stream.DataAvailable)
                {
                    if (!connected)
                    {

                        // wait for connect signal
                        try
                        {
                            recvMsg = rd.ReadLine();
                        }
                        catch
                        {
                            break; // break for while
                        }

                        TesterRecvConnect cmsg = JsonConvert.DeserializeObject<TesterRecvConnect>(recvMsg);
                        Global.logger.LogMessageEx(logDomain, "Recv: {0}", recvMsg);

                        string json = "{\"Command\":\"Connect\",\"Return\":{\"Code\":\"0\",\"Content\":{\"Version\":\"1.4\",\"UseReqResult\":\"True\",\"UseIdleData\":\"False\",\"UseStartReturn\":\"False\"}}}";
                        wr.WriteLine(json);
                        wr.Flush();

                        Global.logger.LogMessageEx(logDomain, "Send: {0}", json);

                        connected = true;
                    }
                    else
                    {
                        try
                        {
                            recvMsg = rd.ReadLine();
                        }
                        catch
                        {
                            break; // break for while;
                        }

                        app.testerReply[testerNum] = recvMsg;
                        if (false && !app.testerIsTesting[testerNum])
                        {
                            Global.logger.LogMessageEx(logDomain, "Recv: {0}", recvMsg);
                        }

                        JObject jobj = (JObject)JsonConvert.DeserializeObject(recvMsg);
                        if (jobj["Data"] != null && jobj["Data"]["SYLZ"] != null)
                        {
                            double force;
                            if (double.TryParse(jobj["Data"]["SYLZ"].ToString(), out force))
                            {
                                app.testerForce[testerNum] = force / 1000.0;
                            }
                            else
                            {
                                app.testerForce[testerNum] = 0;
                            }
                        }
                        else if (jobj["Command"] != null)
                        {
                            if (jobj["Command"].ToString() == "Start" &&
                                jobj["Return"]["Code"] != null)
                            {
                                int code;
                                if (int.TryParse(jobj["Return"]["Code"].ToString(), out code))
                                {
                                    app.testerStartAck[testerNum] = (code == 0);
                                }
                                else
                                {
                                    app.testerStartAck[testerNum] = false;
                                }
                            }
                            Global.logger.LogMessageEx(logDomain, "Recv: {0}", recvMsg);
                        }
                        else if (jobj["Stop"] != null)
                        {
                            Global.logger.LogMessageEx(logDomain, "Recv: {0}", recvMsg);

                            app.clientStatus[testerNum] = "Test Stopped";
                            Global.logger.LogMessageEx("Info", "Tester {0} Stopped", testerNum + 1);
                            app.testerIsTesting[testerNum] = false;
                        }
                        else if (jobj["Result"] != null) 
                        {
                            bool status = false;
                            int testId;
                            double force_kN;

                            Global.logger.LogMessageEx(logDomain, "Recv: {0}", recvMsg);

                            if (jobj["Result"]["SYBH"] != null && jobj["Result"]["ZDL"] != null)
                            {
                                app.clientStatus[testerNum] = "Test Completed";

                                string testIdStr = jobj["Result"]["SYBH"].ToString();
                                testIdStr = testIdStr.Substring(0, testIdStr.Length - 3); // remove "-XX"
                                status = int.TryParse(testIdStr, out testId);
                                if (status)
                                {
                                    if (app.testerPar[testerNum].testID != testId)
                                    {
                                        Global.logger.LogMessageEx("Error", "Tester {0}: Returned Test ID is {1} when it should be {2}.",
                                            testerNum + 1, testId, app.testerPar[testerNum].testID);
                                        status = false;
                                    }
                                }

                                if (status)
                                {
                                    status = double.TryParse(jobj["Result"]["ZDL"].ToString(), out force_kN);
                                    if (status)
                                    {
                                        app.testerForce[testerNum] = force_kN;
                                        app.cubeAtTester[testerNum].MeasuredMaxForce = force_kN;
                                        app.cubeAtTester[testerNum].MeasuredStrength = force_kN * 1000.0 / 
                                            Util.Sqr(app.testerPar[testerNum].dimension);
                                        Global.logger.LogMessageEx("Info", "Tester {0}: Test completed, Dim={1}mm, Force={2:0.0}kN, Strength={3:0.0} N/mm2",
                                            testerNum+1, app.testerPar[testerNum].dimension, app.cubeAtTester[testerNum].MeasuredMaxForce, app.cubeAtTester[testerNum].MeasuredStrength);

                                        app.testerResultReady[testerNum] = true;
                                    }
                                }
                            }

                        }

                    }

                    Thread.Sleep(0);
                }
                
                // process commands
                if (testerCmds[testerNum] == TesterCmd.SetPar)
                {
                    TesterPar p = app.testerPar[testerNum];
                    string json = "{\"GKSelect\":{\"SYBH\":\"$id$\",\"SYLQ\":$age$,\"SYSL\":1,\"PaiHao\":\"HRB500\",\"QDDJ\":\"C$grade$\",\"SYZJ\":\"$dimension$\"}}";
                    p.testID = app.cubeAtTester[testerNum].Barcode;
                    string testIdStr = p.testID.ToString() + "-" + rand.Next(255).ToString("X2");
                    json = json.Replace("$id$", testIdStr);
                    json = json.Replace("$age$", p.age.ToString());
                    json = json.Replace("$grade$", p.grade.ToString());
                    json = json.Replace("$dimension$", p.dimension.ToString());

                    wr.WriteLine(json);
                    wr.Flush();

                    Global.logger.LogMessageEx(logDomain, "Send: {0}", json);

                    testerCmds[testerNum] = TesterCmd.None;
                }
                else if (testerCmds[testerNum] == TesterCmd.Start)
                {
                    string json = "{\"Start\":null}";
                    wr.WriteLine(json);
                    wr.Flush();

                    app.clientStatus[testerNum] = "Start Test";
                    app.testerIsTesting[testerNum] = true;

                    Global.logger.LogMessageEx(logDomain, "Send: {0}", json);
                    testerCmds[testerNum] = TesterCmd.None;
                }
                else if (testerCmds[testerNum] == TesterCmd.Stop)
                {
                    string json = "{\"Stop\":null}";
                    wr.WriteLine(json);
                    wr.Flush();

                    app.clientStatus[testerNum] = "Stop Test";
                    app.testerIsTesting[testerNum] = false;

                    Global.logger.LogMessageEx(logDomain, "Send: {0}", json);
                    testerCmds[testerNum] = TesterCmd.None;
                }

                Thread.Sleep(10);
            }
        }

        public bool SetPar(int testerNum)
        {
            if (testerNum > 0 && testerNum >= testerCmds.Length) return false;

            app.testerReply[testerNum] = "";
            testerCmds[testerNum] = TesterCmd.SetPar;

            long t0 = Util.stopwatch.ElapsedMilliseconds;
            while (Util.stopwatch.ElapsedMilliseconds - t0 < 5000)
            {
                string s = app.testerReply[testerNum];
                if (s.Length > 0)
                {
                    JObject jobj = (JObject)JsonConvert.DeserializeObject(s);
                    if (jobj["Command"] != null && jobj["Command"].ToString() == "GKSelect" &&
                        jobj["Return"]["Code"] != null)
                    {
                        int code;
                        if (int.TryParse(jobj["Return"]["Code"].ToString(), out code))
                        {
                            return (code == 0);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Application.DoEvents();
                }
            }

            return false;
        }

        public bool StartTest(int testerNum)
        {
            if (testerNum < 0 && testerNum >= testerCmds.Length) return false;

            app.testerReply[testerNum] = "";
            app.testerStartAck[testerNum] = false;
            testerCmds[testerNum] = TesterCmd.Start;

            long t0 = Util.stopwatch.ElapsedMilliseconds;
            while (Util.stopwatch.ElapsedMilliseconds - t0 < 120_000)
            {
                if (app.testerStartAck[testerNum])
                {
                    return true;
                }
                

                Thread.Sleep(0);
            }

            return false;
        }

        public bool StopTest(int testerNum)
        {
            if (testerNum > 0 && testerNum >= testerCmds.Length) return false;

            app.testerReply[testerNum] = "";
            testerCmds[testerNum] = TesterCmd.Stop;

            long t0 = Util.stopwatch.ElapsedMilliseconds;
            while (Util.stopwatch.ElapsedMilliseconds - t0 < 5000)
            {
                string s = app.testerReply[testerNum];
                if (s.Length > 0)
                {
                    JObject jobj = (JObject)JsonConvert.DeserializeObject(s);
                    if (jobj["Command"] != null && jobj["Command"].ToString() == "Stop" &&
                        jobj["Return"]["Code"] != null)
                    {
                        int code;
                        if (int.TryParse(jobj["Return"]["Code"].ToString(), out code))
                        {
                            return (code == 0);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Application.DoEvents();
                }
            }

            return false;
        }
    }
}
