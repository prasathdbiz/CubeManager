using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace FBase
{
    public class TcpServer
    {
        static int bufsize = 1024;

        protected string name;
        protected int port;
        protected bool listen = false;
        protected ArrayList readlist;
        protected ArrayList connectlist;
        protected ArrayList errlist;
        protected Socket server;
        protected byte[] buffer;
        protected Thread thread;

        protected FLogger logger;

        public TcpServer(FLogger logger, string name, int port)
        {
            this.logger = logger;
            this.name = name;
            this.port = port;

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(ep);
            server.Listen(1000);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
            server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            readlist = new ArrayList();
            connectlist = new ArrayList();
            errlist = new ArrayList();
            buffer = new byte[bufsize];

            listen = true;

            logger.LogMessage("Info", string.Format("[{0}] listening at port {1}.", name, port));
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

            listen = false;

            //thread.Abort();
            if (thread.Join(10000))
            {
                thread = null;
            }
            else
            {
                logger.LogMessage("Error", string.Format("[{0}] comm server shutdown timeout.", name));
            }
        }

        public bool ServerRunning()
        {
            if (thread == null) return false;

            return thread.IsAlive;
        }

        protected virtual void Run(object par)
        {
            throw new Exception("Derived Run() function not defined.");
        }

        public bool ClientConnected()
        {
            return connectlist.Count > 0;
        }

    }

}