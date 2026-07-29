namespace CubeServer.Models
{
    public class BarcodeAllocation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int Qty { get; set; }
        public int BarcodeStart { get; set; }
        public int BarcodeEnd { get; set;}
        public string CreateUser { get; set; } = "";
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; } = "";
        public DateTime? LastUpdate { get; set; }
    }
}
