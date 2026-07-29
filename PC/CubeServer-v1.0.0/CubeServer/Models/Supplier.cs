using System.ComponentModel.DataAnnotations;

namespace CubeServer.Models
{
    public class Supplier
    {
        [Required]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string CreateUser { get; set; } = "";
        public DateTime? CreateDate { get; set; }
        public string LastUpdateUser { get; set; } = "";
        public DateTime? LastUpdate { get; set; }
    }
}
