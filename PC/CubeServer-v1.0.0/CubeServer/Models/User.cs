using CubeServer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CubeServer.Models
{    
    public class User
    {
        [Required]
        [StringLength(50, ErrorMessage = "User ID too long (50 character limit).")]
        [MinLength(1, ErrorMessage = "Cannot have empty userId")]
        public string userId { get; set; } = "";

        [Required]
        public string userName { get; set; }
        [Required]
        public string windowsId { get; set; }
        [Required]
        [Range(1,100, ErrorMessage = "Select User Privilege")]
        public int privilege { get; set; }

        //[StringLength(50, ErrorMessage = "Password too long (50 character limit).")]
        public string password { get; set; }

        //[StringLength(50, ErrorMessage = "Password too long (50 character limit).")]
        //[Compare("password", ErrorMessage = "Password and Confirm Password must match")]
        public string password2 { get; set; }

        public string phone = "";
        public string mobile = "";

        [EmailAddress]
        public string email = "";
        public string secret = "";

        [Required]
        public bool enabled { get; set; }
        public DateTime? lastLogin { get; set; }
        public DateTime lastUpdate { get; set; }

        public string userPrivilegeStr {
            get
            { return UserPrivilegeStr(); }
            set
            { SetUserPrivilegeStr(value); }
        }

        public User()
        {

        }

        public User(User other)
        {
            userId = other.userId;
            userName = other.userName;
            privilege = other.privilege;
            password = other.password;
            enabled = other.enabled;
            lastUpdate = other.lastUpdate;
        }

        public string UserPrivilegeStr()
        {
            foreach (UserPrivilege p in Enum.GetValues(typeof(UserPrivilege)))
            {
                if ((int)p == privilege) return Util.SplitCamelCase(p.ToString());
            }

            return "Select User Privilege";
            //return privilege.ToString();
        }

        public void SetUserPrivilegeStr(string value)
        {
            string s = value.Replace(" ", "");

            foreach (UserPrivilege p in Enum.GetValues(typeof(UserPrivilege)))
            {
                if (p.ToString() == s)
                {
                    privilege = (int)p;
                }
            }
        }

        public const int passwordLengthMin = 8;

        public static bool CheckPasswordComplexity(string pw)
        {
            Regex regex;

            // length
            if (pw.Length < passwordLengthMin) return false;

            // contains digit
            regex = new Regex(@"[0-9]+");
            if (!regex.IsMatch(pw)) return false;

            // contains lowercase
            regex = new Regex(@"[a-z]+");
            if (!regex.IsMatch(pw)) return false;

            // contains uppercase
            regex = new Regex(@"[A-Z]+");
            if (!regex.IsMatch(pw)) return false;

            // contains symbols
            //regex = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            //if (!regex.IsMatch(pw)) return false;

            return true;
        }
    }

    public enum UserPrivilege
    {
        //Guest = 0, // not used
        //Sales = 10,
        DataEntry = 20,
        //Reporting = 30,
        CustomerService = 40,
        WebApi = 50, 
        Administrator = 100
    }
}
