using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeTester.Classes
{
    public class TesterRecvConnect
    {
        public Dictionary<string, string> Connect;
    }

    public class TesterSendConnect
    {
        public string Command = "Connect";
        public Dictionary<string, string> Return = new Dictionary<string, string>()
        {
            { "Code", "0" },
            { "Content", ""}
        };
    }


    public class TesterDevMove
    {
        public Dictionary<string, object> DevMove;
    }
}
