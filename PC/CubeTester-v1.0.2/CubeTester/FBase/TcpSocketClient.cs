using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace FBase
{
    public class TcpSocketClient
    {
        string ipAddr;
        int port;

        int bufsize = 10 * 1024;
        int receiveTimeout = 1000;

        protected byte[] buffer;

        int bufidx = 0;
        public Socket sock = null;

        object lckSend = 1; // lock for sending
        Thread thread;
        bool debug = false;

        protected FLogger logger;

        public char eomChar = '\n'; // end of message

        public TcpSocketClient(FLogger logger, char eomChar = '\n', int bufsize = 0)
        {
            this.logger = logger;
            this.ipAddr = "";
            this.port = 0;

            if (bufsize > 0)
            {
                this.bufsize = bufsize;
            }
            buffer = new byte[this.bufsize];
            this.eomChar = eomChar;
        }

        public bool ConnectToServer(string ipAddr, int port)
        {
            this.ipAddr = ipAddr;
            this.port = port;

            if (sock != null && sock.Connected) return false; // already connected

            IPAddress ipAddress;
            if (!IPAddress.TryParse(ipAddr, out ipAddress)) 
            { 
                return false; 
            }

            IPEndPoint remoteEP = new IPEndPoint(ipAddress,port);

            // Create a TCP/IP  socket.
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            // Async connect for timeout
            IAsyncResult result = sock.BeginConnect(remoteEP, null, null);
            bool connected = result.AsyncWaitHandle.WaitOne( 5000, true );

            if (!connected)
            {
                sock.Close();
                sock = null;
                return false;
            }
     
            sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeout);

            bufidx = 0;

            return true;
        }

        public void DisconnectFromServer()
        {
            if (sock != null)
            {
                sock.Close();
                //sock.Shutdown(SocketShutdown.Both);
                sock = null;
            }
        }

        public bool IsConnected()
        {
            return (sock != null && sock.Connected);
        }

        public bool ReconnectToServer()
        {
            DisconnectFromServer();

            bool status = ConnectToServer(ipAddr, port);
            return status;
        }

        public bool Send(string format, params object[] args)
        {
            lock (lckSend)
            {
                string msg = string.Format(format, args);
                if (debug) logger.LogMessageEx("Comm", "SEND: {0}", msg);

                int byteCount = 0;

                byte[] byteMsg = Encoding.ASCII.GetBytes(msg);
                int len = byteMsg.Length;

                try
                {
                    long t0 = Util.stopwatch.ElapsedMilliseconds;
                    do
                    {
                        byteCount += sock.Send(byteMsg, byteCount, len - byteCount, SocketFlags.None);
                        if (byteCount >= len) return true;
                    }
                    while (Util.stopwatch.ElapsedMilliseconds - t0 < 1000);

                    return false;

                }
                catch(Exception e)
                {
                    return false;
                }
            }
        }

        protected int FindEndOfMsg(byte[] buf)
        {
            // check if end of message received
            int endOfMsg = -1;

            for (int i = 0; i < buf.Length; i++)
            {
                if (buf[i] == eomChar)
                {
                    endOfMsg = i;
                    break;
                }
            }

            return endOfMsg;

        }

        public int ReceiveBytes(byte[] buf, bool blocking=true)
        {
            int byteCount = 0;

            if (blocking || sock.Available > 0)
            {
                byteCount = sock.Receive(buf, SocketFlags.None);
            }

            return byteCount;
        }

        public void SetReceiveTimeout(int timeoutMs)
        {
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeoutMs);
        }

        public void ResetReceiveTimeout()
        {
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeout);
        }

        public string Receive(bool blocking=true)
        {
            try
            {
                if (sock == null)
                {
                    return null;
                }

                int endOfMsg = -1;
                string s;

                if (blocking || (sock.Available > 0))
                {
                    int byteCount = sock.Receive(buffer, bufidx, bufsize - bufidx, SocketFlags.None);

                    if (byteCount > 0)
                    {
                        bufidx += byteCount;
                    }
                }

                if (bufidx > 0)
                {
                    endOfMsg = FindEndOfMsg(buffer);

                    if (endOfMsg >= 0 && endOfMsg <= bufidx)
                    {
                        s = Encoding.ASCII.GetString(buffer, 0, endOfMsg + 1);
                        Buffer.BlockCopy(buffer, endOfMsg + 1, buffer, 0, bufidx - endOfMsg - 1);
                        bufidx -= endOfMsg + 1;
                        if (bufidx < 0) bufidx = 0;

                        s = s.TrimEnd();
                        if (debug) logger.LogMessageEx("Comm", "RECV: {0}", s);

                        return s;
                    }
                }

                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool Ping()
        {
            if (!Send("Ping" + eomChar))
            {
                return false;
            }

            string resp = Receive();
            return (string.Equals(resp, "Ping", StringComparison.OrdinalIgnoreCase));
        }

        public bool StartService(string name, object par=null)
        {
            if (thread != null) return false;

            thread = new Thread(new ParameterizedThreadStart(Run));
            thread.Name = name;
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Normal;
            thread.Start(par);

            return true;
        }

        public virtual void StopService(int waitTime = 10000)
        {            
            if (thread == null) return;

            if (thread.Join(waitTime))
            {
                thread = null;
            }
            else
            {
                thread.Abort();
                if (thread.Join(5000))
                {
                    thread = null;
                }
                else
                {
                    logger.LogMessage("Error", string.Format("{0} shutdown timeout.", thread.Name));
                }
            }
        }

        public virtual bool ServiceRunning()
        {
            return thread != null && thread.IsAlive;
        }

        // to be run in a thread
        public virtual void Run(object par)
        {
            throw new Exception("TcpSocketClient Run() not derived.");
        }

        // to be run once externally
        public virtual void RunOnce(object par)
        {
            throw new Exception("TcpSocketClient RunOnce() not derived.");
        }
    }
}
