using Plotly.Blazor.LayoutLib;

namespace CubeServer.Models
{
    public class SyncPacket
    {
        public DateTime timestamp { get; set; }
        public List<CubeSet> cubeSets { get; set; }
        public List<Batch> batches { get; set; }
        public List<Cube> cubes { get; set; }
        public List<long> deletedCubeIds { get; set; }
        public SyncPacket()
        {
            timestamp = DateTime.Now;
            cubeSets = new List<CubeSet>();
            batches = new List<Batch>();
            cubes = new List<Cube>();
        }
    }
}
