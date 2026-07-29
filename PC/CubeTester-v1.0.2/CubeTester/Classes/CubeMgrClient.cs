using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FBase;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Win32.SafeHandles;
using System.Security.Principal;
using System.Net;

namespace CubeTester.Classes
{
    public class CubeMgrClient
    {
        Thread thread;
        bool runThread;

        private readonly HttpClient client;
        Token token;
        const int retryMax = 3;
        List<Cube> cubesForUpload = new List<Cube>();

        public static SemaphoreSlim smLock = new SemaphoreSlim(1, 1);
        public DateTime? lastDownload = null;
        HttpClientHandler handler;
        const bool anonymousAuth = false;

        public CubeMgrClient()
        {
            if (!anonymousAuth)
            {
                string userid = Global.app.settings["CubeMgrWinUserId"];
                string password = Util.AesDescrypt(Global.aesKey, Global.app.settings["CubeMgrWinPassword"]);

                handler = new HttpClientHandler()
                {
                    //Credentials = new NetworkCredential("CubeMgr\\fui", "=&sNhi]5c93}xD8")
                    Credentials = new NetworkCredential(userid, password)
                };
                client = new HttpClient(handler);
            }
            else
            {
                client = new HttpClient();
            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Cube Manager");
            client.BaseAddress = new Uri(Global.app.settings["CubeMgrUri"]);
        }

        public async Task<bool> GetToken()
        {
            try
            {
                // ignore validation errors
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    (senderX, certificate, chain, sslPolicyErrors) => { return true; };

                string uri = string.Format("api/token?userId={0}&password={1}",
                    Global.app.settings["CubeMgrUserId"], Global.app.settings["CubeMgrPassword"]);
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<Token>(json);
                    return (token != null);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<SyncPacket> GetData(DateTime? lastDownload)
        {
            try
            {
                HttpResponseMessage response;
                bool responseOk = false;
                int retry = 0;
                DateTime sendTime = DateTime.Now;

                do
                {
                    if (anonymousAuth)
                    {
                        // check if token has expired
                        if (token == null || DateTime.Compare((DateTime)token.expiry, DateTime.Now) < 0)
                        {
                            bool status = await GetToken();
                            if (!status) return null;
                        }

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.token);
                    }

                    string uri;
                    if (lastDownload == DateTime.MinValue || lastDownload == null)
                    {
                        uri = string.Format("api/data/getdata");
                    }
                    else
                    {
                        uri = string.Format("api/data/getdata?lastUpdate={0}", lastDownload.ToString(Global.SqlDateFormat));
                    }

                    response = await client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        responseOk = true;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        token = null;
                    }
                }
                while (!responseOk && (++retry < retryMax));

                if (responseOk)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    SyncPacket packet = JsonConvert.DeserializeObject<SyncPacket>(json);
                    if (packet != null)
                    {
                        lastDownload = packet.timestamp;
                    }
                    return packet;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Cube> UpdateCubeData(Cube cube)
        {
            try
            {
                HttpResponseMessage response;
                bool responseOk = false;
                int retry = 0;

                do
                {
                    if (anonymousAuth)
                    {
                        // check if token has expired
                        if (token == null || DateTime.Compare((DateTime)token.expiry, DateTime.Now) < 0)
                        {
                            bool status = await GetToken();
                            if (!status) return null;
                        }

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.token);
                    }

                    string uri = string.Format("api/data/updatecube/{0}", cube.Barcode);
                    response = await client.PutAsJsonAsync<Cube>(uri, cube);

                    if (response.IsSuccessStatusCode)
                    {
                        responseOk = true;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        token = null;
                    }
                }
                while (!responseOk && (++retry < retryMax));

                if (responseOk)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Cube c = JsonConvert.DeserializeObject<Cube>(json);
                    if (c != null)
                    {
                        return c;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
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

            if (thread.Join(10000))
            {
                //thread.Abort();
                thread = null;
            }
            else
            {
                Global.logger.LogMessageEx("Error", "Error shutting down Cube Manager server poll thread.");
            }
        }

        private async void PollTask(object par)
        {
            bool status = false;
            int errcnt = 0;
            DateTime lastReload = DateTime.Now;

            runThread = true;

            Global.logger.LogMessageEx("Info", "Cube Manager Client Thread started.");
            long t0;

            while (runThread)
            {
                smLock.Wait();

                Task<SyncPacket> tskPacket = GetData(lastDownload);
                await tskPacket;

                t0 = Global.stopwatch.ElapsedMilliseconds;

                do
                {
                    if (tskPacket.IsCompleted)
                    {
                        if (tskPacket.Result != null)
                        {
                            if (lastDownload == null)
                            { // first time, clear db
                                Global.localDB.ClearCubeSetTable();
                                Global.localDB.ClearBatchTable();
                                Global.localDB.ClearCubeTable();
                            }

                            status = Global.localDB.InsertOrUpdateCubeSetList(tskPacket.Result.cubeSets);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error updating cube set data to local database.");
                                break;
                            }

                            status = Global.localDB.InsertOrUpdateBatchList(tskPacket.Result.batches);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error updating batch data to local database.");
                                break;
                            }

                            status = Global.localDB.InsertOrUpdateCubeList(tskPacket.Result.cubes);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Error updating cube data to local database.");
                                break;
                            }

                            if (tskPacket.Result.deletedCubeIds != null)
                            {
                                foreach (int bc in tskPacket.Result.deletedCubeIds)
                                {
                                    Global.localDB.DeleteCube(bc);
                                }
                            }

                            if (tskPacket.Result.batches.Count > 0 || tskPacket.Result.cubes.Count > 0)
                            {
                                Global.app.newCubeData = true;
                            }

                            lastDownload = tskPacket.Result.timestamp.AddSeconds(-5); // to ensure overlap

                            if (DateTime.Today > lastReload.Date)
                            {
                                lastDownload = null;
                                lastReload = DateTime.Now;
                            }
                        }
                        else
                        {
                            if (!Global.app.serverStatus)
                            {
                                Global.logger.LogMessageEx("Error", "Result from Cube Manager is null. Cube Manager may be down.");
                            }
                            status = false;
                        }

                        Global.app.serverStatus = status;
                        break;
                    }
                    else if (tskPacket.IsCanceled || tskPacket.IsFaulted)
                    {
                        Global.logger.LogMessageEx("Error", "Error synching data from Cube Server. Sync canceled or faulted.");
                        status = false;
                        break;
                    }
                    else if (Global.stopwatch.ElapsedMilliseconds - t0 > (long)Global.app.par["WebApiWaitTime"])
                    {
                        Global.logger.LogMessageEx("Error", "Timeout synching data from Cube Server.");
                        status = false;
                        break;
                    }

                    Thread.Sleep(10);
                }
                while (true);

                if (status)
                {
                    // get cubes for update
                    status = Global.localDB.GetCubesForUpload(cubesForUpload);
                    if (status)
                    {
                        foreach (Cube c in cubesForUpload)
                        {
                            Cube retCube = await UploadCube(c);
                        }
                    }
                }

                smLock.Release();


                Thread.Sleep((int)Global.app.par["CubeMgrSyncInterval"]);
            }

            Global.logger.LogMessageEx("Info", "Cube Manager Client Thread exited.");
        }

        public async Task<Cube> UploadCube(Cube c)
        {
            bool status = false;
            c.Uploaded = true;
            c.UploadTime = DateTime.Now;
            Cube retCube = null;

            Task<Cube> tskUpload = UpdateCubeData(c);
            await tskUpload;

            long t0 = Global.stopwatch.ElapsedMilliseconds;

            do
            {
                if (tskUpload.IsCompleted)
                {
                    if (tskUpload.Result != null)
                    {
                        retCube = tskUpload.Result;
                        if (retCube != null && retCube.Barcode == c.Barcode)
                        {
                            // update local as well
                            status = Global.localDB.UpdateCube(retCube);
                            if (!status)
                            {
                                Global.logger.LogMessageEx("Error", "Update of cube {0} to local database unsuccessful.", c.Barcode);
                            }
                        }
                    }
                    else
                    {
                        Global.logger.LogMessageEx("Error", "Upload of cube {0} to Cube Manager unsuccessful.", c.Barcode);
                    }

                    Global.app. serverStatus = status;
                    break;
                }
                else if (tskUpload.IsCanceled || tskUpload.IsFaulted)
                {
                    Global.logger.LogMessageEx("Error", "Error uploading cube data to Cube Server. Upload canceled or faulted.");
                    status = false;
                    break;
                }
                else if (Global.stopwatch.ElapsedMilliseconds - t0 > (long)Global.app.par["WebApiWaitTime"])
                {
                    Global.logger.LogMessageEx("Error", "Timeout uploading data to Cube Server.");
                    status = false;
                    break;
                }

                Thread.Sleep(10);
            }
            while (true);

            return retCube;
        }
    }

}
