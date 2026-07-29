using System.ComponentModel.DataAnnotations;

namespace CubeServer.Models
{
    public class TestSpec
    {
        [Required]
        public string SpecId { get; set; } = "S206";
        public string Description { get; set; }
        public string TestStandard { get; set;}
        [Required]
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdate { get; set; }
    }

}
