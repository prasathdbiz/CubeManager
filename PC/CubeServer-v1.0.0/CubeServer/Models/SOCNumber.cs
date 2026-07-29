namespace CubeServer.Models
{
    public class SCONumber
    {
        public int ScoNumber { get; set; }
        public int ProjectId { get; set; }
        public string LastUpdateUser { get; set; } = "";
        public DateTime? LastUpdate { get; set; }
    }
}
