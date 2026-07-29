using System.ComponentModel.DataAnnotations;

namespace CubeServer.Models
{
    public class Quotation
    {
        [Required]
        public int QuoNum { get; set; }
        [Required]
        public string BillToParty { get; set; }
        [Required]
        public string SoldToParty { get; set; }
        [Required]
        public DateTime? QuoDate { get; set; }
        [Required]
        public long CustomerNum { get; set; }
        [Required]
        public string Currency { get; set; }
        public string AttnName { get; set; }
        public string AttnTel { get; set; }
        public string AttnMobile { get; set; }
        public string AttnFax { get; set; }
        [EmailAddress]
        public string AttnEmail { get; set; }
        [Required]
        public string QuoSubject { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string PaymentTerms { get; set; }
        public bool Confirmed { get; set; }
        public string Filename { get; set; }
        public string UploadUser { get; set; }
        public DateTime? Uploaded { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
