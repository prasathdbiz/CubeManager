using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeServer.Data
{
    public class Session
    {
        public string sessionId { get; set; }
        public string userId { get; set; }
        public string sessionData { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}
