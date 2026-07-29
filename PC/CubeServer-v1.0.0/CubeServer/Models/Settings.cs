using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CubeServer.Models
{
    public class Settings
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Interval must be greater than 0")]
        public double WebPageRefreshInterval { get; set; } // seconds
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Validity must be greater than 0")]
        public double WebApiTokenValidity { get; set; } // seconds

    }
}
