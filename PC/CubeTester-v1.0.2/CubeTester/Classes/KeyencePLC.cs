using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FBase;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CubeTester.Classes
{
    public class KeyencePLC : TcpSocketClient
    {
        string[] sep = new string[] {" ", "\r", "\n"};
        Thread thread;
        bool runThread;

        uint inSize = 48; // words
        uint outSize = 4; // words
        uint[] inputs;
        uint[] outputs;

        // inputs from plc
        public bool plcHeartbeat;
        public bool checkBarcode;
        public bool cubeData123Avail;
        public bool cubeData456Avail;
        public bool[] startTester = new bool[Global.testerMax];
        public bool[] testerResultRead = new bool[Global.testerMax];
        public bool[] testerClearError = new bool[Global.testerMax];

        public uint[] checkBarcodeWord = new uint[5];
        public uint dataTesterNum123;
        public uint[] dimX123 = new uint[6]; // X1-X6
        public uint[] dimY123 = new uint[6]; // Y1-Y6
        public uint weight123; // in g
        public uint[] barcodeWord123 = new uint[5];
        public uint dataTesterNum456;
        public uint[] dimX456 = new uint[6]; // X1-X6
        public uint[] dimY456 = new uint[6]; // Y1-Y6
        public uint weight456; // in g
        public uint[] barcodeWord456 = new uint[5];

        public string checkBarcodeStr; // derived from barcodeWord
        public int checkBarcodeInt; // converted from barcodeStr
        public string barcodeStr; // derived from barcodeWord
        public int barcodeInt; // converted from barcodeStr

        // outputs to plc
        public bool pcHeartbeat;
        public bool[] testerOnline = new bool[Global.testerMax];
        public bool[] testerError = new bool[Global.testerMax];
        public bool checkBarcodeResultReady;
        public bool barcodeOK;
        public bool cubeData123Read;
        public bool cubeData456Read;
        public bool[] testerStarted = new bool[Global.testerMax];
        public bool[] testerResultAvail = new bool[Global.testerMax];
        public bool[] testerResultPass = new bool[Global.testerMax];
        public bool[] testerRapidOn = new bool[Global.testerMax];
        public bool[] isBadCube = new bool[Global.testerMax];

        // vars
        bool lastPlcHeartbeat;

        int bcStep;
        int bcNextStep;
        int d1Step;
        int d1NextStep;
        int d2Step;
        int d2NextStep;
        int[] tStep = new int[Global.testerMax];
        int[] tNextStep = new int[Global.testerMax];
        int[] startRetries = new int[Global.testerMax];

        public KeyencePLC() : base(Global.logger)
        {
            //eom = '\r';

            inputs = new uint[inSize];
            outputs = new uint[outSize];

        }

        public bool Read(string addr, ref uint data)
        {
            bool status = Send("RD {0}\r", addr);
            if (!status) return false;

            string reply = Receive();
            if (reply == null) return false;

            if (!uint.TryParse(reply, out data)) return false;

            return true;
        }

        public bool ReadMulti(string addr, uint cnt, ref uint[] data)
        {
            if (data.Length < cnt) return false;

            bool status = Send("RDS {0} {1}\r", addr, cnt);
            if (!status) return false;

            string reply = Receive();
            if (reply == null) return false;

            string[] s = reply.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length < cnt) return false;

            status = true;
            data = new uint[cnt];
            for (int i=0; i<cnt; i++)
            {
                if (!uint.TryParse(s[i], out data[i]))
                {
                    status = false;
                }

            }

            return status;
        }

        public bool Write(string addr, uint data)
        {
            bool status = Send("WR {0} {1}\r", addr, data);
            if (!status) return false;

            string reply = Receive();
            if (reply == null) return false;

            return (reply == "OK");
        }

        public bool WriteMulti(string addr, uint cnt, uint[] data)
        {
            if (data.Length < cnt) return false;

            StringBuilder cmd = new StringBuilder();
            cmd.AppendFormat("WRS {0} {1}", addr, cnt);
            for (int i=0; i<cnt; i++)
            {
                cmd.AppendFormat(" {0}", data[i]);
            }
            cmd.Append("\r");

            bool status = Send(cmd.ToString());
            if (!status) return false;

            string reply = Receive();
            if (reply == null) return false;

            return (reply == "OK");
        }

        public bool StartThread(string threadName, object par)
        {
            if (thread != null) return false;

            thread = ThreadUtil.StartThread(new ParameterizedThreadStart(PollTask), null, threadName, ThreadPriority.Normal);
            if (thread == null) return false;

            return true;
        }

        public void StopThread()
        {
            if (thread == null) return;

            runThread = false;

            if (thread.Join(5000))
            {
                //thread.Abort();
                thread = null;
            }
            else
            {
                Global.logger.LogMessageEx("Error", "Error shutting down PLC poll thread.");
            }
        }

        private void MapInputs()
        {
            plcHeartbeat = Util.ReadBit(inputs[0], 0);
            checkBarcode = Util.ReadBit(inputs[0], 1);
            cubeData123Avail = Util.ReadBit(inputs[0], 2);
            cubeData456Avail = Util.ReadBit(inputs[0], 3);

            startTester[0] = Util.ReadBit(inputs[1], 4);
            startTester[1] = Util.ReadBit(inputs[1], 5);
            startTester[2] = Util.ReadBit(inputs[1], 6);
            startTester[3] = Util.ReadBit(inputs[1], 7);
            startTester[4] = Util.ReadBit(inputs[1], 8);
            startTester[5] = Util.ReadBit(inputs[1], 9);
            testerResultRead[0] = Util.ReadBit(inputs[1], 10);
            testerResultRead[1] = Util.ReadBit(inputs[1], 11);
            testerResultRead[2] = Util.ReadBit(inputs[1], 12);
            testerResultRead[3] = Util.ReadBit(inputs[1], 13);
            testerResultRead[4] = Util.ReadBit(inputs[1], 14);
            testerResultRead[5] = Util.ReadBit(inputs[1], 15);

            testerClearError[0] = Util.ReadBit(inputs[2], 0);
            testerClearError[1] = Util.ReadBit(inputs[2], 1);
            testerClearError[2] = Util.ReadBit(inputs[2], 2);
            testerClearError[3] = Util.ReadBit(inputs[2], 3);
            testerClearError[4] = Util.ReadBit(inputs[2], 4);
            testerClearError[5] = Util.ReadBit(inputs[2], 5);

            checkBarcodeWord[0] = inputs[5];
            checkBarcodeWord[1] = inputs[6];
            checkBarcodeWord[2] = inputs[7];
            checkBarcodeWord[3] = inputs[8];
            checkBarcodeWord[4] = inputs[9];

            dataTesterNum123 = inputs[10];
            dimX123[0] = inputs[11];
            dimX123[1] = inputs[12];
            dimX123[2] = inputs[13];
            dimX123[3] = inputs[14];
            dimX123[4] = inputs[15];
            dimX123[5] = inputs[16];

            dimY123[0] = inputs[17];
            dimY123[1] = inputs[18];
            dimY123[2] = inputs[19];
            dimY123[3] = inputs[20];
            dimY123[4] = inputs[21];
            dimY123[5] = inputs[22];
            weight123 = inputs[23];
            barcodeWord123[0] = inputs[24];
            barcodeWord123[1] = inputs[25];
            barcodeWord123[2] = inputs[26];
            barcodeWord123[3] = inputs[27];
            barcodeWord123[4] = inputs[28];

            dataTesterNum456 = inputs[29];
            dimX456[0] = inputs[30];
            dimX456[1] = inputs[31];
            dimX456[2] = inputs[32];
            dimX456[3] = inputs[33];
            dimX456[4] = inputs[34];
            dimX456[5] = inputs[35];

            dimY456[0] = inputs[36];
            dimY456[1] = inputs[37];
            dimY456[2] = inputs[38];
            dimY456[3] = inputs[39];
            dimY456[4] = inputs[40];
            dimY456[5] = inputs[41];
            weight456 = inputs[42];
            barcodeWord456[0] = inputs[43];
            barcodeWord456[1] = inputs[44];
            barcodeWord456[2] = inputs[45];
            barcodeWord456[3] = inputs[46];
            barcodeWord456[4] = inputs[47];
        }

        public void MapOutputs()
        {
            Util.SetBit(ref outputs[0], 0, pcHeartbeat);
            Util.SetBit(ref outputs[0], 1, testerOnline[0]);
            Util.SetBit(ref outputs[0], 2, testerOnline[1]);
            Util.SetBit(ref outputs[0], 3, testerOnline[2]);
            Util.SetBit(ref outputs[0], 4, testerOnline[3]);
            Util.SetBit(ref outputs[0], 5, testerOnline[4]);
            Util.SetBit(ref outputs[0], 6, testerOnline[5]);
            Util.SetBit(ref outputs[0], 7, testerError[0]);
            Util.SetBit(ref outputs[0], 8, testerError[1]);
            Util.SetBit(ref outputs[0], 9, testerError[2]);
            Util.SetBit(ref outputs[0], 10, testerError[3]);
            Util.SetBit(ref outputs[0], 11, testerError[4]);
            Util.SetBit(ref outputs[0], 12, testerError[5]);
            Util.SetBit(ref outputs[0], 13, checkBarcodeResultReady);
            Util.SetBit(ref outputs[0], 14, barcodeOK);

            Util.SetBit(ref outputs[1], 0, cubeData123Read);
            Util.SetBit(ref outputs[1], 1, cubeData456Read);

            Util.SetBit(ref outputs[1], 4, testerStarted[0]);
            Util.SetBit(ref outputs[1], 5, testerStarted[1]);
            Util.SetBit(ref outputs[1], 6, testerStarted[2]);
            Util.SetBit(ref outputs[1], 7, testerStarted[3]);
            Util.SetBit(ref outputs[1], 8, testerStarted[4]);
            Util.SetBit(ref outputs[1], 9, testerStarted[5]);
            Util.SetBit(ref outputs[1], 10, testerResultAvail[0]);
            Util.SetBit(ref outputs[1], 11, testerResultAvail[1]);
            Util.SetBit(ref outputs[1], 12, testerResultAvail[2]);
            Util.SetBit(ref outputs[1], 13, testerResultAvail[3]);
            Util.SetBit(ref outputs[1], 14, testerResultAvail[4]);
            Util.SetBit(ref outputs[1], 15, testerResultAvail[5]);

            Util.SetBit(ref outputs[2], 0, testerResultPass[0]);
            Util.SetBit(ref outputs[2], 1, testerResultPass[1]);
            Util.SetBit(ref outputs[2], 2, testerResultPass[2]);
            Util.SetBit(ref outputs[2], 3, testerResultPass[3]);
            Util.SetBit(ref outputs[2], 4, testerResultPass[4]);
            Util.SetBit(ref outputs[2], 5, testerResultPass[5]);
            Util.SetBit(ref outputs[2], 6, testerRapidOn[0]);
            Util.SetBit(ref outputs[2], 7, testerRapidOn[1]);
            Util.SetBit(ref outputs[2], 8, testerRapidOn[2]);
            Util.SetBit(ref outputs[2], 9, testerRapidOn[3]);
            Util.SetBit(ref outputs[2], 10, testerRapidOn[4]);
            Util.SetBit(ref outputs[2], 11, testerRapidOn[5]);

        }

        private async void PollTask(object par)
        {
            bool status;
            int errcnt = 0;

            runThread = true;

            long t0 = Global.stopwatch.ElapsedMilliseconds;
            long t1 = Global.stopwatch.ElapsedMilliseconds;

            while (runThread)
            {
                try
                {
                    if (IsConnected())
                    {
                        status = ReadMulti("DM1000", inSize, ref inputs);
                        if (status)
                        {
                            errcnt = 0;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Error", "Error reading PLC memory.");
                            errcnt++;
                        }
                        MapInputs();

                        // PLC interface logic
                        PCHeartBeat(ref t0);
                        CheckPLCHeartbeat(ref t1);
                        HandleCheckBarcode();
                        HandleCubeData123();
                        HandleCubeData456();
                        await HandleTester(0);
                        await HandleTester(1);
                        await HandleTester(2);
                        await HandleTester(3);
                        await HandleTester(4);
                        await HandleTester(5);

                        //// -------------------

                        MapOutputs();
                        status = WriteMulti("DM1100", outSize, outputs);
                        if (status)
                        {
                            errcnt = 0;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Error", "Error writing PLC memory.");
                            errcnt++;
                        }
                    }
                    else
                    {
                        Global.app.plcStatus = false;
                    }

                    if (errcnt > 3 || !IsConnected())
                    { // PLC connection error, try reconnecting
                        Global.logger.LogMessageEx("Info", "Reconnecting to PLC...");
                        status = ReconnectToServer();
                        if (status)
                        {
                            Global.logger.LogMessageEx("Info", "Reconnected to PLC.");
                            errcnt = 0;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Info", "Failed to reconnect to PLC.");
                        }
                    }

                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Exception reading/writing PLC memory.");
                }

                Thread.Sleep(50);
            }


        }

        private async Task HandleTester(int tNum)
        {
            bool status;

            if (Global.app.testerResultReady == null)
            {
                Global.app.testerResultReady = new bool[Global.testerMax];
            }

            testerOnline[tNum] = Global.app.clients[tNum].GetState() == System.Net.NetworkInformation.TcpState.Established;
            if (!testerOnline[tNum])
            {
                return; // don't run if not online
            }

            // turn off rapid if force above cutoff
            if (Global.app.testerForce[tNum] >= Global.app.par["RapidForceCutoff"] && testerRapidOn[tNum])
            {
                testerRapidOn[tNum] = false;
                Global.logger.LogMessageEx("Info", "Tester {0} Rapid Off.", tNum + 1);
            }

            tStep[tNum] = tNextStep[tNum];

            switch(tStep[tNum])
            {
                case 0:
                    if (startTester[tNum])
                    {
                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, $"T{tNum + 1}_");

                        testerResultAvail[tNum] = false;
                        testerStarted[tNum] = false;

                        // set parameters
                        status = Global.app.testServer.SetPar(tNum);
                        if (!status)
                        {
                            Global.UIContext.Post(Global.mainForm.SetAlarm, $"T{tNum + 1}_SetParFailed");
                            Global.logger.LogMessageEx("Error", "Error Setting Parameters for Tester {0}", tNum + 1);
                            testerError[tNum] = true;
                            tNextStep[tNum] = 4000;
                            break;
                        }

                        if (status)
                        {
                            tNextStep[tNum] = 100;
                        }
                    }
                    break;

                case 100: // start test
                    status = Global.app.testServer.StartTest(tNum);
                    if (status)
                    {
                        Global.app.testerForce[tNum] = 0; // reset force

                        Cube c = Global.localDB.GetCube(Global.app.cubeAtTester[tNum].Barcode);
                        if (c != null)
                        {
                            c.TesterId = tNum + 1;
                            Global.localDB.UpdateCube(c);
                            Global.app.newCubeData = true;
                            tNextStep[tNum] = 200;
                            status = true;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Error", "Error retrieving cube data for barcode {1} at {0}", 
                                tNum + 1, Global.app.cubeAtTester[tNum].Barcode);
                            isBadCube[tNum] = true;
                            tNextStep[tNum] = 200;
                            status = true; // test anyway
                        }

                        startRetries[tNum] = 0;
                    }
                    else if (++startRetries[tNum] < 3)
                    {
                        break; // retry again
                    }

                    if (!status)
                    {
                        Global.UIContext.Post(Global.mainForm.SetAlarm, $"T{tNum + 1}_StartFailed");
                        Global.logger.LogMessageEx("Error", "Error Starting Test at Tester {0}", tNum + 1);
                        testerError[tNum] = true;
                        Global.app.clientStatus[tNum] = "Start Error";
                        tNextStep[tNum] = 4000;
                        break;
                    }
                    break;

                case 200: // wait for testing
                    if (Global.app.testerIsTesting[tNum])
                    {
                        testerStarted[tNum] = true;
                        testerRapidOn[tNum] = true; // turn on Rapid on tester
                        Global.app.testerResultReady[tNum] = false;
                        Global.logger.LogMessageEx("Info", "Tester {0} Rapid On.", tNum + 1);

                        tNextStep[tNum] = 300;
                    }
                    else if (!startTester[tNum])
                    {
                        status = Global.app.testServer.StopTest(tNum);
                        if (status)
                        {
                            Global.logger.LogMessageEx("Error", "Tester {0} Stopped by PLC.", tNum+1);
                            tNextStep[tNum] = 0;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Error", "Tester {0} Stop by PLC unsuccessful.", tNum + 1);
                            tNextStep[tNum] = 0;
                        }
                    }
                    break;

                case 300: // wait for done
                    if (!Global.app.testerIsTesting[tNum] && Global.app.testerResultReady[tNum])
                    {
                        testerStarted[tNum] = false;
                        status = true;

                        Cube c = new Cube();

                        try
                        {
                            CubeMgrClient.smLock.Wait();

                            c = Global.localDB.GetCube(Global.app.cubeAtTester[tNum].Barcode);
                            if (c != null && !isBadCube[tNum])
                            {
                                status = EvaluateCubeLocal(tNum, c);

                                // server upload and eval
                                if (Global.app.cmClient != null)
                                {

                                    Cube retCube = await Global.app.cmClient.UploadCube(c);
                                    if (retCube != null)
                                    {
                                        //status = (retCube.StatusCode == null || retCube.StatusCode.Length == 0); // && retCube.TestResult == 1;
                                        bool updateStatus = Global.localDB.UpdateCube(retCube);
                                        if (!updateStatus)
                                        {
                                            Global.logger.LogMessageEx("Error", "Error updating server eval for cube {0}.", c.Barcode);
                                        }

                                        Global.app.newCubeData = true;
                                    }

                                }
                            }
                            else // unknown cube
                            {
                                c = new Cube();
                                c.MeasuredMaxForce = Global.app.cubeAtTester[tNum].MeasuredMaxForce;
                                c.MeasuredStrength = Global.app.cubeAtTester[tNum].MeasuredStrength;
                                c.ActualTestDate = DateTime.Now;

                                c.TesterId = tNum + 1;
                                c.StatusCode = "";

                                if (Global.app.par["StandaloneMode"] > 0)
                                {
                                    c.TesterId = tNum + 1;
                                    c.StatusCode = "";
                                    c.TestResult = (int)CubeTestResult.Pass; // pass it first

                                    status = Global.localDB.UpdateCube(c);
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error updating local eval for cube {0}.", c.Barcode);
                                    }

                                    Global.app.newCubeData = true;
                                    status = true;
                                }
                                else
                                {
                                    LogBadCubeData(c);
                                    c.TestResult = (int)CubeTestResult.Fail; // fail it for check
                                }
                            }

                            CubeMgrClient.smLock.Release();
                        }
                        catch (Exception ex)
                        {
                            CubeMgrClient.smLock.Release();
                            Global.logger.LogMessageEx("Error", "Exception: {0}\r\nStack Trace:\r\n{1}", ex.Message, ex.StackTrace);
                            status = false;
                        }

                        if (isBadCube[tNum])
                        {
                            Global.logger.LogMessageEx("Info", "Setting Result to Fail because barcode is not in database.");
                            testerResultPass[tNum] = false;
                        }
                        else
                        {
                            testerResultPass[tNum] = c.TestResult == (int)CubeTestResult.Pass;
                        }

                        Global.logger.LogMessageEx("Info", "Cube at Tester {0} {1}", tNum + 1, testerResultPass[tNum] ? "Passed" : "Failed");

                        Global.app.testerResultReady[tNum] = false;
                        tNextStep[tNum] = 400;
                    }
                    else if (!startTester[tNum])
                    {
                        status = Global.app.testServer.StopTest(tNum);
                        if (status)
                        {
                            Global.logger.LogMessageEx("Error", "Tester {0} Stopped by PLC.", tNum + 1);
                            tNextStep[tNum] = 0;
                        }
                        else
                        {
                            Global.logger.LogMessageEx("Error", "Tester {0} Stop by PLC unsuccessful.", tNum + 1);
                            tNextStep[tNum] = 0;
                        }
                    }
                    break;

                case 400: // result avail
                    testerResultAvail[tNum] = true;
                    if (testerResultRead[tNum] && !startTester[tNum])
                    {
                        testerResultAvail[tNum] = false;
                        tNextStep[tNum] = 0;
                    }
                    break;

                case 4000: // wait for clear error
                    if (testerClearError[tNum])
                    {
                        Global.app.clientStatus[tNum] = "Error Cleared";
                        testerError[tNum] = false;
                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, $"T{tNum + 1}_");
                        tNextStep[tNum] = 0;
                    }
                    else if (Global.app.testerStartAck[tNum]) // suddenly start testing
                    {
                        Global.app.clientStatus[tNum] = "Tester Start Ack. Error Cleared";
                        testerError[tNum] = false;
                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, $"T{tNum + 1}_");
                        tNextStep[tNum] = 300;
                    }
                    break;
            }
        }

        private bool EvaluateCubeLocal(int tNum, Cube c)
        {
            bool status = false;

            Batch batch = Global.localDB.GetBatch(c.ScoNum, c.BatchId);
            if (batch != null)
            {
                c.MeasuredMaxForce = Global.app.cubeAtTester[tNum].MeasuredMaxForce;
                c.MeasuredStrength = Global.app.cubeAtTester[tNum].MeasuredStrength;
                c.ActualTestDate = DateTime.Now;

                CubeSet cs = Global.localDB.GetCubeSet(batch.CubeSetId);
                if (cs != null)
                {
                    double cgrade = cs.ConcreteGrade;
                    c.TesterId = tNum + 1;
                    c.StatusCode = "";
                    c.TestResult = (int)CubeTestResult.Pass; // pass it first

                    if (batch.TestAge >= 28 && c.MeasuredStrength < cgrade)
                    {
                        //c.StatusCode += "A";
                        c.TestResult = (int)CubeTestResult.Pending; // set it pending for server to eval
                    }

                    status = Global.localDB.UpdateCube(c);
                    if (!status)
                    {
                        Global.logger.LogMessageEx("Error", "Error updating local eval for cube {0}.", c.Barcode);
                    }

                    Global.app.newCubeData = true;
                }
            }

            return status;
        }

        private void PCHeartBeat(ref long t0)
        {
            // output heartbeat
            if (pcHeartbeat && Global.stopwatch.ElapsedMilliseconds - t0 >= 1000)
            {
                pcHeartbeat = false;
                t0 = Global.stopwatch.ElapsedMilliseconds;
            }
            else if (!pcHeartbeat && Global.stopwatch.ElapsedMilliseconds - t0 >= 1000)
            {
                pcHeartbeat = true;
                t0 = Global.stopwatch.ElapsedMilliseconds;
            }
        }

        private void CheckPLCHeartbeat(ref long t1)
        {
            if (plcHeartbeat == lastPlcHeartbeat)
            {
                if (Global.stopwatch.ElapsedMilliseconds - t1 > 3000)
                {
                    Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_LostComms");
                    Global.app.plcStatus = false;
                }

            }
            else // not equal
            {
                t1 = Global.stopwatch.ElapsedMilliseconds;
                Global.app.plcStatus = true;

                if (Global.alarmActive == "PLC_LostComms")
                {
                    Global.UIContext.Post(Global.mainForm.ClearAlarm, "PLC_LostComms");
                }
            }
        }


        private void HandleCheckBarcode()
        {
            bool status;

            bcStep = bcNextStep;

            switch (bcStep)
            {
                case 0:
                    if (checkBarcode)
                    {
                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, "PLC_");

                        checkBarcodeResultReady = false;
                        Global.UIContext.Post(Global.mainForm.ClearAlarm, null);

                        StringBuilder sb = new StringBuilder("          ");
                        sb[0] = (char)(checkBarcodeWord[0] >> 8);
                        sb[1] = (char)(checkBarcodeWord[0] & 0xFF);
                        sb[2] = (char)(checkBarcodeWord[1] >> 8);
                        sb[3] = (char)(checkBarcodeWord[1] & 0xFF);
                        sb[4] = (char)(checkBarcodeWord[2] >> 8);
                        sb[5] = (char)(checkBarcodeWord[2] & 0xFF);
                        sb[6] = (char)(checkBarcodeWord[3] >> 8);
                        sb[7] = (char)(checkBarcodeWord[3] & 0xFF);
                        sb[8] = (char)(checkBarcodeWord[4] >> 8);
                        sb[9] = (char)(checkBarcodeWord[4] & 0xFF);

                        checkBarcodeStr = sb.ToString();
                        status = int.TryParse(checkBarcodeStr, out checkBarcodeInt);
                        if (!status)
                        {
                            Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_BadBarcode");
                            barcodeOK = false;
                            status = false;

                            Global.logger.LogMessageEx("Error", "Bad barcode from PLC.");
                        }

                        if (status)
                        {
                            Global.app.LoadCubeData();
                            barcodeOK = Global.app.CheckBarcode(checkBarcodeInt) ||
                                        Global.app.par["StandaloneMode"] > 0;
                            Global.logger.LogMessageEx("Info", "Barcode {0:D8} checked: {1}.", checkBarcodeInt, barcodeOK ? "OK" : "Reject");
                        }

                        if (status)
                        {
                            bcNextStep = 100;
                        }
                        else
                        {
                            bcNextStep = 4000;
                        }
                    }
                    break;

                case 100:
                    checkBarcodeResultReady = true;
                    if (!checkBarcode)
                    {
                        checkBarcodeResultReady = false;
                        bcNextStep = 0;
                    }
                    break;

                case 4000: // wait for alarm reset
                    if (Global.alarmActive.Length == 0)
                    {
                        bcNextStep = 0;
                    }
                    break;
            }
        }

        private void HandleCubeData123()
        {
            bool status;

            d1Step = d1NextStep;
            switch(d1Step)
            {
                case 0:
                    if (cubeData123Avail)
                    {

                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, "PLC_");

                        cubeData123Read = false;

                        status = true;
                        if (dataTesterNum123 == 0 || dataTesterNum123 > Global.testerMax)
                        {
                            Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_BadDimTester");
                            Global.logger.LogMessageEx("Error", "Bad Dimension Tester Number From PLC");
                            status = false;
                        }

                        if (status)
                        {
                            uint idx = dataTesterNum123 - 1;
                            isBadCube[idx] = false;

                            Global.app.cubeAtTester[idx].MeasuredDimX1 = dimX123[0] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX2 = dimX123[1] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX3 = dimX123[2] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX4 = dimX123[3] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX5 = dimX123[4] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX6 = dimX123[5] * 0.01;

                            Global.app.cubeAtTester[idx].MeasuredDimY1 = dimY123[0] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY2 = dimY123[1] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY3 = dimY123[2] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY4 = dimY123[3] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY5 = dimY123[4] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY6 = dimY123[5] * 0.01;

                            Global.app.cubeAtTester[idx].AvgDimension =
                                (dimX123[0] + dimX123[1] + dimX123[2] + dimX123[3] + dimX123[4] + dimX123[5] + 
                                 dimY123[0] + dimY123[1] + dimY123[2] + dimY123[3] + dimY123[4] + dimY123[5]) * 0.01 / 12.0;
                            Global.app.cubeAtTester[idx].MeasuredWeight = weight123 * 0.001;
                            Global.app.cubeAtTester[idx].MeasuredDensity = Global.app.cubeAtTester[idx].AvgDimension > 0 ?
                                Global.app.cubeAtTester[idx].MeasuredWeight / Util.Sqr(Global.app.cubeAtTester[idx].AvgDimension / 1000.0) : 0;

                            StringBuilder sb = new StringBuilder("          ");
                            sb[0] = (char)(barcodeWord123[0] >> 8);
                            sb[1] = (char)(barcodeWord123[0] & 0xFF);
                            sb[2] = (char)(barcodeWord123[1] >> 8);
                            sb[3] = (char)(barcodeWord123[1] & 0xFF);
                            sb[4] = (char)(barcodeWord123[2] >> 8);
                            sb[5] = (char)(barcodeWord123[2] & 0xFF);
                            sb[6] = (char)(barcodeWord123[3] >> 8);
                            sb[7] = (char)(barcodeWord123[3] & 0xFF);
                            sb[8] = (char)(barcodeWord123[4] >> 8);
                            sb[9] = (char)(barcodeWord123[4] & 0xFF);

                            barcodeStr = sb.ToString(); // for now
                            status = int.TryParse(barcodeStr, out barcodeInt);
                            if (!status)
                            {
                                Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_BadBarcode");
                                barcodeOK = false;
                                status = false;

                                Global.logger.LogMessageEx("Error", "Bad barcode from PLC at Tester {0}.", dataTesterNum123);
                            }

                            if (status)
                            {
                                Global.app.cubeAtTester[idx].Barcode = barcodeInt;

                                Cube c;
                                if (Global.app.par["StandaloneMode"] > 0)
                                {
                                    c = new Cube();
                                    c.Barcode = barcodeInt;
                                    c.SampleRef = "Test";

                                    status = Global.localDB.InsertOrUpdateCube(c);
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error inserting cube with barcode {0:D8} to database.", barcodeInt);
                                    }
                                }
                                else // production mode
                                {
                                    c = Global.localDB.GetCube(barcodeInt);
                                    if (c == null)
                                    {
                                        Global.logger.LogMessageEx("Error", 
                                            "Cube with barcode {0:D8} not found in database. Data will be logged in {1}.", 
                                            barcodeInt, Global.exceptionPath);
                                        c = new Cube();
                                        status = true;
                                        isBadCube[idx] = true;
                                    }
                                }

                                if (status)
                                {
                                    c.MeasuredDimX1 = Global.app.cubeAtTester[idx].MeasuredDimX1;
                                    c.MeasuredDimX2 = Global.app.cubeAtTester[idx].MeasuredDimX2;
                                    c.MeasuredDimX3 = Global.app.cubeAtTester[idx].MeasuredDimX3;
                                    c.MeasuredDimX4 = Global.app.cubeAtTester[idx].MeasuredDimX4;
                                    c.MeasuredDimX5 = Global.app.cubeAtTester[idx].MeasuredDimX5;
                                    c.MeasuredDimX6 = Global.app.cubeAtTester[idx].MeasuredDimX6;

                                    c.MeasuredDimY1 = Global.app.cubeAtTester[idx].MeasuredDimY1;
                                    c.MeasuredDimY2 = Global.app.cubeAtTester[idx].MeasuredDimY2;
                                    c.MeasuredDimY3 = Global.app.cubeAtTester[idx].MeasuredDimY3;
                                    c.MeasuredDimY4 = Global.app.cubeAtTester[idx].MeasuredDimY4;
                                    c.MeasuredDimY5 = Global.app.cubeAtTester[idx].MeasuredDimY5;
                                    c.MeasuredDimY6 = Global.app.cubeAtTester[idx].MeasuredDimY6;

                                    c.AvgDimension = Global.app.cubeAtTester[idx].AvgDimension;
                                    c.MeasuredWeight = Global.app.cubeAtTester[idx].MeasuredWeight;
                                    c.MeasuredDensity = Global.app.cubeAtTester[idx].MeasuredDensity;

                                    if (isBadCube[idx])
                                    {
                                        //LogBadCubeData(c);
                                    }
                                    else
                                    {
                                        status = Global.localDB.UpdateCube(c);
                                        if (!status)
                                        {
                                            Global.logger.LogMessageEx("Error", "Error updating cube with barcode {0:D8} to database.", barcodeInt);
                                        }
                                    }
                                }
                            }

                        }

                        if (status)
                        {
                            d1NextStep = 100;
                        }
                        else
                        {
                            d1NextStep = 4000;
                        }
                    }
                    break;

                case 100:
                    cubeData123Read = true;
                    if (!cubeData123Avail)
                    {
                        cubeData123Read = false;
                        d1NextStep = 0;
                    }
                    break;

                case 4000: // error, wait for alarm reset
                    if (Global.alarmActive.Length == 0)
                    {
                        d1NextStep = 0;
                    }
                    break;
                    
            }
        }

        private void LogBadCubeData(Cube cube)
        {
            try
            {

                if (!Directory.Exists(Global.exceptionPath)) Directory.CreateDirectory(Global.exceptionPath);

                string fn = string.Format("cube-{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                string path = Path.Combine(Global.exceptionPath, fn);
                using (StreamWriter wr = new StreamWriter(path, true))
                {
                    wr.WriteLine($"{cube.Barcode},{cube.MeasuredWeight},{cube.MeasuredStrength}");
                }
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception writing cube exception data.");
            }
        }

        private void HandleCubeData456()
        {
            bool status;

            d2Step = d2NextStep;

            switch (d2Step)
            {
                case 0:
                    if (cubeData456Avail)
                    {
                        Global.UIContext.Post(Global.mainForm.ClearAlarmPrefix, "PLC_");

                        cubeData456Read = false;

                        status = true;
                        if (dataTesterNum456 == 0 || dataTesterNum456 > Global.testerMax)
                        {
                            Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_BadDimTester");
                            Global.logger.LogMessageEx("Error", "Bad Dimension Tester Number From PLC");
                            status = false;
                        }

                        if (status)
                        {
                            uint idx = dataTesterNum456 - 1;
                            isBadCube[idx] = false;

                            Global.app.cubeAtTester[idx].MeasuredDimX1 = dimX456[0] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX2 = dimX456[1] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX3 = dimX456[2] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX4 = dimX456[3] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX5 = dimX456[4] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimX6 = dimX456[5] * 0.01;

                            Global.app.cubeAtTester[idx].MeasuredDimY1 = dimY456[0] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY2 = dimY456[1] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY3 = dimY456[2] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY4 = dimY456[3] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY5 = dimY456[4] * 0.01;
                            Global.app.cubeAtTester[idx].MeasuredDimY6 = dimY456[5] * 0.01;

                            Global.app.cubeAtTester[idx].AvgDimension =
                                (dimX456[0] + dimX456[1] + dimX456[2] + dimX456[3] + dimX456[4] + dimX456[5] +
                                 dimY456[0] + dimY456[1] + dimY456[2] + dimY456[3] + dimY456[4] + dimY456[5]) * 0.01 / 12.0;
                            Global.app.cubeAtTester[idx].MeasuredWeight = weight456 * 0.001;
                            Global.app.cubeAtTester[idx].MeasuredDensity = Global.app.cubeAtTester[idx].AvgDimension > 0 ?
                                Global.app.cubeAtTester[idx].MeasuredWeight / Util.Sqr(Global.app.cubeAtTester[idx].AvgDimension / 1000.0) : 0;

                            StringBuilder sb = new StringBuilder("          ");
                            sb[0] = (char)(barcodeWord456[0] >> 8);
                            sb[1] = (char)(barcodeWord456[0] & 0xFF);
                            sb[2] = (char)(barcodeWord456[1] >> 8);
                            sb[3] = (char)(barcodeWord456[1] & 0xFF);
                            sb[4] = (char)(barcodeWord456[2] >> 8);
                            sb[5] = (char)(barcodeWord456[2] & 0xFF);
                            sb[6] = (char)(barcodeWord456[3] >> 8);
                            sb[7] = (char)(barcodeWord456[3] & 0xFF);
                            sb[8] = (char)(barcodeWord456[4] >> 8);
                            sb[9] = (char)(barcodeWord456[4] & 0xFF);

                            barcodeStr = sb.ToString(); // for now
                            status = int.TryParse(barcodeStr, out barcodeInt);
                            if (!status)
                            {
                                Global.UIContext.Post(Global.mainForm.SetAlarm, "PLC_BadBarcode");
                                barcodeOK = false;
                                status = false;

                                Global.logger.LogMessageEx("Error", "Bad barcode from PLC at Tester {0}.", dataTesterNum456);
                            }

                            if (status)
                            {
                                Global.app.cubeAtTester[idx].Barcode = barcodeInt;

                                Cube c;
                                if (Global.app.par["StandaloneMode"] > 0)
                                {
                                    c = new Cube();
                                    c.Barcode = barcodeInt;
                                    c.SampleRef = "Test";

                                    status = Global.localDB.InsertOrUpdateCube(c);
                                    if (!status)
                                    {
                                        Global.logger.LogMessageEx("Error", "Error inserting cube with barcode {0:D8} to database.", barcodeInt);
                                    }
                                }
                                else // production mode
                                {
                                    c = Global.localDB.GetCube(barcodeInt);
                                    if (c == null)
                                    {
                                        Global.logger.LogMessageEx("Error",
                                                "Cube with barcode {0:D8} not found in database. Data will be logged in {1}.",
                                                barcodeInt, Global.exceptionPath);
                                        c = new Cube();
                                        isBadCube[idx] = true;
                                    }
                                    status = true;
                                }

                                if (status)
                                {
                                    c.MeasuredDimX1 = Global.app.cubeAtTester[idx].MeasuredDimX1;
                                    c.MeasuredDimX2 = Global.app.cubeAtTester[idx].MeasuredDimX2;
                                    c.MeasuredDimX3 = Global.app.cubeAtTester[idx].MeasuredDimX3;
                                    c.MeasuredDimX4 = Global.app.cubeAtTester[idx].MeasuredDimX4;
                                    c.MeasuredDimX5 = Global.app.cubeAtTester[idx].MeasuredDimX5;
                                    c.MeasuredDimX6 = Global.app.cubeAtTester[idx].MeasuredDimX6;

                                    c.MeasuredDimY1 = Global.app.cubeAtTester[idx].MeasuredDimY1;
                                    c.MeasuredDimY2 = Global.app.cubeAtTester[idx].MeasuredDimY2;
                                    c.MeasuredDimY3 = Global.app.cubeAtTester[idx].MeasuredDimY3;
                                    c.MeasuredDimY4 = Global.app.cubeAtTester[idx].MeasuredDimY4;
                                    c.MeasuredDimY5 = Global.app.cubeAtTester[idx].MeasuredDimY5;
                                    c.MeasuredDimY6 = Global.app.cubeAtTester[idx].MeasuredDimY6;

                                    c.AvgDimension = Global.app.cubeAtTester[idx].AvgDimension;
                                    c.MeasuredWeight = Global.app.cubeAtTester[idx].MeasuredWeight;
                                    c.MeasuredDensity = Global.app.cubeAtTester[idx].MeasuredDensity;

                                    if (isBadCube[idx])
                                    {
                                        //LogBadCubeData(c);
                                    }
                                    else
                                    {
                                        status = Global.localDB.UpdateCube(c);
                                        if (!status)
                                        {
                                            Global.logger.LogMessageEx("Error", "Error updating cube with barcode {0:D8} to database.", barcodeInt);
                                        }
                                    }
                                }
                            }

                        }

                        if (status)
                        {
                            d2NextStep = 100;
                        }
                        else
                        {
                            d2NextStep = 4000;
                        }
                    }
                    break;

                case 100:
                    cubeData456Read = true;
                    if (!cubeData456Avail)
                    {
                        cubeData456Read = false;
                        d2NextStep = 0;
                    }
                    break;

                case 4000: // error, wait for alarm reset
                    if (Global.alarmActive.Length == 0)
                    {
                        d2NextStep = 0;
                    }
                    break;

            }
        }
    }
}

