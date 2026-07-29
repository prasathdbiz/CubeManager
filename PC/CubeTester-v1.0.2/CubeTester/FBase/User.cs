using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class User
    {
        public string id;
        public string name;
        public UserPrivilege privilege;
        public bool enabled;
        public string password;

        public static UserPrivilege IntToUserPrivilege(int priv)
        {
            foreach(UserPrivilege p in Enum.GetValues(typeof(UserPrivilege)))
            {
                if ((int)p == priv) return p;
            }

            return UserPrivilege.Operator;
        }

        public static UserPrivilege StringToUserPrivilege(string s)
        {
            foreach (UserPrivilege p in Enum.GetValues(typeof(UserPrivilege)))
            {
                if (p.ToString() == s) return p;
            }

            return UserPrivilege.Operator;
        }
    }

    public enum UserPrivilege
    {
        Operator = 10,
        Supervisor = 20,
        Engineer = 30,
        Administrator = 40
    }

}
