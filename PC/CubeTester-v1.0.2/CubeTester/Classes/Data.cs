using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CubeTester.Classes
{

    public enum SystemStatus
    {
        Idle,
        Ready,
        Processing,
        Error,
    }

    public enum TesterCmd
    {
        None,
        SetPar,
        Start,
        Stop
    }

    public enum RunMode
    {
        None,
        Production,
        Setup,
        Test,
        DryRun,
        WetRun
    }

    public class Var<T>
    {
        public string name { get; set; }
        public T prev
        {
            get;
            protected set;
        }

        private T _val;
        public T val
        {
            get { return _val; }
            set { prev = _val; _val = value; }
        }

        public bool Changed()
        {
            return !prev.Equals(val);
        }

        public Var(string name, T val)
        {
            this.name = name;
            this.val = val;
        }
    }

    public class ClientPar
    {
        public TcpClient client;
        public int testerNum;
    }

    public class TesterPar
    {
        public long testID = 1;
        public int age = 7;
        public int grade = 30;
        public int dimension = 150;
    }

}
