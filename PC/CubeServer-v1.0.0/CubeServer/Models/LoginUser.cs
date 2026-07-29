using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CubeServer.Models
{
    public class LoginUser
    {
        [Required]
        [StringLength(50, ErrorMessage = "User ID too long (50 character limit).")]
        [MinLength(1, ErrorMessage = "Cannot have empty userId")]
        public string userId { get; set; }        

        [Required]
        [StringLength(50, ErrorMessage = "Password too long (50 character limit).")]
        public string password { get; set; }

        [Required]
        public string otp { get; set; }

    }
}
