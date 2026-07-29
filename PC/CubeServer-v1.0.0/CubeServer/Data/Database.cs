using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CubeServer.Models;
using CubeServer.Pages;
using static MudBlazor.CategoryTypes;
using System.Xml.Linq;
using Plotly.Blazor.Traces;
using Org.BouncyCastle.Bcpg;
using System.Linq.Expressions;
using Scriban.Parsing;
using Newtonsoft.Json.Linq;
using Plotly.Blazor.Traces.WaterfallLib;
using CubeServer.Authentication;
using Plotly.Blazor.Traces.StreamTubeLib;

namespace CubeServer.Data
{
    public enum CubeQueryOption
    {
        All,
        Untested,
        PassOnly,
        FailOnly,
        TestedOnDate,
    }

    public partial class Database
    {
        string connStr;

        public Database()
        {
            // mysql
            //connStr = "Server=127.0.0.1;Port=3306;database=cubemgr;user id=cubemgr;password=TuvSud60";
            // sql server
            Global.logger.LogMessageEx("Info", $"Machine name is {System.Environment.MachineName}");
            if (System.Environment.MachineName == "FYTROJET4" || System.Environment.MachineName == "HAL6")
            {
                connStr = "Server=localhost;Database=CubeMgr;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            //else if (System.Environment.MachineName == "HAL6")
            //{
            //    connStr = "Server=localhost;TrustServerCertificate=True;Database=CubeMgr;User Id=cubemgr;Password=TuvSud60;";

            //}
            else if (System.Environment.MachineName == "CUBEMGR")
            {
                //connStr = "Server=localhost\\CUBE-TESTER;Database=CubeMgr;Trusted_Connection=True;TrustServerCertificate=True;";
                connStr = Global.config.GetSection("Data:LocalConnection:ConnectionString").Value;
            }
            else
                    {
                //connStr = "Server=localhost\\MSSQLSERVER01;TrustServerCertificate=True;Database=CubeMgr;User Id=cubemgr;Password=TuvSud60;";
                //connStr = "Server=localhost\\MSSQLSERVER01;Database=CubeMgr;Trusted_Connection=True;TrustServerCertificate=True;User ID=cubemgr;Password=TuvSud60;";
                //connStr = "Server=ssgsinx08552.sg001.itgr.net:14330;Database=CubeMgr;TrustServerCertificate=True;User ID=cubemgr;Password=0HAiJKYSVdh2jEa@;";
                connStr = Global.config.GetSection("Data:DefaultConnection:ConnectionString").Value;
            }
        }

        //public Task Execute<T>(string sql, T parameters)
        //{
        //    return conn.ExecuteAsync(sql, parameters);
        //}

        //public async Task<List<T>> Query<T, U>(string sql, U parameters)
        //{
        //    var rows = await conn.QueryAsync<T>(sql, parameters);
        //    return rows.ToList();
        //}

        //*************************************************************
        // Settings
        //*************************************************************
        public string ReadSettings(string name)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string value = null;
                string sql = "SELECT Value FROM Settings WHERE Name=@Name;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", name);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            value = reader.SafeGetString(0);
                        }
                    }
                }

                return value;
            }
        }

        public bool SaveSettings(string name, string value)
        {
            string sql;
            bool status;
            int cnt = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Name", name);
                        cmd.Parameters.AddWithValue("Value", value);

                        cnt = cmd.ExecuteNonQuery();
                    }

                    if (cnt <= 0)
                    {   // do an insert
                        sql = "INSERT INTO Settings (Name, Value) VALUES (@Name,@Value)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Name", name);
                            cmd.Parameters.AddWithValue("Value", value);

                            cnt = cmd.ExecuteNonQuery();
                        }
                    }

                    status = cnt > 0;

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error saving settings for '{0}': {1}", name, ex.Message);
                return false;
            }

        }

        public bool ReadAllSettings(Dictionary<string, string> par)
        {
            string name;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Name, Value FROM Settings;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name = reader.SafeGetString(0);
                            if (par.ContainsKey(name))
                            {
                                par[name] = reader.SafeGetString(1);
                            }
                        }
                    }
                }

                return true;
            }

        }

        public bool SaveAllSettings(Dictionary<string, string> par)
        {
            string sql;
            bool status = false;
            int cnt = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    foreach (var pair in par)
                    {
                        sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Name", pair.Key);
                            cmd.Parameters.AddWithValue("Value", pair.Value);

                            cnt = cmd.ExecuteNonQuery();
                        }

                        if (cnt <= 0)
                        {   // do an insert
                            sql = "INSERT INTO Settings (Name, Value) VALUES (@Name,@Value)";
                            using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("Name", pair.Key);
                                cmd.Parameters.AddWithValue("Value", pair.Value);

                                cnt = cmd.ExecuteNonQuery();
                            }
                        }

                        status = cnt > 0;
                        if (!status) break;
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error saving all settings to database: {0}", ex.Message);
                return false;
            }

        }

        public double ReadNumericSettings(string name)
        {
            double val = 0;
            string valStr;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT Value FROM Settings WHERE Name=@Name;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Name", name);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = reader.SafeGetString(0);
                            valStr = reader.SafeGetString(1);

                            double.TryParse(reader.SafeGetString(1), out val);
                        }
                    }
                }

                return val;
            }

        }

        public bool ReadAllNumericSettings(Dictionary<string, double> dblPar)
        {
            string name;
            double val;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT Name, Value FROM Settings;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name = reader.SafeGetString(0);
                            if (dblPar.ContainsKey(name))
                            {
                                if (!double.TryParse(reader.SafeGetString(1), out val)) return false;

                                dblPar[name] = val;
                            }
                        }
                    }
                }

                return true;
            }

        }

        public bool SaveNumericSettings(string name, double value)
        {
            string sql;
            int cnt;
            bool status;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Name", name);
                        cmd.Parameters.AddWithValue("Value", value);

                        cnt = cmd.ExecuteNonQuery();
                    }

                    if (cnt <= 0)
                    {
                        sql = "INSERT INTO Settings (Name, Value) VALUES (@Name, @Value);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Name", name);
                            cmd.Parameters.AddWithValue("Value", value);

                            cnt = cmd.ExecuteNonQuery();
                        }
                    }

                    status = cnt > 0;

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error saving numeric settings '{0}' to database: {1}", name, ex.Message);
                return false;
            }

        }

        public bool SaveAllNumericSettings(Dictionary<string, double> dblPar)
        {
            bool status;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    foreach (var par in dblPar)
                    {
                        status = SaveNumericSettings(par.Key, par.Value);
                        if (!status)
                        {
                            trans.Rollback();
                            return false;
                        }
                    }

                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error saving all numeric settings to database: {0}", ex.Message);
                return false;
            }

        }

        #region Users
        //*************************************************************
        // Users
        //*************************************************************
        public bool GetUsers(List<User> userList, int offset=0, int rows=0)
        {
            bool status = false;
            userList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT UserId, UserName, WindowsID, Privilege, Enabled, Password, " +
                             "Tel, Mobile, Email, Secret, LastLogin, LastUpdate " +
                             "FROM Users ORDER BY UserId;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User u = new User();

                            u.userId = reader.GetString(0);
                            u.userName = reader.SafeGetString(1);
                            u.windowsId = reader.SafeGetString(2);
                            u.privilege = reader.SafeGetInt(3);
                            u.enabled = reader.SafeGetByte(4) > 0;
                            u.password = reader.SafeGetString(5);
                            u.phone = reader.SafeGetString(6);
                            u.mobile = reader.SafeGetString(7);
                            u.email = reader.SafeGetString(8);
                            u.secret = reader.SafeGetString(9);
                            u.lastLogin = reader.SafeGetDateTime(10);
                            u.lastUpdate = reader.GetDateTime(11);

                            u.userPrivilegeStr = u.UserPrivilegeStr();

                            userList.Add(u);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public bool GetUsersForPage(List<User> userList, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            userList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                sql = string.Format("SELECT UserId, UserName, WindowsID, Privilege, Enabled, Password, " +
                                    "Tel, Mobile, Email, Secret, LastLogin, LastUpdate, COUNT(*) " +
                                    "FROM Users ORDER BY UserId {0};", sqlseg);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User u = new User();

                            u.userId = reader.GetString(0);
                            u.userName = reader.SafeGetString(1);
                            u.windowsId = reader.SafeGetString(2);
                            u.privilege = reader.SafeGetInt(3);
                            u.enabled = reader.SafeGetByte(4) > 0;
                            u.password = reader.SafeGetString(5);
                            u.phone = reader.SafeGetString(6);
                            u.mobile = reader.SafeGetString(7);
                            u.email = reader.SafeGetString(8);
                            u.secret = reader.SafeGetString(9);
                            u.lastLogin = reader.SafeGetDateTime(10);
                            u.lastUpdate = reader.GetDateTime(11);

                            u.userPrivilegeStr = u.UserPrivilegeStr();

                            userList.Add(u);
                        }
                    }
                }

                return status;
            }
        }

        public int GetUserCount()
        {
            int cnt = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT COUNT(*) FROM Users;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cnt = reader.GetInt32(0);
                            }
                        }
                    }

                    return cnt;
                }
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Exception getting user count: {0}", ex.Message);
                return 0;
            }

        }

        public User GetUser(string userId)
        {
            User u = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT UserId, UserName, WindowsID, Privilege, Enabled, Password, " +
                             "Tel, Mobile, Email, Secret, LastLogin, LastUpdate " +
                             "FROM Users WHERE UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                u = new User();

                                u.userId = reader.GetString(0);
                                u.userName = reader.SafeGetString(1);
                                u.windowsId = reader.SafeGetString(2);
                                u.privilege = reader.SafeGetInt(3);
                                u.enabled = reader.SafeGetByte(4) > 0;
                                u.password = reader.SafeGetString(5);
                                u.phone = reader.SafeGetString(6);
                                u.mobile = reader.SafeGetString(7);
                                u.email = reader.SafeGetString(8);
                                u.secret = reader.SafeGetString(9);
                                u.lastLogin = reader.SafeGetDateTime(10);
                                u.lastUpdate = reader.GetDateTime(11);

                                u.userPrivilegeStr = u.UserPrivilegeStr();
                            }
                        }
                    }

                    return u;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error getting user '{0}' details: {1}", userId, ex.Message);
                return null;
            }

        }

        public User GetWinUser(string winId)
        {
            User u = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT UserId, UserName, WindowsID, Privilege, Enabled, Password, " +
                             "Tel, Mobile, Email, Secret, LastLogin, LastUpdate " +
                             "FROM Users WHERE WindowsID=@WindowsID;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("WindowsID", winId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                u = new User();

                                u.userId = reader.GetString(0);
                                u.userName = reader.SafeGetString(1);
                                u.windowsId = reader.SafeGetString(2);
                                u.privilege = reader.SafeGetInt(3);
                                u.enabled = reader.SafeGetByte(4) > 0;
                                u.password = reader.SafeGetString(5);
                                u.phone = reader.SafeGetString(6);
                                u.mobile = reader.SafeGetString(7);
                                u.email = reader.SafeGetString(8);
                                u.secret = reader.SafeGetString(9);
                                u.lastLogin = reader.SafeGetDateTime(10);
                                u.lastUpdate = reader.GetDateTime(11);

                                u.userPrivilegeStr = u.UserPrivilegeStr();
                            }
                        }
                    }

                    return u;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error getting windows user ID '{0}' details: {1}", winId, ex.Message);
                return null;
            }

        }

        public bool AddUser(User user, string curUserId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "INSERT INTO Users (UserId, UserName, WindowsID, Privilege, Password, Enabled, Tel, Mobile, Email) " +
                             "VALUES (@UserId, @UserName, @WindowsID, @Privilege, @Password, @Enabled, @Tel, @Mobile, @Email);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", user.userId);
                        cmd.Parameters.AddWithValue("UserName", user.userName);
                        cmd.Parameters.AddWithValue("WindowsID", user.windowsId);
                        cmd.Parameters.AddWithValue("Privilege", user.privilege);
                        cmd.Parameters.AddWithValue("Password", user.password);
                        cmd.Parameters.AddWithValue("Enabled", user.enabled ? 1 : 0);
                        cmd.Parameters.AddWithValue("Tel", user.phone);
                        cmd.Parameters.AddWithValue("Mobile", user.mobile);
                        cmd.Parameters.AddWithValue("Email", user.email);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added User {0}", user.userId),
                            TableName = "Users",
                            UserId = curUserId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();
                }

                return status;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error adding user '{0}': {1}", user.userId, ex.Message);
                return false;
            }

        }

        public bool CreateDefaultUsers()
        {
            bool status = false;

            User user = new User();
            if (GetUserCount() > 0) return false;

            user.userId = "admin";
            user.userName = "Administrator";
            user.privilege = (int)UserPrivilege.Administrator;
            user.enabled = true;
            user.password = Global.testingMode ? 
                Util.EncryptPassword("admin", user.privilege) : 
                Util.EncryptPassword("T6VrdFoy", user.privilege);
            AddUser(user, "admin");

            user.userId = "webapi";
            user.userName = "WebApi";
            user.privilege = (int)UserPrivilege.WebApi;
            user.enabled = true;
            user.password = Util.EncryptPassword("mAiXNx7J", user.privilege);
            AddUser(user, "admin");

            //user.userId = "sales";
            //user.userName = "Sales";
            //user.privilege = (int)UserPrivilege.Sales;
            //user.enabled = true;
            //user.password = Global.testingMode ?
            //    Util.EncryptPassword("sales", user.privilege) : 
            //    Util.EncryptPassword("iJAqYGV2", user.privilege);
            //AddUser(user, "admin");

            user.userId = "service";
            user.userName = "service";
            user.privilege = (int)UserPrivilege.CustomerService;
            user.enabled = true;
            user.password = Global.testingMode ?
                Util.EncryptPassword("service", user.privilege) : 
                Util.EncryptPassword("hIGahahq", user.privilege);
            AddUser(user, "admin");

            //user.userId = "reporting";
            //user.userName = "reporting";
            //user.privilege = (int)UserPrivilege.Reporting;
            //user.enabled = true;
            //user.password = Global.testingMode ?
            //    Util.EncryptPassword("reporting", user.privilege) : 
            //    Util.EncryptPassword("WnVnZoxn", user.privilege);
            //AddUser(user, "admin");

            user.userId = "dataentry";
            user.userName = "Data Entry";
            user.privilege = (int)UserPrivilege.DataEntry;
            user.enabled = true;
            user.password = Global.testingMode ?
                Util.EncryptPassword("dataentry", user.privilege) : 
                Util.EncryptPassword("6ci8vKYc", user.privilege);
            AddUser(user, "admin");

            //user.userId = "guest";
            //user.userName = "Guest";
            //user.privilege = (int)UserPrivilege.Guest;
            //user.enabled = true;
            //user.password = Util.EncryptPassword("guest", user.privilege);
            //AddUser(user, "admin");

            return status;
        }

        public bool UpdateUser(User user, bool changePassword, string curUserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    bool status = false;
                    string sqlSegment = changePassword ? ", Password=@Password " : "";
                    string sql = "UPDATE Users SET UserName=@UserName, WindowsID=@WindowsID, Privilege=@Privilege, " +
                                 "Tel=@Tel, Mobile=@Mobile, Email=@Email, Enabled=@Enabled " +
                                 sqlSegment +
                                 "WHERE UserId=@UserId;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", user.userId);
                        cmd.Parameters.AddWithValue("UserName", user.userName);
                        cmd.Parameters.AddWithValue("WindowsID", user.windowsId);
                        cmd.Parameters.AddWithValue("Privilege", user.privilege);
                        cmd.Parameters.AddWithValue("Tel", user.phone);
                        cmd.Parameters.AddWithValue("Mobile", user.mobile);
                        cmd.Parameters.AddWithValue("Email", user.email);
                        cmd.Parameters.AddWithValue("Enabled", user.enabled ? 1 : 0);
                        cmd.Parameters.AddWithValue("Password", user.password);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Update User {0}", user.userId),
                            TableName = "Users",
                            UserId = curUserId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating user '{0}': {1}", user.userId, ex.Message);
                return false;
            }
        }

        public bool UpdateUserPassword(int userId, string password, string curUserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    bool status = false;
                    string sql = "UPDATE Users SET Password=@Password WHERE UserId=@UserId;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", userId);
                        cmd.Parameters.AddWithValue("Password", password);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Update password for User {0}", userId),
                            TableName = "Users",
                            UserId = curUserId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating user '{0}': {1}", userId, ex.Message);
                return false;
            }
        }

        public bool UpdateUserOTPSecret(User user, string secret, string curUserId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Users SET Secret=@Secret WHERE UserId=@UserId;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", user.userId);

                        byte[] enc = AesEncryption.AesEncrypt(secret, Global.aesKey, Global.aesIV);
                        string encSecret = Convert.ToBase64String(enc);
                        cmd.Parameters.AddWithValue("Secret", encSecret);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Update password for User {0}", user.userId),
                            TableName = "Users",
                            UserId = curUserId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating secret for user '{0}': {1}", user.userId, ex.Message);
                return false;
            }
        }

        public string GetUserOTPSecret(int userId)
        {
            string secret = null;
            int privilege = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT Secret, Privilege FROM Users WHERE UserId=@UserId;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                secret = reader.SafeGetString(0);
                                privilege = reader.SafeGetInt(1);
                            }
                        }

                        if (secret.Length > 0)
                        {
                            byte[] bytes = Convert.FromBase64String(secret);

                            secret = AesEncryption.AesDecrypt(bytes, Global.aesKey, Global.aesIV);
                        }
                    }

                    return secret;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error getting secret for user '{0}': {1}", userId, ex.Message);
                return null;
            }
        }

        public bool UpdateUserLogin(User user)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Users SET LastLogin=@LastLogin " +
                                 "WHERE UserId=@UserId;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", user.userId);
                        cmd.Parameters.AddWithValue("LastLogin", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating user login for '{0}': {1}", user.userId, ex.Message);
                return false;
            }
        }

        public bool DeleteUser(string userId, string curUserId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Users WHERE UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("UserId", userId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added User {0}", userId),
                            TableName = "Quotations",
                            UserId = curUserId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting user '{0}': {1}", userId, ex.Message);
                return false;
            }

        }

        public bool HasUserId(string userId)
        {
            bool status = true;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Users WHERE UserId=@UserId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cnt = reader.SafeGetInt(0);
                            status = (cnt > 0);
                        }
                    }
                }

                return status;
            }
        }
        #endregion

        #region Session
        //*************************************************************
        // Session
        //*************************************************************
        public Session CreateSession(string userId, string data)
        {
            string sql;
            bool status;

            Session session = new Session()
            {
                
                sessionId = Guid.NewGuid().ToString("N"),
                userId = userId,
                sessionData = data
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    int cnt = 0;
                    // try update first
                    sql = "UPDATE Session SET UserId=@UserId, SessionId=@SessionId, SessionData=@SessionData;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("SessionId", session.sessionId);
                        cmd.Parameters.AddWithValue("UserId", session.userId);
                        cmd.Parameters.AddWithValue("SessionData", session.sessionData);

                        cnt = cmd.ExecuteNonQuery();
                    }

                    if (cnt == 0) // needs an insert
                    {
                        sql = "INSERT INTO Session(UserId, SessionId, SessionData) " +
                                "VALUES (@UserId, @SessionId, @SessionData);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("SessionId", session.sessionId);
                            cmd.Parameters.AddWithValue("UserId", session.userId);
                            cmd.Parameters.AddWithValue("SessionData", session.sessionData);

                            cnt = cmd.ExecuteNonQuery();
                        }
                    }

                    status = cnt > 0;

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status ? session : null;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error creating new session for user '{0}': {1}", userId, ex.Message);
                return null;
            }

        }

        public Session GetSession(string sessionId)
        {
            Session session = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT UserId, SessionData,LastUpdate FROM Session " +
                         "WHERE SessionId=@SessionId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("SessionId", sessionId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            session = new Session();
                            session.sessionId = sessionId;
                            session.userId = reader.SafeGetString(0);
                            session.sessionData = reader.SafeGetString(1);
                            session.lastUpdate = reader.GetDateTime(2);
                        }
                    }
                }

                return session;
            }
        }

        public bool UpdateSessionData(Session session)
        {
            bool status;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE Session SET SessionData=@SessionData " +
                             "WHERE SessionId=@SessionId AND UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("SessionId", session.sessionId);
                        cmd.Parameters.AddWithValue("UserId", session.userId);
                        cmd.Parameters.AddWithValue("SessionData", session.sessionData);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating session data: {0}", ex.Message);
                return false;
            }
        }

        public bool DeleteSession(string sessionId)
        {
            bool status;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "DELETE FROM Session WHERE SessionId=@SessionId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("SessionId", sessionId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting session: {0}", ex.Message);
                return false;
            }
        }
        #endregion

        #region Tokens
        //*************************************************************
        // Tokens
        //*************************************************************
        public Token CreateToken(string userId)
        {
            bool status;

            try
            {
                Token token = GetToken(userId); // userId and cellId must exist
                if (token == null) return null;

                token.token = Guid.NewGuid().ToString("N");
                token.expiry = DateTime.Now.AddSeconds(Global.app.par["WebApiTokenValidity"]);
                token.lastAccess = DateTime.Now;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE Tokens SET Token=@Token, Expiry=@Expiry, LastAccess=@LastAccess " +
                             "WHERE UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", token.userId);
                        cmd.Parameters.AddWithValue("Token", token.token);
                        cmd.Parameters.AddWithValue("Expiry", token.expiry.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastAccess", token.lastAccess.SafeDateTime());

                        status = (cmd.ExecuteNonQuery() >= 1); // duplicate key action counts as another operation
                    }

                    return status ? token : null;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error creating new token for user '{0}': {1}", userId, ex.Message);
                return null;
            }
        }

        public Token GetToken(string userId)
        {
            Token token = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Token, Expiry, LastAccess FROM Tokens " +
                         "WHERE UserId=@UserId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            token = new Token();
                            token.userId = userId;
                            token.token = reader.SafeGetString(0);
                            token.expiry = reader.SafeGetDateTime(1);
                            token.lastAccess = reader.SafeGetDateTime(2);
                        }
                    }
                }

                if (token != null)
                {
                    UpdateTokenAccessTime(token);
                }

                return token;
            }
        }

        public Token GetTokenFromTokenStr(string tokenStr)
        {
            Token token = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT UserId, Token, Expiry, LastAccess FROM Tokens " +
                         "WHERE Token=@Token;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Token", tokenStr);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            token = new Token();
                            token.userId = reader.SafeGetString(0);
                            token.token = reader.SafeGetString(1);
                            token.expiry = reader.SafeGetDateTime(2);
                            token.lastAccess = reader.SafeGetDateTime(3);
                        }
                    }
                }

                if (token != null)
                {
                    UpdateTokenAccessTime(token);
                }

                return token;
            }
        }

        public bool UpdateTokenAccessTime(Token token)
        {
            bool status;

            token.lastAccess = DateTime.Now;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "UPDATE Tokens SET LastAccess=@LastAccess " +
                             "WHERE UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("LastAccess", token.lastAccess.SafeDateTime());
                        cmd.Parameters.AddWithValue("UserId", token.userId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating token for user '{0}': {1}", token.userId, ex.Message);
                return false;
            }
        }

        public bool DeleteToken(string userId)
        {
            bool status;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "DELETE FROM Tokens WHERE UserId=@UserId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("UserId", userId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting token for user '{0}': {1}", userId, ex.Message);
                return false;
            }
        }
        #endregion

        #region Quotations
        //*********************************************************************
        // Quotations
        //*********************************************************************

        public bool GetQuotations(List<Quotation> quoList, string filterConfirmed, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            string sqlwhere = "";
            total = 0;
            bool status = false;
            quoList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }

                if (filterConfirmed == "Confirmed")
                {
                    sqlwhere = "WHERE Confirmed=1";
                }
                else if (filterConfirmed == "Not Confirmed")
                {
                    sqlwhere = "WHERE Confirmed=0";
                }

                sql = string.Format("SELECT QuoNum, BillToParty, SoldToParty, QuoDate, CustomerNum, Currency, " +
                                    "AttnName, AttnTel, AttnMobile, AttnFax, AttnEmail, QuoSubject, Price, PaymentTerms, Confirmed, " +
                                    "Filename, UploadUser, Uploaded, LastUpdateUser, LastUpdate " +
                                    "FROM Quotations {1} ORDER BY QuoNum {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Quotation q = new Quotation();

                            q.QuoNum = reader.SafeGetInt(0);
                            q.BillToParty = reader.SafeGetString(1);
                            q.SoldToParty = reader.SafeGetString(2);
                            q.QuoDate = reader.SafeGetDateTime(3);
                            q.CustomerNum = reader.SafeGetLong(4);
                            q.Currency = reader.SafeGetString(5);
                            q.AttnName = reader.SafeGetString(6);
                            q.AttnTel = reader.SafeGetString(7);
                            q.AttnMobile = reader.SafeGetString(8);
                            q.AttnFax = reader.SafeGetString(9);
                            q.AttnEmail = reader.SafeGetString(10);
                            q.QuoSubject = reader.SafeGetString(11);
                            q.Price = reader.GetDouble(12);
                            q.PaymentTerms = reader.SafeGetString(13);
                            q.Confirmed = reader.SafeGetByte(14) > 0;
                            q.Filename = reader.SafeGetString(15);
                            q.UploadUser = reader.SafeGetString(16);
                            q.Uploaded = reader.SafeGetDateTime(17);
                            q.LastUpdateUser = reader.SafeGetString(18);
                            q.LastUpdate = reader.SafeGetDateTime(19);

                            quoList.Add(q);
                        }

                        status = true;
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM Quotations {0};", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public Quotation GetQuotation(int quoNum)
        {
            Quotation q = null;

            #region debug-point QD-B
            Global.DebugReport("pre-fix", "B", "Database.cs:1969", "Enter GetQuotation", new { quoNum });
            #endregion

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT QuoNum, BillToParty, SoldToParty, QuoDate, CustomerNum, Currency, " +
                             "AttnName, AttnTel, AttnMobile, AttnFax, AttnEmail, QuoSubject, Price, PaymentTerms, " +
                             "Confirmed, Filename, UploadUser, Uploaded, LastUpdateUser, LastUpdate " +
                             "FROM Quotations WHERE QuoNum=@QuoNum;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("QuoNum", quoNum);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            q = new Quotation();

                            q.QuoNum = reader.SafeGetInt(0);
                            q.BillToParty = reader.SafeGetString(1);
                            q.SoldToParty = reader.SafeGetString(2);
                            q.QuoDate = reader.SafeGetDateTime(3);
                            q.CustomerNum = reader.SafeGetLong(4);
                            q.Currency = reader.SafeGetString(5);
                            q.AttnName = reader.SafeGetString(6);
                            q.AttnTel = reader.SafeGetString(7);
                            q.AttnMobile = reader.SafeGetString(8);
                            q.AttnFax = reader.SafeGetString(9);
                            q.AttnEmail = reader.SafeGetString(10);
                            q.QuoSubject = reader.SafeGetString(11);
                            q.Price = reader.GetDouble(12);
                            q.PaymentTerms = reader.SafeGetString(13);
                            q.Confirmed = reader.SafeGetByte(14) > 0;
                            q.Filename = reader.SafeGetString(15);
                            q.UploadUser = reader.SafeGetString(16);
                            q.Uploaded = reader.SafeGetDateTime(17);
                            q.LastUpdateUser = reader.SafeGetString(18);
                            q.LastUpdate = reader.SafeGetDateTime(19);
                        }
                    }
                }

                #region debug-point QD-B
                Global.DebugReport("pre-fix", "B", "Database.cs:2013", "Exit GetQuotation", new { quoNum, found = q != null });
                #endregion

                return q;
            }
        }

        public bool HasQuotation(int quoNum)
        {
            string sql;
            int cnt = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT COUNT(*) FROM Quotations WHERE QuoNum=@Quotation;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Quotation", quoNum);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cnt = reader.SafeGetInt(0);
                        }
                    }
                }

                return cnt > 0;
            }
        }

        public bool InsertQuotation(Quotation q, string userId)
        {
            bool status = false;

            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "INSERT INTO Quotations (QuoNum, BillToParty, SoldToParty, QuoDate, CustomerNum, Currency, " +
                                 "AttnName, AttnTel, AttnMobile, AttnFax, AttnEmail, QuoSubject, Price, PaymentTerms, " +
                                 "Confirmed, Filename, UploadUser, Uploaded, LastUpdateUser, LastUpdate) " +
                                 "VALUES (@QuoNum, @BillToParty, @SoldToParty, @QuoDate, @CustomerNum, @Currency, " +
                                 "@AttnName, @AttnTel, @AttnMobile, @AttnFax, @AttnEmail, @QuoSubject, @Price, @PaymentTerms, " +
                                 "@Confirmed, @Filename, @UploadUser, @Uploaded, @LastUpdateUser, @LastUpdate);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("QuoNum", q.QuoNum);
                        cmd.Parameters.AddWithValue("BillToParty", q.BillToParty);
                        cmd.Parameters.AddWithValue("SoldToParty", q.SoldToParty);
                        cmd.Parameters.AddWithValue("QuoDate", q.QuoDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("CustomerNum", q.CustomerNum);
                        cmd.Parameters.AddWithValue("Currency", q.Currency);
                        cmd.Parameters.AddWithValue("AttnName", q.AttnName);
                        cmd.Parameters.AddWithValue("AttnTel", q.AttnTel);
                        cmd.Parameters.AddWithValue("AttnMobile", q.AttnMobile);
                        cmd.Parameters.AddWithValue("AttnFax", q.AttnFax);
                        cmd.Parameters.AddWithValue("AttnEmail", q.AttnEmail);
                        cmd.Parameters.AddWithValue("QuoSubject", q.QuoSubject);
                        cmd.Parameters.AddWithValue("Price", q.Price);
                        cmd.Parameters.AddWithValue("PaymentTerms", q.PaymentTerms);
                        cmd.Parameters.AddWithValue("Confirmed", q.Confirmed);
                        cmd.Parameters.AddWithValue("Filename", q.Filename);
                        cmd.Parameters.AddWithValue("UploadUser", q.UploadUser);
                        cmd.Parameters.AddWithValue("Uploaded", q.Uploaded.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", q.LastUpdate.SafeDateTime());

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting quotation into database.", ex.Message);
                return false;
            }
        }

        public bool UpdateQuotation(Quotation q, string userId)
        {
            bool status = false;

            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Quotations SET BillToParty=@BillToParty, SoldToParty=@SoldToParty, QuoDate=@QuoDate, " +
                                 "CustomerNum=@CustomerNum, Currency=@Currency, " +
                                 "AttnName=@AttnName, AttnTel=@AttnTel, AttnMobile=@AttnMobile, AttnFax=@AttnFax, " +
                                 "AttnEmail=@AttnEmail, QuoSubject=@QuoSubject, Price=@Price, PaymentTerms=@PaymentTerms, Confirmed=@Confirmed, " +
                                 "Filename=@Filename, UploadUser=@UploadUser, Uploaded=@Uploaded, LastUpdateUser=@LastUpdateUser, " +
                                 "LastUpdate=@LastUpdate " +
                                 "WHERE QuoNum=@QuoNum;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("QuoNum", q.QuoNum);
                        cmd.Parameters.AddWithValue("BillToParty", q.BillToParty);
                        cmd.Parameters.AddWithValue("SoldToParty", q.SoldToParty);
                        cmd.Parameters.AddWithValue("QuoDate", q.QuoDate);
                        cmd.Parameters.AddWithValue("CustomerNum", q.CustomerNum);
                        cmd.Parameters.AddWithValue("Currency", q.Currency);
                        cmd.Parameters.AddWithValue("AttnName", q.AttnName);
                        cmd.Parameters.AddWithValue("AttnTel", q.AttnTel);
                        cmd.Parameters.AddWithValue("AttnMobile", q.AttnMobile);
                        cmd.Parameters.AddWithValue("AttnFax", q.AttnFax);
                        cmd.Parameters.AddWithValue("AttnEmail", q.AttnEmail);
                        cmd.Parameters.AddWithValue("QuoSubject", q.QuoSubject);
                        cmd.Parameters.AddWithValue("Price", q.Price);
                        cmd.Parameters.AddWithValue("PaymentTerms", q.PaymentTerms);
                        cmd.Parameters.AddWithValue("Confirmed", q.Confirmed);
                        cmd.Parameters.AddWithValue("Filename", q.Filename);
                        cmd.Parameters.AddWithValue("UploadUser", q.UploadUser);
                        cmd.Parameters.AddWithValue("Uploaded", q.Uploaded.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Update Quotation {0}", q.QuoNum),
                            TableName = "Quotations",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating quotation '{0}': {1}", q.QuoNum, ex.Message);
                return false;
            }
        }

        public bool DeleteQuotation(int quoNum, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Quotations WHERE QuoNum=@QuoNum;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("QuoNum", quoNum);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Delete QuoNum={0}", quoNum),
                            TableName = "Quotations",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting quotation '{0}': {1}", quoNum, ex.Message);
                return false;
            }
        }
        #endregion
        #region ActivityLog
        //*********************************************************************
        // Activity Log
        //*********************************************************************

        public bool LogActivity(ActivityLog log, SqlConnection conn, SqlTransaction trans)
        {
            bool status = false;

            try
            {
                string sql = "INSERT INTO ActivityLog (Activity, TableName, UserId) VALUES " +
                            "(@Activity, @TableName, @UserId);";
                using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("Activity", log.Activity);
                    cmd.Parameters.AddWithValue("TableName", log.TableName);
                    cmd.Parameters.AddWithValue("UserId", log.UserId);

                    status = (cmd.ExecuteNonQuery() == 1);
                }

                return status;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error logging activity '{0}': {1}", log.Activity, ex.Message);
                return false;
            }
        }
        #endregion
        #region Projects
        //*********************************************************************
        // Projects
        //*********************************************************************

        public bool GetProjects(List<Project> projList, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            projList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                sql = string.Format("SELECT Id, PaymentCode, CurSCONum, Quotation, " +
                                    "ProjectName, BillToCompany, BillToAddress, SoldToCompany, SoldToAddress, " +
                                    "ReportsRequired, ApplicantName, ApplicantDesignation, " +
                                    "EmailAddr, EmailCC, PricePerCube, " +
                                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                                    "FROM Projects ORDER BY Id {0};", sqlseg);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project p = new Project();

                            p.Id = reader.SafeGetInt(0);
                            p.PaymentCode = reader.SafeGetString(1);
                            p.CurSCONum = reader.SafeGetInt(2);
                            p.Quotation = reader.SafeGetInt(3);
                            p.ProjectName = reader.SafeGetString(4);
                            p.BillToCompany = reader.SafeGetString(5);
                            p.BillToAddress = reader.SafeGetString(6);
                            p.SoldToCompany = reader.SafeGetString(7);
                            p.SoldToAddress = reader.SafeGetString(8);
                            p.ReportsRequired = reader.SafeGetString(9);
                            p.ApplicantName = reader.SafeGetString(10);
                            p.ApplicantDesignation = reader.SafeGetString(11);
                            p.EmailAddr = Util.LinesToStringList(reader.SafeGetString(12));
                            p.EmailCC = Util.LinesToStringList(reader.SafeGetString(13));
                            p.PricePerCube = reader.GetDouble(14);
                            p.CreateUser = reader.SafeGetString(15);
                            p.CreateDate = reader.SafeGetDateTime(16);
                            p.LastUpdateUser = reader.SafeGetString(17);
                            p.LastUpdate = reader.SafeGetDateTime(18);

                            p.ReadSync();
                            projList.Add(p);
                        }
                    }
                }

                sql = "SELECT COUNT(*) FROM Projects;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public bool CountProjectsWithQuotation(int quoNum)
        {
            bool status = false;

            #region debug-point QD-C
            Global.DebugReport("pre-fix", "C", "Database.cs:1835", "Enter CountProjectsWithQuotation", new { quoNum });
            #endregion

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Projects WHERE Quotation=@Quotation;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Quotation", quoNum);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            status = reader.SafeGetInt(0) > 0;
                        }
                    }
                }

                #region debug-point QD-C
                Global.DebugReport("pre-fix", "C", "Database.cs:1857", "Exit CountProjectsWithQuotation", new { quoNum, status });
                #endregion

                return status;
            }
        }

        public Project GetProject(int projId)
        {
            Project p = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, PaymentCode, CurSCONum, Quotation, " +
                            "ProjectName, BillToCompany, BillToAddress, SoldToCompany, SoldToAddress, " +
                            "ReportsRequired, ApplicantName, ApplicantDesignation, " +
                            "EmailAddr, EmailCC, PricePerCube, " +
                            "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                            "FROM Projects WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", projId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Project();

                            p.Id = reader.SafeGetInt(0);
                            p.PaymentCode = reader.SafeGetString(1);
                            p.CurSCONum = reader.SafeGetInt(2);
                            p.Quotation = reader.SafeGetInt(3);
                            p.ProjectName = reader.SafeGetString(4);
                            p.BillToCompany = reader.SafeGetString(5);
                            p.BillToAddress = reader.SafeGetString(6);
                            p.SoldToCompany = reader.SafeGetString(7);
                            p.SoldToAddress = reader.SafeGetString(8);
                            p.ReportsRequired = reader.SafeGetString(9);
                            p.ApplicantName = reader.SafeGetString(10);
                            p.ApplicantDesignation = reader.SafeGetString(11);
                            p.EmailAddr = Util.LinesToStringList(reader.SafeGetString(12));
                            p.EmailCC = Util.LinesToStringList(reader.SafeGetString(13));
                            p.PricePerCube = reader.GetDouble(14);
                            p.CreateUser = reader.SafeGetString(15);
                            p.CreateDate = reader.SafeGetDateTime(16);
                            p.LastUpdateUser = reader.SafeGetString(17);
                            p.LastUpdate = reader.SafeGetDateTime(18);

                            p.ReadSync();
                        }
                    }
                }

                return p;
            }
        }

        public bool GetProjectsWithTestedCubes(DateTime dtStart, DateTime dtEnd, List<Project> projects)
        {
            string sql;
            bool status = false;
            projects.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT p.Id, PaymentCode, CurSCONum, Quotation, " +
                    "ProjectName, BillToCompany, BillToAddress, SoldToCompany, SoldToAddress, " +
                    "ReportsRequired, ApplicantName, ApplicantDesignation, " +
                    "EmailAddr, EmailCC, PricePerCube, " +
                    "p.CreateUser, p.CreateDate, p.LastUpdateUser, p.LastUpdate " +
                    "FROM Projects p, CubeSets, Batches, Cubes " +
                    "WHERE p.Id=CubeSets.ProjectId AND CubeSets.Id=Batches.CubeSetId AND " +
                    "Batches.ScoNum=Cubes.ScoNum AND Batches.Id=Cubes.BatchId AND Cubes.TestResult>0 AND " +
                    "CAST(Cubes.ActualTestDate AS DATE)>=CAST(@StartDate AS DATE) AND " +
                    "CAST(Cubes.ActualTestDate AS DATE)<=CAST(@EndDate AS DATE) " +
                    "GROUP BY p.Id, PaymentCode, CurSCONum, Quotation, " +
                    "ProjectName, BillToCompany, BillToAddress, SoldToCompany, SoldToAddress, " +
                    "ReportsRequired, ApplicantName, ApplicantDesignation, " +
                    "EmailAddr, EmailCC, PricePerCube, " +
                    "p.CreateUser, p.CreateDate, p.LastUpdateUser, p.LastUpdate;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("StartDate", dtStart);
                    cmd.Parameters.AddWithValue("EndDate", dtEnd);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project p = new Project();

                            p.Id = reader.SafeGetInt(0);
                            p.PaymentCode = reader.SafeGetString(1);
                            p.CurSCONum = reader.SafeGetInt(2);
                            p.Quotation = reader.SafeGetInt(3);
                            p.ProjectName = reader.SafeGetString(4);
                            p.BillToCompany = reader.SafeGetString(5);
                            p.BillToAddress = reader.SafeGetString(6);
                            p.SoldToCompany = reader.SafeGetString(7);
                            p.SoldToAddress = reader.SafeGetString(8);
                            p.ReportsRequired = reader.SafeGetString(9);
                            p.ApplicantName = reader.SafeGetString(10);
                            p.ApplicantDesignation = reader.SafeGetString(11);
                            p.EmailAddr = Util.LinesToStringList(reader.SafeGetString(12));
                            p.EmailCC = Util.LinesToStringList(reader.SafeGetString(13));
                            p.PricePerCube = reader.GetDouble(14);
                            p.CreateUser = reader.SafeGetString(15);
                            p.CreateDate = reader.SafeGetDateTime(16);
                            p.LastUpdateUser = reader.SafeGetString(17);
                            p.LastUpdate = reader.SafeGetDateTime(18);

                            p.ReadSync();
                            projects.Add(p);
                        }

                        status = true;
                    }
                }

            }

            return status;
        }

        public bool GetTestedStats(int dayCnt, out List<object> cubesTested, out List<object> batchesTested,
            out List<object>cubeSetsTested, out List<object> projectsTested)
        {
            string sql;
            cubesTested = new List<object>();
            batchesTested = new List<object>();
            cubeSetsTested = new List<object>();
            projectsTested = new List<object>();

            List<Cube> cubes = new List<Cube>();
            Dictionary<string, Batch> batches = new Dictionary<string, Batch>();
            Dictionary<int, CubeSet> cubeSets = new Dictionary<int, CubeSet>();
            Dictionary<int, Project> projects = new Dictionary<int, Project>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                for (int i=dayCnt; i>0; i--)
                {
                    DateTime d = DateTime.Today.AddDays(-i);

                    // get cubes
                    cubes.Clear();
                    sql = "SELECT Barcode, ScoNum, BatchId FROM Cubes " +
                        "WHERE CAST(ActualTestDate AS DATE)=CAST(@TestDate AS DATE);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("TestDate", d);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Cube c = new Cube();
                                c.Barcode = reader.SafeGetLong(0);
                                c.ScoNum = reader.SafeGetInt(1);
                                c.BatchId = reader.SafeGetInt(2);

                                cubes.Add(c);
                            }
                        }
                    }

                    cubesTested.Add(cubes.Count);

                    // find batches
                    batches.Clear();
                    foreach(Cube c in cubes)
                    {
                        sql = "SELECT Id, ScoNum, CubeSetId FROM Batches WHERE Id=@Id AND ScoNum=@ScoNum;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                            cmd.Parameters.AddWithValue("Id", c.BatchId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Batch b = new Batch();
                                    b.Id = reader.SafeGetInt(0);
                                    b.ScoNum = reader.SafeGetInt(1);
                                    b.CubeSetId = reader.SafeGetInt(2);

                                    b.BatchNum = $"{b.ScoNum}-{b.Id}";

                                    if (!batches.ContainsKey(b.BatchNum))
                                    {
                                        batches.Add(b.BatchNum, b);
                                    }
                                }
                            }
                        }
                    }

                    batchesTested.Add(batches.Count);

                    // find cube sets
                    cubeSets.Clear();
                    foreach (Batch b in batches.Values)
                    {
                        sql = "SELECT Id, ProjectId FROM CubeSets WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("Id", b.CubeSetId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    CubeSet cs = new CubeSet();
                                    cs.Id = reader.SafeGetInt(0);
                                    cs.ProjectId = reader.SafeGetInt(1);

                                    if (!cubeSets.ContainsKey(cs.Id))
                                    {
                                        cubeSets.Add(cs.Id, cs);
                                    }
                                }
                            }
                        }
                    }

                    cubeSetsTested.Add(cubeSets.Count);

                    // find projects
                    projects.Clear();
                    foreach (CubeSet cs in cubeSets.Values)
                    {
                        sql = "SELECT Id FROM Projects WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("Id", cs.ProjectId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Project p = new Project();
                                    p.Id = reader.SafeGetInt(0);

                                    if (!projects.ContainsKey(p.Id))
                                    {
                                        projects.Add(p.Id, p);
                                    }
                                }
                            }
                        }
                    }

                    projectsTested.Add(projects.Count);
                }
            }

            return true;
        }

        public bool GetUntestedStats(int dayCnt, out List<object> cubesUntested, out List<object> batchesUntested,
            out List<object> cubeSetsUntested, out List<object> projectsUntested)
        {
            string sql;
            cubesUntested = new List<object>();
            batchesUntested = new List<object>();
            cubeSetsUntested = new List<object>();
            projectsUntested = new List<object>();

            List<Cube> cubes = new List<Cube>();
            Dictionary<string, Batch> batches = new Dictionary<string, Batch>();
            Dictionary<int, CubeSet> cubeSets = new Dictionary<int, CubeSet>();
            Dictionary<int, Project> projects = new Dictionary<int, Project>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                for (int i = dayCnt; i > 0; i--)
                {
                    DateTime d = DateTime.Today.AddDays(-i);

                    // get cubes
                    cubes.Clear();
                    sql = "SELECT Barcode, Cubes.ScoNum, Cubes.BatchId FROM Cubes, Batches " +
                        "WHERE Cubes.ScoNum=Batches.ScoNum AND Cubes.BatchId=Batches.Id AND " +
                        "CAST(Batches.TargetTestDate AS DATE)=CAST(@TestDate AS DATE) AND TestResult=0;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("TestDate", d);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Cube c = new Cube();
                                c.Barcode = reader.SafeGetLong(0);
                                c.ScoNum = reader.SafeGetInt(1);
                                c.BatchId = reader.SafeGetInt(2);

                                cubes.Add(c);
                            }
                        }
                    }

                    cubesUntested.Add(cubes.Count);

                    // find batches
                    batches.Clear();
                    foreach (Cube c in cubes)
                    {
                        sql = "SELECT Id, ScoNum, CubeSetId FROM Batches WHERE Id=@Id AND ScoNum=@ScoNum;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                            cmd.Parameters.AddWithValue("Id", c.BatchId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Batch b = new Batch();
                                    b.Id = reader.SafeGetInt(0);
                                    b.ScoNum = reader.SafeGetInt(1);
                                    b.CubeSetId = reader.SafeGetInt(2);

                                    b.BatchNum = $"{b.ScoNum}-{b.Id}";

                                    if (!batches.ContainsKey(b.BatchNum))
                                    {
                                        batches.Add(b.BatchNum, b);
                                    }
                                }
                            }
                        }
                    }

                    batchesUntested.Add(batches.Count);

                    // find cube sets
                    cubeSets.Clear();
                    foreach (Batch b in batches.Values)
                    {
                        sql = "SELECT Id, ProjectId FROM CubeSets WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("Id", b.CubeSetId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    CubeSet cs = new CubeSet();
                                    cs.Id = reader.SafeGetInt(0);
                                    cs.ProjectId = reader.SafeGetInt(1);

                                    if (!cubeSets.ContainsKey(cs.Id))
                                    {
                                        cubeSets.Add(cs.Id, cs);
                                    }
                                }
                            }
                        }
                    }

                    cubeSetsUntested.Add(cubeSets.Count);

                    // find projects
                    projects.Clear();
                    foreach (CubeSet cs in cubeSets.Values)
                    {
                        sql = "SELECT Id FROM Projects WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("Id", cs.ProjectId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Project p = new Project();
                                    p.Id = reader.SafeGetInt(0);

                                    if (!projects.ContainsKey(p.Id))
                                    {
                                        projects.Add(p.Id, p);
                                    }
                                }
                            }
                        }
                    }

                    projectsUntested.Add(projects.Count);
                }
            }

            return true;
        }

        public string GetProjectCode(int projId)
        {
            string pcode = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, PaymentCode " +
                            "FROM Projects WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", projId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.SafeGetInt(0);
                            string pc = reader.SafeGetString(1);
                            pcode = $"{pc}{id:D4}";
                        }
                    }
                }

                return pcode;
            }
        }

        
        

        public bool InsertProject(Project p, string userId)
        {
            bool status = false;
            int projId = 0;
            string sql;

            if (p.CurSCONum == 0) return false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    p.CreateUser = userId;
                    p.CreateDate = DateTime.Now;
                    p.WriteSync();

                    SqlTransaction trans = conn.BeginTransaction();

                    // make sure the sco no. has not been used
                    int cnt = 0;
                    sql = "SELECT COUNT(*) FROM SCONumbers WHERE ScoNum=@ScoNum;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("SCONum", p.CurSCONum);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cnt = reader.SafeGetInt(0);
                            }
                        }
                    }

                    if (cnt > 0)
                    {
                        trans.Rollback();
                        return false;
                    }

                    sql = "SELECT COUNT(*) FROM Projects WHERE Quotation=@Quotation;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Quotation", p.Quotation);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cnt = reader.SafeGetInt(0);
                            }
                        }
                    }

                    if (cnt > 0)
                    {
                        trans.Rollback();
                        return false;
                    }

                    sql = "INSERT INTO Projects (PaymentCode, CurSCONum, Quotation, " +
                            "ProjectName, BillToCompany, BillToAddress, SoldToCompany, SoldToAddress, " +
                            "ReportsRequired, ApplicantName, ApplicantDesignation, " +
                            "EmailAddr, EmailCC, PricePerCube, " +
                            "CreateUser, CreateDate, LastUpdateUser) " +
                            "VALUES (@PaymentCode, @CurSCONum, @Quotation, " +
                            "@ProjectName, @BillToCompany, @BillToAddress, @SoldToCompany, @SoldToAddress, " +
                            "@ReportsRequired, @ApplicantName, @ApplicantDesignation, " +
                            "@EmailAddr, @EmailCC, @PricePerCube, " +
                            "@CreateUser, @CreateDate, @LastUpdateUser);" +
                            "SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        //cmd.Parameters.AddWithValue("Id", p.Id);
                        cmd.Parameters.AddWithValue("PaymentCode", p.PaymentCode);
                        cmd.Parameters.AddWithValue("CurSCONum", p.CurSCONum);
                        cmd.Parameters.AddWithValue("Quotation", p.Quotation);
                        cmd.Parameters.AddWithValue("ProjectName", p.ProjectName);
                        cmd.Parameters.AddWithValue("BillToCompany", p.BillToCompany);
                        cmd.Parameters.AddWithValue("BillToAddress", p.BillToAddress);
                        cmd.Parameters.AddWithValue("SoldToCompany", p.SoldToCompany);
                        cmd.Parameters.AddWithValue("SoldToAddress", p.SoldToAddress);
                        cmd.Parameters.AddWithValue("ReportsRequired", p.ReportsRequired);
                        cmd.Parameters.AddWithValue("ApplicantName", p.ApplicantName);
                        cmd.Parameters.AddWithValue("ApplicantDesignation", p.ApplicantDesignation);
                        cmd.Parameters.AddWithValue("EmailAddr", Util.StringListToLines(p.EmailAddr));
                        cmd.Parameters.AddWithValue("EmailCC", Util.StringListToLines(p.EmailCC));
                        cmd.Parameters.AddWithValue("PricePerCube", p.PricePerCube);
                        cmd.Parameters.AddWithValue("CreateUser", p.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", p.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                projId = reader.SafeGetDecimal(0);
                            }
                        }
                        status = projId > 0;
                    }

                    if (status)
                    {
                        p.Id = projId;
                        p.ReadSync();
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added Project {0}{1:0000}", p.PaymentCode, projId),
                            TableName = "Projects",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    // insert sco number
                    if (status)
                    {
                        SCONumber sco = new SCONumber()
                        {
                            ScoNumber = p.CurSCONum,
                            ProjectId = p.Id,
                            LastUpdateUser = userId
                        };

                        sql = "INSERT INTO SCONumbers (SCONum, ProjectId, LastUpdateUser) " +
                            "VALUES (@SCONum, @ProjectId, @LastUpdateUser);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("SCONum", sco.ScoNumber);
                            cmd.Parameters.AddWithValue("ProjectId", sco.ProjectId);
                            cmd.Parameters.AddWithValue("LastUpdateUser", sco.LastUpdateUser);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }

                        if (status)
                        {
                            ActivityLog log = new ActivityLog()
                            {
                                Activity = string.Format("Added New SCO Number {0}", sco.ScoNumber),
                                TableName = "SCO Numbers",
                                UserId = userId
                            };

                            status = LogActivity(log, conn, trans);
                        }

                        if (status) trans.Commit();
                        else trans.Rollback();
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting project '{0}': {1}", p.ProjectCode, ex.Message);
                return false;
            }
        }

        public bool UpdateProject(Project p, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    p.WriteSync();
                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Projects SET CurSCONum=@CurSCONum, Quotation=@Quotation, " +
                                "ProjectName=@ProjectName, BillToCompany=@BillToCompany, BillToAddress=@BillToAddress, " +
                                "SoldToCompany=@SoldToCompany, SoldToAddress=@SoldToAddress, " +
                                "ReportsRequired=@ReportsRequired, ApplicantName=@ApplicantName, ApplicantDesignation=@ApplicantDesignation, " +
                                "EmailAddr=@EmailAddr, EmailCC=@EmailCC, PricePerCube=@PricePerCube, " +
                                "CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, " +
                                "LastUpdate=@LastUpdate " +
                                "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", p.Id);
                        cmd.Parameters.AddWithValue("CurSCONum", p.CurSCONum);
                        cmd.Parameters.AddWithValue("Quotation", p.Quotation);
                        cmd.Parameters.AddWithValue("ProjectName", p.ProjectName);
                        cmd.Parameters.AddWithValue("BillToCompany", p.BillToCompany);
                        cmd.Parameters.AddWithValue("BillToAddress", p.BillToAddress);
                        cmd.Parameters.AddWithValue("SoldToCompany", p.SoldToCompany);
                        cmd.Parameters.AddWithValue("SoldToAddress", p.SoldToAddress);
                        cmd.Parameters.AddWithValue("ReportsRequired", p.ReportsRequired);
                        cmd.Parameters.AddWithValue("ApplicantName", p.ApplicantName);
                        cmd.Parameters.AddWithValue("ApplicantDesignation", p.ApplicantDesignation);
                        cmd.Parameters.AddWithValue("EmailAddr", Util.StringListToLines(p.EmailAddr));
                        cmd.Parameters.AddWithValue("EmailCC", Util.StringListToLines(p.EmailCC));
                        cmd.Parameters.AddWithValue("PricePerCube", p.PricePerCube);
                        cmd.Parameters.AddWithValue("CreateUser", p.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", p.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Project {0}{1:0000}", p.PaymentCode, p.Id),
                            TableName = "Projects",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating project '{0}': {1}", p.ProjectCode, ex.Message);
                return false;
            }
        }

        public bool UpdateProjectSCONum(Project p, int scoNum, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    p.WriteSync();
                    SqlTransaction trans = conn.BeginTransaction();

                    p.CurSCONum = scoNum;

                    string sql = "UPDATE Projects SET CurSCONum=@CurSCONum, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                                "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", p.Id);
                        cmd.Parameters.AddWithValue("CurSCONum", p.CurSCONum);
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated SCO Number {2} for Project {0}{1:0000}",
                                p.PaymentCode, p.Id, p.CurSCONum),
                            TableName = "Projects",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating project '{0}' with ScoNum {2}: {1}", p.ProjectCode, ex.Message, scoNum);
                return false;
            }
        }

        public bool DeleteProject(int projId, string userId)
        {
            string sql;
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    // delete sco numbers assco with project
                    sql = "DELETE FROM ScoNumbers WHERE ProjectId=@ProjectId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ProjectId", projId);

                        cmd.ExecuteNonQuery();
                    }

                    sql = "DELETE FROM Projects WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", projId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted Project {0:0000}", projId),
                            TableName = "Projects",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting project '{0}': {1}", projId, ex.Message);
                return false;
            }
        }

        public bool HasProjectId(string projId)
        {
            bool status = true;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Projects WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", projId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cnt = reader.SafeGetInt(0);
                            status = (cnt > 0);
                        }
                    }
                }

                return status;
            }
        }
        #endregion
        #region Suppliers
        //*********************************************************************
        // Suppliers
        //*********************************************************************

        public bool GetSuppliers(List<Supplier> supplierList, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            supplierList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                sql = string.Format("SELECT Id, Name, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Suppliers ORDER BY Id {0};", sqlseg);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Supplier s = new Supplier();

                            s.Id = reader.SafeGetString(0);
                            s.Name = reader.SafeGetString(1);
                            s.CreateUser = reader.SafeGetString(2);
                            s.CreateDate = reader.SafeGetDateTime(3);
                            s.LastUpdateUser = reader.SafeGetString(4);
                            s.LastUpdate = reader.SafeGetDateTime(5);

                            supplierList.Add(s);
                        }
                    }
                }

                sql = "SELECT COUNT(*) FROM Suppliers;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public bool GetSuppliersForProject(int projectId, List<Supplier> supplierList)
        {
            string sql;
            bool status = false;
            supplierList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT s.Id, s.Name, s.CreateUser, s.CreateDate, s.LastUpdateUser, s.LastUpdate " +
                    "FROM CubeSets cs, Suppliers s " +
                    "WHERE cs.SupplierId=s.Id AND cs.ProjectId=@ProjectId " +
                    "GROUP BY s.Id, s.Name, s.CreateUser, s.CreateDate, s.LastUpdateUser, s.LastUpdate;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Supplier s = new Supplier();

                            s.Id = reader.SafeGetString(0);
                            s.Name = reader.SafeGetString(1);
                            s.CreateUser = reader.SafeGetString(2);
                            s.CreateDate = reader.SafeGetDateTime(3);
                            s.LastUpdateUser = reader.SafeGetString(4);
                            s.LastUpdate = reader.SafeGetDateTime(5);

                            supplierList.Add(s);
                        }
                        status = true;
                    }
                }

                return status;
            }
        }

        public Supplier GetSupplier(string supplierId)
        {
            Supplier s = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, Name, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Suppliers WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", supplierId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            s = new Supplier();

                            s.Id = reader.SafeGetString(0);
                            s.Name = reader.SafeGetString(1);
                            s.CreateUser = reader.SafeGetString(2);
                            s.CreateDate = reader.SafeGetDateTime(3);
                            s.LastUpdateUser = reader.SafeGetString(4);
                            s.LastUpdate = reader.SafeGetDateTime(5);
                        }
                    }
                }

                return s;
            }
        }

        public bool InsertSupplier(Supplier s, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    s.CreateUser = userId;
                    s.CreateDate = DateTime.Now;

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "INSERT INTO Suppliers (Id, Name, " +
                                 "CreateUser, CreateDate, LastUpdateUser) " +
                                 "VALUES (@Id, @Name, " +
                                 "@CreateUser, @CreateDate, @LastUpdateUser);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        //cmd.Parameters.AddWithValue("Id", p.Id);
                        cmd.Parameters.AddWithValue("Id", s.Id);
                        cmd.Parameters.AddWithValue("Name", s.Name);
                        cmd.Parameters.AddWithValue("CreateUser", s.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", s.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added Supplier {0}", s.Id),
                            TableName = "Suppliers",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting supplier '{0}': {1}", s.Id, ex.Message);
                return false;
            }
        }

        public bool UpdateSupplier(Supplier s, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Suppliers SET Name=@Name, " +
                                 "CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, " +
                                 "LastUpdate=@LastUpdate " +
                                 "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", s.Id);
                        cmd.Parameters.AddWithValue("Name", s.Name);
                        cmd.Parameters.AddWithValue("CreateUser", s.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", s.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Supplier {0}", s.Id),
                            TableName = "Suppliers",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating supplier '{0}': {1}", s.Id, ex.Message);
                return false;
            }
        }

        public bool DeleteSupplier(string supplierId, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Suppliers WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", supplierId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted Supplier {0}", supplierId),
                            TableName = "Suppliers",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting supplier '{0}': {1}", supplierId, ex.Message);
                return false;
            }
        }

        public bool HasSupplierId(string supplierId)
        {
            bool status = true;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Suppliers WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", supplierId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cnt = reader.SafeGetInt(0);
                            status = (cnt > 0);
                        }
                    }
                }

                return status;
            }
        }
        #endregion
        #region CubeSets
        //*********************************************************************
        // Cube Sets
        //*********************************************************************

        public bool GetCubeSets(List<CubeSet> cubeSets, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                sql = string.Format("SELECT Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM CubeSets ORDER BY Id {0};", sqlseg);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }
                    }
                }

                sql = "SELECT COUNT(*) FROM CubeSets;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                foreach(CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        

        public bool GetCubeSetsForProject(int projectId, List<CubeSet> cubeSets, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                string sqlwhere = "";
                if (projectId > 0)
                {
                    sqlwhere = "WHERE ProjectId=@ProjectId";
                }

                sql = string.Format("SELECT Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM CubeSets {1} ORDER BY Id {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM CubeSets {0};", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                foreach (CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        public bool GetTestedCubeSetsForProject(int projectId, List<CubeSet> cubeSets, DateTime dtStart, DateTime dtEnd)
        {
            string sql;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT CS.Id, CS.ProjectId, CS.SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CS.CreateUser, CS.CreateDate, CS.LastUpdateUser, CS.LastUpdate " +
                    "FROM Cubes C, Batches B, CubeSets CS, Projects P " +
                    "WHERE C.ScoNum=C.ScoNum AND C.BatchId=B.Id AND B.CubeSetId=CS.Id AND " +
                    "CS.ProjectId=P.Id AND P.Id=@ProjectId AND C.TestResult>0 AND " +
                    "CAST(C.ActualTestDate AS DATE)>=CAST(@StartDate AS DATE) AND " +
                    "CAST(C.ActualTestDate AS DATE)<=CAST(@EndDate AS DATE) " +
                    "GROUP BY CS.Id, CS.ProjectId, CS.SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CS.CreateUser, CS.CreateDate, CS.LastUpdateUser, CS.LastUpdate " +
                    "ORDER BY CS.Id;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("StartDate", dtStart);
                    cmd.Parameters.AddWithValue("EndDate", dtEnd);
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }

                        status = true;
                    }
                }

                foreach (CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        public bool GetCubeSetsForCalc(List<CubeSet> cubeSets)
        {
            string sql;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get cubesets with uncalculated batches 
                sql = "SELECT cs.Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, cs.CreateUser, cs.CreateDate, cs.LastUpdateUser, cs.LastUpdate " +
                    "FROM Cubesets cs, Batches b WHERE cs.Id=b.CubeSetId AND " +
                    "b.RollingAvgStrength=0 AND CAST(TargetTestDate AS DATE)<=CAST(@Today AS DATE)" +
                    "GROUP BY cs.Id, ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, " +
                    "CharacteristicStrength, StdDeviation, SupplierId, Location, CastingDate, " +
                    "cs.CreateUser, cs.CreateDate, cs.LastUpdateUser, cs.LastUpdate;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Today", DateTime.Now);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }

                        status = true;
                    }
                }

                foreach (CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        public bool GetCubeSetsForProjectSupplier(int projectId, string supplierId, List<CubeSet> cubeSets, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            total = 0;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("LIMIT {0}, {1}", offset, rows);
                }
                string sqlwhere = "";
                if (projectId > 0 && supplierId.Length > 0)
                {
                    sqlwhere = "WHERE ProjectId=@ProjectId AND CubeSets.SupplierId=@SupplierId";
                }

                sql = string.Format("SELECT Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM CubeSets {1} ORDER BY SpecId, Id {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);
                    cmd.Parameters.AddWithValue("SupplierId", supplierId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM CubeSets {0}", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);
                    cmd.Parameters.AddWithValue("SupplierId", supplierId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                foreach (CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        public bool GetCubeSetsForProjectSupplierTestSpec(int projectId, string supplierId, string specId, List<CubeSet> cubeSets)
        {
            string sql;
            bool status = false;
            cubeSets.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";
                if (projectId > 0 && supplierId.Length > 0)
                {
                    sqlwhere = "WHERE ProjectId=@ProjectId AND CubeSets.SupplierId=@SupplierId AND SpecId=@SpecId";
                }

                sql = string.Format("SELECT Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM CubeSets {1} ORDER BY ConcreteGrade, Id {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);
                    cmd.Parameters.AddWithValue("SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("SpecId", specId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CubeSet cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);

                            cubeSets.Add(cs);
                        }

                        status = true;
                    }
                }

                foreach (CubeSet cs in cubeSets)
                {
                    cs.ProjectCode = Global.db.GetProjectCode(cs.ProjectId);
                }

                return status;
            }
        }

        public CubeSet GetCubeSet(int cubeSetId)
        {
            CubeSet cs = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, ProjectId, SpecId, " +
                    "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                    "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM CubeSets WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", cubeSetId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cs = new CubeSet();

                            cs.Id = reader.SafeGetInt(0);
                            cs.ProjectId = reader.SafeGetInt(1);
                            cs.SpecId = reader.SafeGetString(2);
                            cs.TestCriteria = reader.SafeGetString(3);
                            cs.ConcreteGrade = reader.SafeGetInt(4);
                            cs.ConcreteType = reader.SafeGetString(5);
                            cs.CharacteristicStrength = reader.GetDouble(6);
                            cs.StdDeviation = reader.GetDouble(7);
                            cs.SupplierId = reader.SafeGetString(8);
                            cs.Location = reader.SafeGetString(9);
                            cs.CastingDate = reader.SafeGetDateTime(10);
                            cs.CreateUser = reader.SafeGetString(11);
                            cs.CreateDate = reader.SafeGetDateTime(12);
                            cs.LastUpdateUser = reader.SafeGetString(13);
                            cs.LastUpdate = reader.SafeGetDateTime(14);
                        }
                    }
                }

                if (cs != null)
                {
                    cs.ProjectCode = GetProjectCode(cs.ProjectId);
                }

                return cs;
            }
        }

        public bool InsertCubeSet(CubeSet cs, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    cs.CreateUser = userId;
                    cs.CreateDate = DateTime.Now;

                    string sql = "INSERT INTO CubeSets (ProjectId, SpecId, " +
                        "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                        "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser) " +
                        "VALUES (@ProjectId, @SpecId, " +
                        "@TestCriteria, @ConcreteGrade, @ConcreteType, @CharacteristicStrength, @StdDeviation, " +
                        "@SupplierId, @Location, @CastingDate, @CreateUser, @CreateDate, @LastUpdateUser);" +
                        "SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ProjectId", cs.ProjectId);
                        cmd.Parameters.AddWithValue("SpecId", cs.SpecId);
                        cmd.Parameters.AddWithValue("TestCriteria", cs.TestCriteria);
                        cmd.Parameters.AddWithValue("ConcreteGrade", cs.ConcreteGrade);
                        cmd.Parameters.AddWithValue("ConcreteType", cs.ConcreteType);
                        cmd.Parameters.AddWithValue("CharacteristicStrength", cs.CharacteristicStrength);
                        cmd.Parameters.AddWithValue("StdDeviation", cs.StdDeviation);
                        cmd.Parameters.AddWithValue("SupplierId", cs.SupplierId);
                        cmd.Parameters.AddWithValue("Location", cs.Location);
                        cmd.Parameters.AddWithValue("CastingDate", cs.CastingDate);
                        cmd.Parameters.AddWithValue("CreateUser", cs.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", cs.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cs.Id = reader.SafeGetDecimal(0);
                            }
                        }

                        status = cs.Id > 0;
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added CubeSet {0}", cs.Id),
                            TableName = "CubeSets",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting CubeSet '{0}': {1}", cs.Id, ex.Message);
                return false;
            }
        }

        public bool UpdateCubeSet(CubeSet cs, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE CubeSets SET ProjectId=@ProjectId, SpecId=@SpecId, " +
                        "TestCriteria=@TestCriteria, ConcreteGrade=@ConcreteGrade, ConcreteType=@ConcreteType, " +
                        "CharacteristicStrength=@CharacteristicStrength, StdDeviation=@StdDeviation, " +
                        "SupplierId=@SupplierId, Location=@Location, CastingDate=@CastingDate, " +
                        "CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, " +
                        "LastUpdate=@LastUpdate " +
                        "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", cs.Id);
                        cmd.Parameters.AddWithValue("ProjectId", cs.ProjectId);
                        cmd.Parameters.AddWithValue("SpecId", cs.SpecId);
                        cmd.Parameters.AddWithValue("TestCriteria", cs.TestCriteria);
                        cmd.Parameters.AddWithValue("ConcreteGrade", cs.ConcreteGrade);
                        cmd.Parameters.AddWithValue("ConcreteType", cs.ConcreteType);
                        cmd.Parameters.AddWithValue("CharacteristicStrength", cs.CharacteristicStrength);
                        cmd.Parameters.AddWithValue("StdDeviation", cs.StdDeviation);
                        cmd.Parameters.AddWithValue("SupplierId", cs.SupplierId);
                        cmd.Parameters.AddWithValue("Location", cs.Location);
                        cmd.Parameters.AddWithValue("CastingDate", cs.CastingDate);
                        cmd.Parameters.AddWithValue("CreateUser", cs.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", cs.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated CubeSet {0}", cs.Id),
                            TableName = "CubeSets",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating CubeSet '{0}': {1}", cs.Id, ex.Message);
                return false;
            }
        }

        public bool DeleteCubeSet(int cubeSetId, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM CubeSets WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", cubeSetId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted CubeSet {0}", cubeSetId),
                            TableName = "CubeSets",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting CubeSet '{0}': {1}", cubeSetId, ex.Message);
                return false;
            }
        }
        #endregion
        #region Barcode Allocations
        public bool GetBarcodeAllocations(int projectId, List<BarcodeAllocation> baList, out int total, int offset = 0, int rows = 0)
        {
            string sql;
            bool status = false;
            baList.Clear();
            total = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }

                string sqlwhere = "";
                if (projectId > 0)
                {
                    sqlwhere = "WHERE ProjectId=@ProjectId";
                }

                sql = string.Format("SELECT Id, ProjectId, Qty, BarcodeStart, BarcodeEnd, LastUpdateUser, LastUpdate " +
                    "FROM BarcodeAllocation {1} ORDER BY Id {0} DESC;", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BarcodeAllocation ba = new BarcodeAllocation();

                            ba.Id = reader.SafeGetInt(0);
                            ba.ProjectId = reader.SafeGetInt(1);
                            ba.Qty = reader.SafeGetInt(2);
                            ba.BarcodeStart = reader.SafeGetInt(3);
                            ba.BarcodeEnd = reader.SafeGetInt(4);
                            ba.LastUpdateUser = reader.SafeGetString(5);
                            ba.LastUpdate = reader.SafeGetDateTime(6);

                            baList.Add(ba);
                        }
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM BarcodeAllocation {0};", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public BarcodeAllocation GetBarcodeAllocation(int baId)
        {
            BarcodeAllocation ba = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, ProjectId, Qty, BarcodeStart, BarcodeEnd, LastUpdateUser, LastUpdate " +
                    "FROM BarcodeAllocation WHERE Id=@Id;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", baId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ba = new BarcodeAllocation();

                            ba.Id = reader.SafeGetInt(0);
                            ba.ProjectId = reader.SafeGetInt(1);
                            ba.Qty = reader.SafeGetInt(2);
                            ba.BarcodeStart = reader.SafeGetInt(3);
                            ba.BarcodeEnd = reader.SafeGetInt(4);
                            ba.LastUpdateUser = reader.SafeGetString(5);
                            ba.LastUpdate = reader.SafeGetDateTime(6);
                        }
                    }
                }

                return ba;
            }
        }

        public bool InsertBarcodeAllocation(int projectId, BarcodeAllocation ba, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    ba.Id = 0;
                    ba.ProjectId = projectId;
                    ba.BarcodeEnd = ba.BarcodeStart + ba.Qty - 1;
                    ba.LastUpdateUser = userId;

                    bool valid = BarcodeAllocationValid(ba);
                    if (!valid)
                    {
                        trans.Rollback();
                        return false;
                    }

                    string sql = "INSERT INTO BarcodeAllocation " +
                        "(ProjectId, Qty, BarcodeStart, BarcodeEnd, LastUpdateUser) " +
                        "VALUES (@ProjectId, @Qty, @BarcodeStart, @BarcodeEnd, @LastUpdateUser);" +
                        "SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ProjectId", ba.ProjectId);
                        cmd.Parameters.AddWithValue("Qty", ba.Qty);
                        cmd.Parameters.AddWithValue("BarcodeStart", ba.BarcodeStart);
                        cmd.Parameters.AddWithValue("BarcodeEnd", ba.BarcodeEnd);
                        cmd.Parameters.AddWithValue("LastUpdateUser", ba.LastUpdateUser);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ba.Id = reader.SafeGetDecimal(0);
                            }

                            status = ba.Id > 0;
                        }
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added BarcodeAllocation {0}", ba.Id),
                            TableName = "BarcodeAllocations",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting Barcode Allocation '{0}': {1}", ba.Id, ex.Message);
                return false;
            }
        }

        public bool BarcodeAllocationValid(BarcodeAllocation ba)
        {
            bool valid = false;

            if (ba.BarcodeStart <= 0 || ba.Qty <= 0) return false;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                ba.BarcodeEnd = ba.BarcodeStart + ba.Qty - 1;

                string sql = "SELECT COUNT(*) FROM BarcodeAllocation " +
                    "WHERE Id<>@Id AND " +
                    "(@BarcodeStart>=BarcodeStart AND @BarcodeStart<=BarcodeEnd OR " +
                    "@BarcodeEnd>=BarcodeStart AND @BarcodeEnd<=BarcodeEnd);";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", ba.Id);
                    cmd.Parameters.AddWithValue("BarcodeStart", ba.BarcodeStart);
                    cmd.Parameters.AddWithValue("BarcodeEnd", ba.BarcodeEnd);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int count = reader.SafeGetInt(0);
                            valid = (count == 0);
                        }
                    }
                }

                return valid;
            }
        }

        public bool UpdateBarcodeAllocation(BarcodeAllocation ba, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    bool valid = BarcodeAllocationValid(ba);
                    if (!valid) return false;

                    string sql = "UPDATE BarcodeAllocation SET ProjectId=@ProjectId, Qty=@Qty, BarcodeStart=@BarcodeStart, " +
                        "BarcodeEnd=@BarcodeEnd, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                        "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", ba.Id);
                        cmd.Parameters.AddWithValue("ProjectId", ba.ProjectId);
                        cmd.Parameters.AddWithValue("Qty", ba.Qty);
                        cmd.Parameters.AddWithValue("BarcodeStart", ba.BarcodeStart);
                        cmd.Parameters.AddWithValue("BarcodeEnd", ba.BarcodeEnd);
                        cmd.Parameters.AddWithValue("LastUpdateUser", ba.LastUpdateUser);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Barcode Allocation {0}", ba.Id),
                            TableName = "BarcodeAllocations",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating Barcode Allocation '{0}': {1}", ba.Id, ex.Message);
                return false;
            }
        }

        public bool DeleteBarcodeAllocation(int baId, string userId)
        {
            string sql;
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    int cnt = 0;

                    // check barcode has been allocated within range
                    sql = "SELECT COUNT(*) FROM Cubes, BarcodeAllocation " +
                        "WHERE Cubes.Barcode>=BarcodeAllocation.BarcodeStart AND Cubes.Barcode<=BarcodeAllocation.BarcodeEnd AND " +
                        "BarcodeAllocation.Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", baId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cnt = reader.GetInt32(0);
                            }
                        }
                    }

                    if (cnt == 0)
                    {
                        sql = "DELETE FROM BarcodeAllocation WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Id", baId);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }

                        if (status)
                        {
                            ActivityLog log = new ActivityLog()
                            {
                                Activity = string.Format("Deleted Barcode Allocation {0}", baId),
                                TableName = "BarcodeAllocation",
                                UserId = userId
                            };

                            status = LogActivity(log, conn, trans);
                        }
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting Barcode Allocation '{0}': {1}", baId, ex.Message);
                return false;
            }
        }

        #endregion

        #region Batches
        //*********************************************************************
        // Batches
        //*********************************************************************
        public bool GetBatches(List<Batch> batches, out int total, int offset = 0, int rows = 0, DateTime? lastUpdate=null)
        {
            string sql;
            total = 0;
            bool status = false;
            batches.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }
                if (lastUpdate != null)
                {
                    sqlwhere = string.Format("WHERE LastUpdate>@LastUpdate");
                }

                sql = string.Format("SELECT ScoNum, Id, CubeSetId, TestAge, " +
                    "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Batches {1} ORDER BY Id {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("LastUpdate", lastUpdate.SafeDateTime());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Batch b = new Batch();

                            b.ScoNum = reader.SafeGetInt(0);
                            b.Id = reader.SafeGetInt(1);
                            b.CubeSetId = reader.SafeGetInt(2);
                            b.TestAge = reader.SafeGetInt(3);
                            b.TargetTestDate = reader.SafeGetDateTime(4);
                            b.Dimension = reader.SafeGetDouble(5);
                            b.WitnessNum = reader.SafeGetInt(6);
                            b.AvgStrength = reader.SafeGetDouble(7);
                            b.RollingAvgStrength = reader.SafeGetDouble(8);
                            b.CriterionA = reader.SafeGetChar(9);
                            b.CriterionB = reader.SafeGetChar(10);
                            b.CreateUser = reader.SafeGetString(11);
                            b.CreateDate = reader.SafeGetDateTime(12);
                            b.LastUpdateUser = reader.SafeGetString(13);
                            b.LastUpdate = reader.SafeGetDateTime(14);

                            b.BatchNum = $"{b.ScoNum}-{b.Id}";

                            batches.Add(b);
                        }
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM Batches {0};", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        public bool GetCubesForTest(List<Cube> cubes, CubeQueryOption option, DateTime? lastUpdate = null)
        {
            bool status;
            int totalCubes;
            List<Batch> batches = new List<Batch>();
            List<CubeSet> cubeSets = new List<CubeSet>();
            cubes.Clear();

            status = Global.db.GetBatchesForTest(batches, lastUpdate);
            if (!status) return false;

            foreach (Batch b in batches)
            {
                // Get cube sets
                CubeSet cs = Global.db.GetCubeSet(b.CubeSetId);
                if (cs != null)
                {
                    cubeSets.Add(cs);
                }

                // get cubes
                List<Cube> cl = new List<Cube>();
                status = Global.db.GetCubes(cl, b.ScoNum, b.Id, CubeViewOption.All, out totalCubes, 0, 0, lastUpdate);
                if (status)
                {
                    cubes.AddRange(cl);
                }
            }

            return status;
        }

        public bool GetBatchesForToday(List<Batch> batches, CubeViewOption option)
        {
            string sql;
            bool status = false;
            batches.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sqlwhere = "";
                    if (option == CubeViewOption.Untested) sqlwhere += "AND C.TestResult=0";
                    else if (option == CubeViewOption.Tested) sqlwhere += "AND C.TestResult>0";

                    sql = string.Format("SELECT B.ScoNum, B.Id, CubeSetId, TestAge, " +
                        "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                        "B.CreateUser, B.CreateDate, B.LastUpdateUser, B.LastUpdate " +
                        "FROM Batches B, Cubes C " +
                        "WHERE CAST(TargetTestDate AS DATE)<=CAST(@TargetTestDate AS DATE) {0} AND " +
                        "B.ScoNum=C.ScoNum AND B.Id=C.BatchId " +
                        "GROUP BY B.ScoNum, B.Id, CubeSetId, TestAge, " +
                        "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                        "B.CreateUser, B.CreateDate, B.LastUpdateUser, B.LastUpdate;", sqlwhere);

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("TargetTestDate", DateTime.Now);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Batch b = new Batch();

                                b.ScoNum = reader.SafeGetInt(0);
                                b.Id = reader.SafeGetInt(1);
                                b.CubeSetId = reader.SafeGetInt(2);
                                b.TestAge = reader.SafeGetInt(3);
                                b.TargetTestDate = reader.SafeGetDateTime(4);
                                b.Dimension = reader.SafeGetDouble(5);
                                b.WitnessNum = reader.SafeGetInt(6);
                                b.AvgStrength = reader.SafeGetDouble(7);
                                b.RollingAvgStrength = reader.SafeGetDouble(8);
                                b.CriterionA = reader.SafeGetChar(9);
                                b.CriterionB = reader.SafeGetChar(10);
                                b.CreateUser = reader.SafeGetString(11);
                                b.CreateDate = reader.SafeGetDateTime(12);
                                b.LastUpdateUser = reader.SafeGetString(13);
                                b.LastUpdate = reader.SafeGetDateTime(14);

                                b.BatchNum = $"{b.ScoNum}-{b.Id}";

                                batches.Add(b);
                            }

                            status = true;
                        }
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error running GetBatchesPartiallyTestedToday(): {0}", ex.Message);
                return false;
            }
        }

        public bool GetBatchesForTest(List<Batch> batches, DateTime? lastUpdate = null)
        {
            string sql;
            bool status = false;
            batches.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sqlwhere = "";
                    if (lastUpdate != null && lastUpdate != DateTime.MinValue)
                    {
                        sqlwhere = string.Format("AND (Batches.LastUpdate>@LastUpdate OR Cubes.LastUpdate>@LastUpdate)");
                    }

                    sql = string.Format("SELECT Batches.ScoNum, Batches.Id, CubeSetId, TestAge, " +
                        "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                        "Batches.CreateUser, Batches.CreateDate, Batches.LastUpdateUser, Batches.LastUpdate " +
                        "FROM Batches,Cubes " +
                        "WHERE (CAST(TargetTestDate AS DATE)=CAST(@TargetTestDate AS DATE) OR " +
                        "CAST(TargetTestDate AS DATE)<CAST(@TargetTestDate AS DATE) AND Cubes.TestResult=0) AND " +
                        "Batches.ScoNum=Cubes.ScoNum AND Batches.Id=Cubes.BatchId {0} " +
                        "GROUP BY Batches.ScoNum, Batches.Id, CubeSetId, TestAge, " +
                        "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                        "Batches.CreateUser, Batches.CreateDate, Batches.LastUpdateUser, Batches.LastUpdate;", sqlwhere);

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("LastUpdate", lastUpdate.SafeDateTime());
                        cmd.Parameters.AddWithValue("TargetTestDate", DateTime.Now);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Batch b = new Batch();

                                b.ScoNum = reader.SafeGetInt(0);
                                b.Id = reader.SafeGetInt(1);
                                b.CubeSetId = reader.SafeGetInt(2);
                                b.TestAge = reader.SafeGetInt(3);
                                b.TargetTestDate = reader.SafeGetDateTime(4);
                                b.Dimension = reader.SafeGetDouble(5);
                                b.WitnessNum = reader.SafeGetInt(6);
                                b.AvgStrength = reader.SafeGetDouble(7);
                                b.RollingAvgStrength = reader.SafeGetDouble(8);
                                b.CriterionA = reader.SafeGetChar(9);
                                b.CriterionB = reader.SafeGetChar(10);
                                b.CreateUser = reader.SafeGetString(11);
                                b.CreateDate = reader.SafeGetDateTime(12);
                                b.LastUpdateUser = reader.SafeGetString(13);
                                b.LastUpdate = reader.SafeGetDateTime(14);

                                b.BatchNum = $"{b.ScoNum}-{b.Id}";

                                batches.Add(b);
                            }

                            status = true;
                        }
                    }

                    return status;
                }
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error running GetBatchesForTest(): {0}", ex.Message);
                return false;
            }
        }

        public bool GetBatchesForCalc(List<Batch> batches)
        {
            string sql;
            bool status = false;
            batches.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT ScoNum, Id, CubeSetId, TestAge, " +
                    "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                    "b.CreateUser, b.CreateDate, b.LastUpdateUser, b.LastUpdate " +
                    "FROM Batches AS b, Cubes WHERE Cubes.ScoNum=Batches.ScoNum AND Cubes.BatchId=Batches.Id AND " +
                    "Cubes.TestResult>0 AND Batches.AvgStrength=0;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Batch b = new Batch();

                            b.ScoNum = reader.SafeGetInt(0);
                            b.Id = reader.SafeGetInt(1);
                            b.CubeSetId = reader.SafeGetInt(2);
                            b.TestAge = reader.SafeGetInt(3);
                            b.TargetTestDate = reader.SafeGetDateTime(4);
                            b.Dimension = reader.GetDouble(5);
                            b.WitnessNum = reader.SafeGetInt(6);
                            b.AvgStrength = reader.SafeGetDouble(7);
                            b.RollingAvgStrength = reader.SafeGetDouble(8);
                            b.CriterionA = reader.SafeGetChar(9);
                            b.CriterionB = reader.SafeGetChar(10);
                            b.CreateUser = reader.SafeGetString(11);
                            b.CreateDate = reader.SafeGetDateTime(12);
                            b.LastUpdateUser = reader.SafeGetString(13);
                            b.LastUpdate = reader.SafeGetDateTime(14);

                            b.BatchNum = $"{b.ScoNum}-{b.Id}";

                            batches.Add(b);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }


        public bool GetBatchesForCubeSet(int cubeSetId, List<Batch> batches, int testAge=0)
        {
            string sql;
            bool status = false;
            batches.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "ORDER BY ScoNum, Id";
                string sqlwhere = "";
                if (cubeSetId > 0)
                {
                    sqlwhere = string.Format("WHERE CubeSetId=@CubeSetId");
                    if (testAge > 0)
                    {
                        sqlwhere += " AND TestAge=@TestAge";
                    }
                }

                sql = string.Format("SELECT ScoNum, Id, CubeSetId, TestAge, " +
                    "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Batches {1} {0};", sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("CubeSetId", cubeSetId);
                    cmd.Parameters.AddWithValue("TestAge", testAge);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Batch b = new Batch();

                            b.ScoNum = reader.SafeGetInt(0);
                            b.Id = reader.SafeGetInt(1);
                            b.CubeSetId = reader.SafeGetInt(2);
                            b.TestAge = reader.SafeGetInt(3);
                            b.TargetTestDate = reader.SafeGetDateTime(4);
                            b.Dimension = reader.SafeGetDouble(5);
                            b.WitnessNum = reader.SafeGetInt(6);
                            b.AvgStrength = reader.SafeGetDouble(7);
                            b.RollingAvgStrength = reader.SafeGetDouble(8);
                            b.CriterionA = reader.SafeGetChar(9);
                            b.CriterionB = reader.SafeGetChar(10);
                            b.CreateUser = reader.SafeGetString(11);
                            b.CreateDate = reader.SafeGetDateTime(12);
                            b.LastUpdateUser = reader.SafeGetString(13);
                            b.LastUpdate = reader.SafeGetDateTime(14);

                            b.BatchNum = $"{b.ScoNum}-{b.Id}";

                            batches.Add(b);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }

        public Batch GetBatch(int sco, int batchId)
        {
            Batch b = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT ScoNum, Id, CubeSetId, TestAge, " +
                    "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Batches WHERE Id=@Id AND ScoNum=@ScoNum;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", batchId);
                    cmd.Parameters.AddWithValue("ScoNum", sco);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            b = new Batch();

                            b.ScoNum = reader.SafeGetInt(0);
                            b.Id = reader.SafeGetInt(1);
                            b.CubeSetId = reader.SafeGetInt(2);
                            b.TestAge = reader.SafeGetInt(3);
                            b.TargetTestDate = reader.SafeGetDateTime(4);
                            b.Dimension = reader.SafeGetDouble(5);
                            b.WitnessNum = reader.SafeGetInt(6);
                            b.AvgStrength = reader.SafeGetDouble(7);
                            b.RollingAvgStrength = reader.SafeGetDouble(8);
                            b.CriterionA = reader.SafeGetChar(9);
                            b.CriterionB = reader.SafeGetChar(10);
                            b.CreateUser = reader.SafeGetString(11);
                            b.CreateDate = reader.SafeGetDateTime(12);
                            b.LastUpdateUser = reader.SafeGetString(13);
                            b.LastUpdate = reader.SafeGetDateTime(14);

                            b.BatchNum = $"{b.ScoNum}-{b.Id}";
                        }
                    }
                }

                return b;
            }
        }

        public int GetNextWitnessNum(DateTime testDate)
        {
            int witnessNum = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT MAX(WitnessNum) FROM Batches " +
                    "WHERE TargetTestDate=@TargetTestDate;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("TargetTestDate", testDate);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            witnessNum = reader.GetInt32(0);   
                        }
                    }
                }

                return witnessNum+1;
            }
        }

        public int GetScoNumFromCubeSetId(int cubeSetId, SqlTransaction trans=null)
        {
            int sco = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Projects.CurScoNum FROM " +
                            "Projects, CubeSets WHERE CubeSets.ProjectId=Projects.Id AND CubeSets.Id=@CubeSetId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("CubeSetId", cubeSetId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sco = reader.SafeGetInt(0);
                        }
                    }
                }
            }

            return sco;
        }

        public int GetMaxBatchNum(int scoNum, SqlTransaction trans = null)
        {
            int max = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*), MAX(Id) " +
                            "FROM Batches " +
                            "WHERE ScoNum=@ScoNum;";
                using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cnt = reader.SafeGetInt(0);
                            if (cnt > 0)
                            {
                                max = reader.SafeGetInt(1);   
                            }
                        }
                    }
                }
            }

            return max;
        }

        public (int, int) CreateBatchNum(int cubeSetId)
        {
            int sco = GetScoNumFromCubeSetId(cubeSetId);
            int max = GetMaxBatchNum(sco);

            return (sco, max + 1);
        }

        public bool InsertBatch(Batch b, string userId)
        {
            int sco, id;
            bool status = false;

            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    b.CreateUser = userId;
                    b.CreateDate = DateTime.Now;

                    (sco, id) = CreateBatchNum(b.CubeSetId);

                    string sql = "INSERT INTO Batches (ScoNum, Id, CubeSetId, TestAge, " +
                        "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                        "CreateUser, CreateDate, LastUpdateUser) " +
                        "VALUES (@ScoNum, @Id, @CubeSetId, @TestAge, " +
                        "@TargetTestDate, @Dimension, @WitnessNum, @AvgStrength, @RollingAvgStrength, @CriterionA, @CriterionB, " +
                        "@CreateUser, @CreateDate, @LastUpdateUser);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                        cmd.Parameters.AddWithValue("Id", b.Id);
                        cmd.Parameters.AddWithValue("CubeSetId", b.CubeSetId);
                        cmd.Parameters.AddWithValue("TestAge", b.TestAge);
                        cmd.Parameters.AddWithValue("TargetTestDate", b.TargetTestDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("Dimension", b.Dimension);
                        cmd.Parameters.AddWithValue("WitnessNum", b.WitnessNum);
                        cmd.Parameters.AddWithValue("AvgStrength", b.AvgStrength);
                        cmd.Parameters.AddWithValue("RollingAvgStrength", b.RollingAvgStrength);
                        cmd.Parameters.AddWithValue("CriterionA", b.CriterionA);
                        cmd.Parameters.AddWithValue("CriterionB", b.CriterionB);
                        cmd.Parameters.AddWithValue("CreateUser", b.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", b.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added Batch {0}", b.Id),
                            TableName = "Batches",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting Batch {0}-{1}: {2}", b.ScoNum, b.Id, ex.Message);
                return false;
            }
        }

        public bool UpdateBatch(Batch b, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Batches SET CubeSetId=@CubeSetId, " +
                        "TestAge=@TestAge, TargetTestDate=@TargetTestDate, Dimension=@Dimension, " +
                        "WitnessNum=@WitnessNum, CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, " +
                        "LastUpdate=@LastUpdate " +
                        "WHERE ScoNum=@ScoNum AND Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                        cmd.Parameters.AddWithValue("Id", b.Id);
                        cmd.Parameters.AddWithValue("CubeSetId", b.CubeSetId);
                        cmd.Parameters.AddWithValue("TestAge", b.TestAge);
                        cmd.Parameters.AddWithValue("TargetTestDate", b.TargetTestDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("Dimension", b.Dimension);
                        cmd.Parameters.AddWithValue("WitnessNum", b.WitnessNum);
                        cmd.Parameters.AddWithValue("CreateUser", b.CreateUser);
                        cmd.Parameters.AddWithValue("CreateDate", b.CreateDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Batch {0}-{1}", b.ScoNum, b.Id),
                            TableName = "Batches",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating Batch {0}-{1}: {2}", b.ScoNum, b.Id, ex.Message);
                return false;
            }
        }

        public bool UpdateAvgStrengthForBatch(Batch b, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Batches SET AvgStrength=@AvgStrength, RollingAvgStrength=@RollingAvgStrength, " +
                        "CriterionA=@CriterionA, CriterionB=@CriterionB, LastUpdateUser=@LastUpdateUser, " +
                        "LastUpdate=@LastUpdate " +
                        "WHERE ScoNum=@ScoNum AND Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                        cmd.Parameters.AddWithValue("Id", b.Id);
                        cmd.Parameters.AddWithValue("AvgStrength", b.AvgStrength);
                        cmd.Parameters.AddWithValue("RollingAvgStrength", b.RollingAvgStrength);
                        cmd.Parameters.AddWithValue("CriterionA", b.CriterionA);
                        cmd.Parameters.AddWithValue("CriterionB", b.CriterionB);
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Batch {0}-{1}", b.ScoNum, b.Id),
                            TableName = "Batches",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating avg strength for Batch {0}-{1}: {2}", b.ScoNum, b.Id, ex.Message);
                return false;
            }
        }

        public bool DeleteBatch(string batchNum, string userId)
        {
            bool status = false;

            string[] s = batchNum.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length != 2) return false;

            int sco, batchId;
            if (!int.TryParse(s[0], out sco) || !int.TryParse(s[1], out batchId))
            {
                return false;
            }

            try
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Batches WHERE ScoNum=@ScoNum AND Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ScoNum", sco);
                        cmd.Parameters.AddWithValue("Id", batchId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted batch {0}", batchNum),
                            TableName = "Batches",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting Batch {0}: {1}", batchNum, ex.Message);
                return false;
            }
        }

        #endregion

        #region Cubes
        //*********************************************************************
        // Cubes
        //*********************************************************************

        public bool GetCubes(List<Cube> cubeList, int scoNum, int batchId, CubeViewOption viewOption, out int total, 
            int offset = 0, int rows = 0, DateTime? lastUpdate=null)
        {
            string sql;
            total = 0;
            bool status = false;
            cubeList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";
                if (rows > 0 && offset >= 0)
                {
                    sqlseg = string.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, rows);
                }

                if (scoNum == 0 || batchId == 0)
                {
                    if (lastUpdate != null && lastUpdate != DateTime.MinValue)
                    {
                        sqlwhere = "WHERE LastUpdate>@LastUpdate";
                    }

                    if (viewOption == CubeViewOption.Untested)
                    {
                        if (sqlwhere.Length > 0) sqlwhere += " AND ";
                        else sqlwhere = "WHERE ";
                        sqlwhere += "TestResult=0";
                    }
                    else if (viewOption == CubeViewOption.Tested)
                    {
                        if (sqlwhere.Length > 0) sqlwhere += " AND ";
                        else sqlwhere = "WHERE ";
                        sqlwhere += "TestResult>0";
                    }

                    sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                    "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Cubes {1} ORDER BY Barcode {0};", sqlseg, sqlwhere);
                }
                else
                {
                    sqlwhere = "WHERE ScoNum=@ScoNum AND BatchId=@BatchId ";
                    if (lastUpdate != null && lastUpdate != DateTime.MinValue)
                    {
                        sqlwhere += "AND LastUpdate>@LastUpdate ";
                    }

                    if (viewOption == CubeViewOption.Untested)
                    {
                        sqlwhere += "AND TestResult=0";
                    }
                    else if (viewOption == CubeViewOption.Tested)
                    {
                        sqlwhere += "AND TestResult>0";
                    }

                    sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                        "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                        "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                        "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                        "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                        "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                        "FROM Cubes {1} ORDER BY Barcode {0};", 
                        sqlseg, sqlwhere);
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);
                    cmd.Parameters.AddWithValue("LastUpdate", lastUpdate.SafeDateTime());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cube c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);


                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                            cubeList.Add(c);
                        }
                    }
                }

                sql = string.Format("SELECT COUNT(*) FROM Cubes {0};", sqlwhere);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);
                    cmd.Parameters.AddWithValue("LastUpdate", lastUpdate.SafeDateTime());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = reader.GetInt32(0);

                            status = true;
                        }
                    }
                }

                return status;
            }
        }

        private string GetCubeOptionSQL(CubeQueryOption option, DateTime? dt=null)
        {
            if (option == CubeQueryOption.Untested)
            {
                return "AND TestResult=0";
            }
            else if (option == CubeQueryOption.PassOnly)
            {
                return "AND TestResult<>0 AND LEN(StatusCode)=0";
            }
            else if (option == CubeQueryOption.FailOnly)
            {
                return "AND TestResult<>0 AND LEN(StatusCode)>0";
            }
            else // All
            {
                return "";
            }
        }

        public bool GetCubesWithOption(List<Cube> cubeList, int scoNum, int batchId, CubeQueryOption option, 
            DateTime? lastUpdate = null)
        {
            string sql;
            bool status = false;
            cubeList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";

                if (scoNum == 0 || batchId == 0)
                {
                    if (lastUpdate != null && lastUpdate != DateTime.MinValue)
                    {
                        sqlwhere = "WHERE LastUpdate>@LastUpdate";
                    }
                    sqlwhere += GetCubeOptionSQL(option);
                    
                    sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                        "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                        "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                        "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                        "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                        "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                        "FROM Cubes {1} ORDER BY ScoNum, BatchId {0};", sqlseg, sqlwhere);
                }
                else
                {
                    if (lastUpdate != null && lastUpdate != DateTime.MinValue)
                    {
                        sqlwhere = "AND LastUpdate>@LastUpdate";
                    }
                    sqlwhere += GetCubeOptionSQL(option);

                    sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                        "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                        "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                        "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                        "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                        "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                        "FROM Cubes WHERE ScoNum={1} AND BatchId={2} {3} ORDER BY ScoNum, BatchId {0};",
                        sqlseg, scoNum, batchId, sqlwhere);
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("LastUpdate", lastUpdate.SafeDateTime());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cube c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);


                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                            cubeList.Add(c);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }

        public bool GetCubesForToday(List<Cube> cubeList, CubeQueryOption option, bool testedToday)
        {
            string sql;
            bool status = false;
            cubeList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";
                string sqltoday = testedToday ? "AND CAST(C.ActualTestDate AS DATE)=CAST(@Today AS DATE)" : "";

                sqlwhere += GetCubeOptionSQL(option);                

                sql = string.Format("SELECT C.Id, Barcode, SampleRef, C.ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                    "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                    "C.CreateUser, C.CreateDate, C.LastUpdateUser, C.LastUpdate " +
                    "FROM Cubes C, Batches B WHERE C.ScoNum=B.ScoNum AND C.BatchId=B.Id AND " +
                    "(CAST(B.TargetTestDate AS DATE)=CAST(@Today AS DATE) OR " +
                    "CAST(B.TargetTestDate AS DATE)<CAST(@Today AS DATE) AND C.TestResult=0) {1} {2} " +
                    "ORDER BY C.ScoNum, C.BatchId {0};", sqlseg, sqlwhere, sqltoday);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Today", DateTime.Now);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cube c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);


                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                            cubeList.Add(c);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }

        public bool GetTestedCubes(List<Cube> cubeList, int scoNum, int batchId, DateTime dtStart, DateTime dtEnd)
        {
            string sql;
            bool status = false;
            cubeList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlseg = "";
                string sqlwhere = "";

                sqlwhere = "AND CAST(ActualTestDate AS DATE)>=CAST(@StartDate AS DATE) AND " +
                    "CAST(ActualTestDate AS DATE)<=CAST(@EndDate AS DATE)";

                sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                    "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Cubes WHERE ScoNum=@ScoNum AND BatchId=@BatchId {1} " +
                    "ORDER BY ScoNum, BatchId, ActualTestDate {0};",
                    sqlseg, sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);
                    cmd.Parameters.AddWithValue("StartDate", dtStart);
                    cmd.Parameters.AddWithValue("EndDate", dtEnd);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cube c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);


                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                            cubeList.Add(c);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }

        public bool GetTestedCubesForProject(int projectId, List<Cube> cubeList, DateTime dtStart, DateTime dtEnd)
        {
            string sql;
            bool status = false;
            cubeList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT C.Id, Barcode, SampleRef, C.ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                    "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                    "C.CreateUser, C.CreateDate, C.LastUpdateUser, C.LastUpdate " +
                    "FROM Cubes C, Batches, CubeSets, Projects " +
                    "WHERE C.ScoNum=Batches.ScoNum AND C.BatchId=Batches.Id AND Batches.CubeSetId=CubeSets.Id AND " +
                    "CubeSets.ProjectId=Projects.Id AND Projects.Id=@ProjectId AND C.TestResult>0 AND " +
                    "CAST(ActualTestDate AS DATE)>=CAST(@StartDate AS DATE) AND " +
                    "CAST(ActualTestDate AS DATE)<=CAST(@EndDate AS DATE) ORDER BY ActualTestDate;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("StartDate", dtStart);
                    cmd.Parameters.AddWithValue("EndDate", dtEnd);
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cube c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);

                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                            cubeList.Add(c);
                        }

                        status = true;
                    }
                }

                return status;
            }
        }

        public Cube GetCube(long barcode)
        {
            Cube c = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                        "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                        "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                        "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                        "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                        "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                        "FROM Cubes WHERE Barcode=@Barcode;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Barcode", barcode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            c = new Cube();

                            c.Id = reader.SafeGetLong(0);
                            c.Barcode = reader.SafeGetLong(1);
                            c.SampleRef = reader.SafeGetString(2);
                            c.ScoNum = reader.SafeGetInt(3);
                            c.BatchId = reader.SafeGetInt(4);
                            c.MeasuredDimX1 = reader.SafeGetDouble(5);
                            c.MeasuredDimX2 = reader.SafeGetDouble(6);
                            c.MeasuredDimX3 = reader.SafeGetDouble(7);
                            c.MeasuredDimX4 = reader.SafeGetDouble(8);
                            c.MeasuredDimX5 = reader.SafeGetDouble(9);
                            c.MeasuredDimX6 = reader.SafeGetDouble(10);
                            c.MeasuredDimY1 = reader.SafeGetDouble(11);
                            c.MeasuredDimY2 = reader.SafeGetDouble(12);
                            c.MeasuredDimY3 = reader.SafeGetDouble(13);
                            c.MeasuredDimY4 = reader.SafeGetDouble(14);
                            c.MeasuredDimY5 = reader.SafeGetDouble(15);
                            c.MeasuredDimY6 = reader.SafeGetDouble(16);
                            c.AvgDimension = reader.SafeGetDouble(17);
                            c.MeasuredMaxForce = reader.SafeGetDouble(18);
                            c.MeasuredStrength = reader.SafeGetDouble(19);
                            c.MeasuredWeight = reader.SafeGetDouble(20);
                            c.MeasuredDensity = reader.SafeGetDouble(21);
                            c.TesterId = reader.SafeGetByte(22);
                            c.TestResult = reader.SafeGetByte(23);
                            c.StatusCode = reader.SafeGetString(24);
                            c.Uploaded = reader.SafeGetByte(25) > 0;
                            c.ActualTestDate = reader.SafeGetDateTime(26);
                            c.UploadTime = reader.SafeGetDateTime(27);
                            c.CreateUser = reader.SafeGetString(28);
                            c.CreateDate = reader.SafeGetDateTime(29);
                            c.LastUpdateUser = reader.SafeGetString(30);
                            c.LastUpdate = reader.SafeGetDateTime(31);

                            c.BatchNum = $"{c.ScoNum}-{c.BatchId}";
                        }
                    }
                }

                return c;
            }
        }

        public int GetCubeQtyForBatch(int scoNum, int batchId)
        {
            int cnt = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) " +
                        "FROM Batches, Cubes " +
                        "WHERE Batches.ScoNum=Cubes.ScoNum AND Batches.Id=Cubes.BatchId AND " +
                        "Cubes.ScoNum=@ScoNum AND Cubes.BatchId=@BatchId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cnt = reader.GetInt32(0);
                        }
                    }
                }

                return cnt;
            }
        }

        public TimeSpan GetCubeTimeUncompletedForBatch(int scoNum, int batchId)
        {
            DateTime? firstTest = null;
            TimeSpan ts = new TimeSpan(0);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT MIN(ActualTestDate) FROM Cubes " +
                        "WHERE ScoNum=@ScoNum AND BatchId=@BatchId AND " +
                        "TestResult>0 AND ActualTestDate IS NOT NULL;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                           firstTest = reader.SafeGetDateTime(0);
                        }
                    }
                }

                if (firstTest.HasValue)
                {
                    ts = DateTime.Now - (DateTime)firstTest;
                }

                return ts;
            }
        }

        public bool InsertCubes(int scoNum, int batchId, List<Cube> cubeList, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    foreach (Cube c in cubeList)
                    {
                        status = BarcodeIsUnique(c.Barcode) && BarcodeIsInRange(c.Barcode, scoNum, batchId);
                        if (!status) break;

                        c.ScoNum = scoNum;
                        c.BatchId = batchId;
                        c.CreateUser = userId;
                        c.CreateDate = DateTime.Now;
                        c.LastUpdateUser = userId;

                        sql = "INSERT INTO Cubes (Barcode, SampleRef, ScoNum, BatchId, " +
                            "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                            "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                            "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                            "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                            "CreateUser, CreateDate, LastUpdateUser) " +
                        "VALUES (@Barcode, @SampleRef, @ScoNum, @BatchId, " +
                            "@MeasuredDimX1, @MeasuredDimX2, @MeasuredDimX3, @MeasuredDimX4, @MeasuredDimX5, @MeasuredDimX6, " +
                            "@MeasuredDimY1, @MeasuredDimY2, @MeasuredDimY3, @MeasuredDimY4, @MeasuredDimY5, @MeasuredDimY6," +
                            "@AvgDimension, @MeasuredMaxForce, @MeasuredStrength, @MeasuredWeight, @MeasuredDensity, @TesterId, " +
                            "@TestResult, @StatusCode, @Uploaded, @ActualTestDate, @UploadTime, " +
                            "@CreateUser, @CreateDate, @LastUpdateUser);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                            cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                            cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                            cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                            cmd.Parameters.AddWithValue("MeasuredDimX1", c.MeasuredDimX1);
                            cmd.Parameters.AddWithValue("MeasuredDimX2", c.MeasuredDimX2);
                            cmd.Parameters.AddWithValue("MeasuredDimX3", c.MeasuredDimX3);
                            cmd.Parameters.AddWithValue("MeasuredDimX4", c.MeasuredDimX4);
                            cmd.Parameters.AddWithValue("MeasuredDimX5", c.MeasuredDimX5);
                            cmd.Parameters.AddWithValue("MeasuredDimX6", c.MeasuredDimX6);
                            cmd.Parameters.AddWithValue("MeasuredDimY1", c.MeasuredDimY1);
                            cmd.Parameters.AddWithValue("MeasuredDimY2", c.MeasuredDimY2);
                            cmd.Parameters.AddWithValue("MeasuredDimY3", c.MeasuredDimY3);
                            cmd.Parameters.AddWithValue("MeasuredDimY4", c.MeasuredDimY4);
                            cmd.Parameters.AddWithValue("MeasuredDimY5", c.MeasuredDimY5);
                            cmd.Parameters.AddWithValue("MeasuredDimY6", c.MeasuredDimY6);
                            cmd.Parameters.AddWithValue("AvgDimension", c.AvgDimension);
                            cmd.Parameters.AddWithValue("MeasuredMaxForce", c.MeasuredMaxForce);
                            cmd.Parameters.AddWithValue("MeasuredStrength", c.MeasuredStrength);
                            cmd.Parameters.AddWithValue("MeasuredWeight", c.MeasuredWeight);
                            cmd.Parameters.AddWithValue("MeasuredDensity", c.MeasuredDensity);
                            cmd.Parameters.AddWithValue("TesterId", c.TesterId);
                            cmd.Parameters.AddWithValue("TestResult", c.TestResult);
                            cmd.Parameters.AddWithValue("StatusCode", c.StatusCode);
                            cmd.Parameters.AddWithValue("Uploaded", c.Uploaded);
                            cmd.Parameters.AddWithValue("ActualTestDate", c.ActualTestDate.SafeDateTime());
                            cmd.Parameters.AddWithValue("UploadTime", c.UploadTime.SafeDateTime());
                            cmd.Parameters.AddWithValue("CreateUser", c.CreateUser);
                            cmd.Parameters.AddWithValue("CreateDate", c.CreateDate.SafeDateTime());
                            cmd.Parameters.AddWithValue("LastUpdateUser", c.LastUpdateUser);

                            status = (cmd.ExecuteNonQuery() == 1);
                            if (!status) break;
                        }

                        if (status)
                        {
                            ActivityLog log = new ActivityLog()
                            {
                                Activity = string.Format("Added Cube {0}", c.Barcode),
                                TableName = "Cubes",
                                UserId = userId
                            };

                            status = LogActivity(log, conn, trans);
                            if (!status) break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (status) trans.Commit();
                    else
                    {
                        trans.Rollback();
                        return false;
                    }

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting cube list: {0}", ex.Message);
                return false;
            }

        }


        public bool InsertCube(Cube c, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    status = BarcodeIsUnique(c.Barcode) && BarcodeIsInRange(c.Barcode, c.ScoNum, c.BatchId);

                    if (status)
                    {
                        c.CreateUser = userId;
                        c.CreateDate = DateTime.Now;
                        c.LastUpdateUser = userId;

                        string sql = "INSERT INTO Cubes (Barcode, SampleRef, ScoNum, BatchId, " +
                                "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                                "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6," +
                                "AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                                "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, " +
                                "CreateUser, CreateDate, LastUpdateUser) " +
                            "VALUES (@Barcode, @SampleRef, @ScoNum, @BatchId, " +
                                "@MeasuredDimX1, @MeasuredDimX2, @MeasuredDimX3, @MeasuredDimX4, @MeasuredDimX5, @MeasuredDimX6, " +
                                "@MeasuredDimY1, @MeasuredDimY2, @MeasuredDimY3, @MeasuredDimY4, @MeasuredDimY5, @MeasuredDimY6," +
                                "@AvgDimension, @MeasuredMaxForce, @MeasuredStrength, @MeasuredWeight, @MeasuredDensity, @TesterId, " +
                                "@TestResult, @StatusCode, @Uploaded, @ActualTestDate, @UploadTime, " +
                                "@CreateUser, @CreateDate, @LastUpdateUser);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                            cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                            cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                            cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                            cmd.Parameters.AddWithValue("MeasuredDimX1", c.MeasuredDimX1);
                            cmd.Parameters.AddWithValue("MeasuredDimX2", c.MeasuredDimX2);
                            cmd.Parameters.AddWithValue("MeasuredDimX3", c.MeasuredDimX3);
                            cmd.Parameters.AddWithValue("MeasuredDimX4", c.MeasuredDimX4);
                            cmd.Parameters.AddWithValue("MeasuredDimX5", c.MeasuredDimX5);
                            cmd.Parameters.AddWithValue("MeasuredDimX6", c.MeasuredDimX6);
                            cmd.Parameters.AddWithValue("MeasuredDimY1", c.MeasuredDimY1);
                            cmd.Parameters.AddWithValue("MeasuredDimY2", c.MeasuredDimY2);
                            cmd.Parameters.AddWithValue("MeasuredDimY3", c.MeasuredDimY3);
                            cmd.Parameters.AddWithValue("MeasuredDimY4", c.MeasuredDimY4);
                            cmd.Parameters.AddWithValue("MeasuredDimY5", c.MeasuredDimY5);
                            cmd.Parameters.AddWithValue("MeasuredDimY6", c.MeasuredDimY6);
                            cmd.Parameters.AddWithValue("AvgDimension", c.AvgDimension);
                            cmd.Parameters.AddWithValue("MeasuredMaxForce", c.MeasuredMaxForce);
                            cmd.Parameters.AddWithValue("MeasuredStrength", c.MeasuredStrength);
                            cmd.Parameters.AddWithValue("MeasuredWeight", c.MeasuredWeight);
                            cmd.Parameters.AddWithValue("MeasuredDensity", c.MeasuredDensity);
                            cmd.Parameters.AddWithValue("TesterId", c.TesterId);
                            cmd.Parameters.AddWithValue("TestResult", c.TestResult);
                            cmd.Parameters.AddWithValue("StatusCode", c.StatusCode);
                            cmd.Parameters.AddWithValue("Uploaded", c.Uploaded);
                            cmd.Parameters.AddWithValue("ActualTestDate", c.ActualTestDate.SafeDateTime());
                            cmd.Parameters.AddWithValue("UploadTime", c.UploadTime.SafeDateTime());
                            cmd.Parameters.AddWithValue("CreateUser", c.CreateUser);
                            cmd.Parameters.AddWithValue("CreateDate", c.CreateDate.SafeDateTime());
                            cmd.Parameters.AddWithValue("LastUpdateUser", c.LastUpdateUser);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added Cube {0}", c.Barcode),
                            TableName = "Cubes",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting cube {0}: {1}", c.Barcode, ex.Message);
                return false;
            }
        }

        public bool UpdateCubeBarcodes(List<Cube> cubes, string userId)
        {
            bool status = false;

            // check that they have not been tested
            foreach(Cube cube in cubes)
            {
                if (cube.TestResult != 0) return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    foreach (Cube c in cubes)
                    {
                        string sql = "UPDATE Cubes SET Barcode=@Barcode, SampleRef=@SampleRef, " +
                            "ScoNum=@ScoNum, BatchId=@BatchId, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                            "WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Id", c.Id);
                            cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                            cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                            cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                            cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                            cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                            cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }

                        if (!status) break;

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Cube {0}", c.Barcode),
                            TableName = "Cubes",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);

                        if (!status) break;
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating cube barcodes: {0}", ex.Message);
                return false;
            }
        }

        public double GetAvgStrength(int cubeSetId, int testAge)
        {
            double strength = double.NaN;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = "SELECT AVG(Cubes.MeasuredStrength) " +
                             "FROM Cubes,Batches,CubeSets " +
                             "WHERE CubeSets.Id=@CubeSetId AND Batches.TestAge=@TestAge AND " +
                             "Batches.CubeSetId=CubeSets.Id AND " +
                             "Cubes.ScoNum=Batches.ScoNum AND Cubes.BatchId=Batches.Id AND " +
                             "(Cubes.TestResult=1 OR Cubes.TestResult=2);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("CubeSetId", cubeSetId);
                        cmd.Parameters.AddWithValue("TestAge", testAge);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                strength = reader.GetDouble(0);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Error calculating avg strength for cubeset {0}", cubeSetId);
                }
            }

            return strength;

        }

        public (double, double) GetMinMaxStrength(int cubeSetId, int testAge)
        {
            double min = double.NaN;
            double max = double.NaN;
            int cnt = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = "SELECT COUNT(*), MIN(Cubes.MeasuredStrength), MAX(Cubes.MeasuredStrength) " +
                             "FROM Cubes,Batches,CubeSets " +
                             "WHERE CubeSets.Id=@CubeSetId AND Batches.TestAge=@TestAge AND " +
                             "Batches.CubeSetId=CubeSets.Id AND " +
                             "Cubes.ScoNum=Batches.ScoNum AND Cubes.BatchId=Batches.Id AND " +
                             "(Cubes.TestResult=1 OR Cubes.TestResult=2);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("CubeSetId", cubeSetId);
                        cmd.Parameters.AddWithValue("TestAge", testAge);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cnt = reader.GetInt32(0);
                                if (cnt > 0)
                                {
                                    min = reader.GetDouble(1);
                                    max = reader.GetDouble(2);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Global.logger.LogMessageEx("Error", "Error getting min/max strength for cubeset {0}", cubeSetId);
                }
            }

            return (min, max);

        }

        public bool EvaluateBatchCompletion()
        {
            bool status;
            List<Batch> batchList = new List<Batch>();
            status = Global.db.GetBatchesForToday(batchList, CubeViewOption.Untested);
            if (!status) return false;

            foreach(Batch b in batchList)
            {
                TimeSpan? ts = Global.db.GetCubeTimeUncompletedForBatch(b.ScoNum, b.Id);
                if (ts != null && ((TimeSpan)ts).TotalHours > 2.0)
                {
                    int total;
                    List<Cube> cubeList = new List<Cube>();
                    status = Global.db.GetCubes(cubeList, b.ScoNum, b.Id, CubeViewOption.Untested, out total); 
                    if (status)
                    {
                        foreach(Cube c in cubeList)
                        {
                            if (!c.StatusCode.Contains("G"))
                            {
                                c.StatusCode += "G"; // batch untested for too long
                                status = Global.db.UpdateCube(c, "system");
                            }
                        }
                    }

                }
            }

            return true;
        }

        public bool EvaluateCube(Cube c)
        {
            Batch b = GetBatch(c.ScoNum, c.BatchId);
            if (b == null) return false;

            CubeSet cs = GetCubeSet(b.CubeSetId);
            if (cs == null) return false;

            double cgrade = cs.ConcreteGrade;

            c.StatusCode = "";
            //c.TestResult = (int)CubeTestResult.Pending;

            // pass or failed determined at cube tester PC and not here

            // A: 28 days result < concrete grade
            if (b.TestAge == 28 && c.MeasuredStrength < cgrade)
            {
                c.StatusCode += "A";
                //c.TestResult = (int)CubeTestResult.Fail; // fail
            }

            // B: 28 days result < 7 days result
            double strength7days = GetAvgStrength(cs.Id, 7);
            if (!double.IsNaN(strength7days))
            {
                if (b.TestAge == 28 && c.MeasuredStrength < strength7days)
                {
                    c.StatusCode += "B";
                    //c.TestResult = (int)CubeTestResult.Fail; // fail;
                }
            }

            // C: 56 days < 28 days < 7 days
            double strength28days = GetAvgStrength(cs.Id, 28);
            if (!double.IsNaN(strength28days))
            {
                if (b.TestAge == 56 && c.MeasuredStrength < strength28days)
                {
                    c.StatusCode += "C";
                    //c.TestResult = (int)CubeTestResult.Fail; // fail;
                }
            }

            // D: Highest-Lowest > 15% of mean of set
            double min, max, mean;
            mean = GetAvgStrength(cs.Id, b.TestAge);
            (min, max) = GetMinMaxStrength(cs.Id, b.TestAge);
            if (!double.IsNaN(min) && !double.IsNaN(max))
            {
                if (max - min > 0.15*mean)
                {
                    c.StatusCode += "D";
                    //c.TestResult = (int)CubeTestResult.Fail; // fail
                }
            }

            // E: 7 days result < 65% of concrete grade
            if (b.TestAge == 7 && c.MeasuredStrength < 0.65*cgrade)
            {
                c.StatusCode += "E";
                //c.TestResult = (int)CubeTestResult.Fail; // fail
            }

            // F: 28 days mean < characteristic strength+1
            if (b.TestAge == 28 && strength28days < cs.CharacteristicStrength + 1)
            {
                c.StatusCode += "F";
                //c.TestResult = (int)CubeTestResult.Fail; // fail
            }

            // G: Alert staff if batch not completed within 2 hours
            // Run in time-interval task

            //if (c.StatusCode.Length == 0)
            //{
            //    c.TestResult = (int)CubeTestResult.Pass; // pass
            //}

            return true;
        }

        public List<Cube> ScanForBatchTimeout(Batch b, double timeoutHours, out double actualHours)
        {
            string sql;
            List<Cube> cubes = new List<Cube>();
            actualHours = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // see if it has been tested
                DateTime? dtFirstTest = null;
                sql = "SELECT MIN(Cubes.ActualTestDate) FROM Cubes, Batches " +
                      "WHERE Cubes.ScoNum=Batches.ScoNum AND Cubes.BatchId=Batches.Id AND " +
                      "Batches.ScoNum=@ScoNum AND Batches.Id=@BatchId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                    cmd.Parameters.AddWithValue("BatchId", b.Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dtFirstTest = reader.SafeGetDateTime(0);
                        }
                    }
                }

                if (dtFirstTest == null) return cubes;

                             
                // has it passed timeout?
                TimeSpan ts = (DateTime.Now - (DateTime)dtFirstTest);
                actualHours = ts.TotalHours;
                if (actualHours < 2) return cubes;

                // get cubes
                List<int> barcodes = new List<int>();
                sql = "SELECT Barcode FROM Cubes " +
                      "WHERE Cubes.ScoNum=@ScoNum AND Cubes.BatchId=@BatchId AND Cubes.TestResult=0;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                    cmd.Parameters.AddWithValue("BatchId", b.Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int barcode = reader.SafeGetInt(0);

                            barcodes.Add(barcode);
                        }
                    }
                }

                foreach (int barcode in barcodes)
                {
                    Cube c = Global.db.GetCube(barcode);
                    if (c != null)
                    {
                        cubes.Add(c);
                    }
                }
            }

            return cubes;
        }

        public bool UpdateCube(Cube c, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Cubes SET SampleRef=@SampleRef, ScoNum=@ScoNum, BatchId=@BatchId, " +
                            "MeasuredDimX1=@MeasuredDimX1, MeasuredDimX2=@MeasuredDimX2, MeasuredDimX3=@MeasuredDimX3, " +
                            "MeasuredDimX4=@MeasuredDimX4, MeasuredDimX5=@MeasuredDimX5, MeasuredDimX6=@MeasuredDimX6, " +
                            "MeasuredDimY1=@MeasuredDimY1, MeasuredDimY2=@MeasuredDimY2, MeasuredDimY3=@MeasuredDimY3, " +
                            "MeasuredDimY4=@MeasuredDimY4, MeasuredDimY5=@MeasuredDimY5, MeasuredDimY6=@MeasuredDimY6," +
                            "AvgDimension=@AvgDimension, MeasuredMaxForce=@MeasuredMaxForce, MeasuredStrength=@MeasuredStrength, " +
                            "MeasuredWeight=@MeasuredWeight, MeasuredDensity=@MeasuredDensity, TesterId=@TesterId, " +
                            "TestResult=@TestResult, StatusCode=@StatusCode, Uploaded=@Uploaded, ActualTestDate=@ActualTestDate, " +
                            "UploadTime=@UploadTime, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                            "WHERE Barcode=@Barcode;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                        cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                        cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                        cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                        cmd.Parameters.AddWithValue("MeasuredDimX1", c.MeasuredDimX1);
                        cmd.Parameters.AddWithValue("MeasuredDimX2", c.MeasuredDimX2);
                        cmd.Parameters.AddWithValue("MeasuredDimX3", c.MeasuredDimX3);
                        cmd.Parameters.AddWithValue("MeasuredDimX4", c.MeasuredDimX4);
                        cmd.Parameters.AddWithValue("MeasuredDimX5", c.MeasuredDimX5);
                        cmd.Parameters.AddWithValue("MeasuredDimX6", c.MeasuredDimX6);
                        cmd.Parameters.AddWithValue("MeasuredDimY1", c.MeasuredDimY1);
                        cmd.Parameters.AddWithValue("MeasuredDimY2", c.MeasuredDimY2);
                        cmd.Parameters.AddWithValue("MeasuredDimY3", c.MeasuredDimY3);
                        cmd.Parameters.AddWithValue("MeasuredDimY4", c.MeasuredDimY4);
                        cmd.Parameters.AddWithValue("MeasuredDimY5", c.MeasuredDimY5);
                        cmd.Parameters.AddWithValue("MeasuredDimY6", c.MeasuredDimY6);
                        cmd.Parameters.AddWithValue("AvgDimension", c.AvgDimension);
                        cmd.Parameters.AddWithValue("MeasuredMaxForce", c.MeasuredMaxForce);
                        cmd.Parameters.AddWithValue("MeasuredStrength", c.MeasuredStrength);
                        cmd.Parameters.AddWithValue("MeasuredWeight", c.MeasuredWeight);
                        cmd.Parameters.AddWithValue("MeasuredDensity", c.MeasuredDensity);
                        cmd.Parameters.AddWithValue("TesterId", c.TesterId);
                        cmd.Parameters.AddWithValue("TestResult", c.TestResult);
                        cmd.Parameters.AddWithValue("StatusCode", c.StatusCode);
                        cmd.Parameters.AddWithValue("Uploaded", c.Uploaded);
                        cmd.Parameters.AddWithValue("ActualTestDate", c.ActualTestDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("UploadTime", c.UploadTime.SafeDateTime());
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Cube {0}", c.Barcode),
                            TableName = "Cubes",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating cube {0}: {1}", c.Barcode, ex.Message);
                return false;
            }
        }

        public bool DeleteCube(long barcode, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Cubes WHERE Barcode=@Barcode;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Barcode", barcode);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted Cube {0}", barcode),
                            TableName = "Cubes",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);

                        lock (Global.app.lckDeletedCubeIds)
                        {
                            if (!Global.app.deleteCubeIds.Contains(barcode))
                            {
                                Global.app.deleteCubeIds.Add(barcode);
                            }
                        }
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting cube {0}: {1}", barcode, ex.Message);
                return false;
            }
        }

        public bool BarcodeIsInRange(long barcode, int scoNum, int batchId)
        {
            bool status = false;
            string sql;
            int projId = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get project id
                sql = "SELECT CubeSets.ProjectId FROM CubeSets, " +
                      "Batches WHERE Batches.CubeSetId=CubeSets.Id AND Batches.ScoNum=@ScoNum AND Batches.Id=@BatchId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ScoNum", scoNum);
                    cmd.Parameters.AddWithValue("BatchId", batchId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            projId = reader.SafeGetInt(0);

                            status = projId > 0;
                        }
                    }
                }

                if (!status) return false;

                // get avail barcode
                status = false;
                sql = "SELECT Id FROM BarcodeAllocation " +
                      "WHERE ProjectId=@ProjectId AND @Barcode>=BarcodeStart AND @Barcode<=BarcodeEnd;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Barcode", barcode);
                    cmd.Parameters.AddWithValue("ProjectId", projId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int baId = reader.SafeGetInt(0);

                            status = baId > 0;
                        }
                    }
                }
            }

            return status;
        }

        public bool BarcodeIsUnique(long barcode)
        {
            bool status = false;
            string sql;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT COUNT(*) FROM Cubes WHERE Barcode=@Barcode;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Barcode", barcode);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int cnt = reader.SafeGetInt(0);

                            status = cnt == 0;
                        }
                    }
                }
            }

            return status;
        }
        #endregion
        #region TestSpecs

        public bool GetTestSpecs(List<TestSpec> specList)
        {
            bool status = false;
            string sql;
            specList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT SpecId, Description, TestStandard, LastUpdateUser, LastUpdate FROM TestSpecs;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TestSpec spec = new TestSpec();
                            spec.SpecId = reader.SafeGetString(0);
                            spec.Description = reader.SafeGetString(1);
                            spec.TestStandard = reader.SafeGetString(2);
                            spec.LastUpdateUser = reader.SafeGetString(3);
                            spec.LastUpdate = reader.SafeGetDateTime(4);

                            specList.Add(spec);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public bool GetTestSpecsForSupplier(int projectId, string supplierId, List<TestSpec> specList)
        {
            bool status = false;
            string sql;
            specList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT TestSpecs.SpecId, Description, TestStandard, TestSpecs.LastUpdateUser, TestSpecs.LastUpdate " +
                    "FROM TestSpecs, CubeSets " +
                    "WHERE CubeSets.SpecId=TestSpecs.SpecId AND CubeSets.SupplierId=@SupplierId AND " +
                    "CubeSets.ProjectId=@ProjectId " +
                    "GROUP BY TestSpecs.SpecId, Description, TestStandard, TestSpecs.LastUpdateUser, TestSpecs.LastUpdate;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("SupplierId", supplierId);
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TestSpec spec = new TestSpec();
                            spec.SpecId = reader.SafeGetString(0);
                            spec.Description = reader.SafeGetString(1);
                            spec.TestStandard = reader.SafeGetString(2);
                            spec.LastUpdateUser = reader.SafeGetString(3);
                            spec.LastUpdate = reader.SafeGetDateTime(4);

                            specList.Add(spec);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public TestSpec GetTestSpec(string specId)
        {
            TestSpec spec = null;
            string sql;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT SpecId, Description, TestStandard, LastUpdateUser, LastUpdate " +
                    "FROM TestSpecs WHERE SpecId=@SpecId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("SpecId", specId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            spec = new TestSpec();
                            spec.SpecId = reader.SafeGetString(0);
                            spec.Description = reader.SafeGetString(1);
                            spec.TestStandard = reader.SafeGetString(2);
                            spec.LastUpdateUser = reader.SafeGetString(3);
                            spec.LastUpdate = reader.SafeGetDateTime(4);
                        }
                    }
                }
            }

            return spec;
        }

        public bool InsertTestSpec(TestSpec ts, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "INSERT INTO TestSpecs (SpecId, Description, TestStandard, LastUpdateUser) " +
                        "VALUES (@SpecId, @Description, @TestStandard, @LastUpdateUser);";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("SpecId", ts.SpecId);
                        cmd.Parameters.AddWithValue("Description", ts.Description);
                        cmd.Parameters.AddWithValue("TestStandard", ts.TestStandard);
                        cmd.Parameters.AddWithValue("LastUpdateUser", ts.LastUpdateUser);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (!status)
                    {
                        trans.Rollback();
                        return false;
                    }

                    ActivityLog log = new ActivityLog()
                    {
                        Activity = string.Format("Added TestSpec {0}", ts.SpecId),
                        TableName = "TestSpecs",
                        UserId = userId
                    };

                    status = LogActivity(log, conn, trans);

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting test spec {0}: {1}", ts.SpecId, ex.Message);
                return false;
            }
        }

        public bool UpdateTestSpec(TestSpec ts, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE TestSpecs SET Description=@Description, " +
                        "TestStandard=@TestStandard, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                        "WHERE SpecId=@SpecId;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("SpecId", ts.SpecId);
                        cmd.Parameters.AddWithValue("Description", ts.Description);
                        cmd.Parameters.AddWithValue("TestStandard", ts.TestStandard);
                        cmd.Parameters.AddWithValue("LastUpdateUser", userId);
                        cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated TestSpec {0}", ts.SpecId),
                            TableName = "TestSpecs",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating test spec {0}: {1}", ts.SpecId, ex.Message);
                return false;
            }
        }
        #endregion

        #region Reports
        //*********************************************************************
        // Reports
        //*********************************************************************

        public bool GetReports(int projectId, string reportStatus, string reportType, List<Report> reports, 
            DateTime? startDate = null, DateTime? endDate=null)
        {
            string sql;
            bool status = false;
            reports.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlwhere = "";
                if (projectId != 0)
                {
                    sqlwhere += "ProjectId=@ProjectId ";
                }

                if (reportStatus == "Pending")
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += "Released=0 ";
                }
                else if (reportStatus == "Released")
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += "Released>0 AND Released<3 ";
                }
                else if (reportStatus == "Sent")
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += "Released=3 ";
                }

                if (reportType != "All")
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += $"ReportType='{reportType}' ";
                }

                if (startDate != null && startDate.HasValue)
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += $"(CAST(StartDate AS DATE)=CAST(@StartDate AS DATE)) ";
                }

                if (endDate != null && endDate.HasValue)
                {
                    if (sqlwhere.Length > 0) sqlwhere += "AND ";
                    sqlwhere += $"(CAST(EndDate AS DATE)=CAST(@EndDate AS DATE)) ";
                }

                if (sqlwhere.Length > 0)
                {
                    sqlwhere = "WHERE " + sqlwhere;
                }

                sql = string.Format("SELECT Id, ProjectId, ReportType, FileName, ReportDate, StartDate, " +
                    "EndDate, EmailTo, EmailCC, Released, ReleaseUser, ReleaseDate " +
                    "FROM Reports {0} ORDER BY ReportDate DESC;", sqlwhere);

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);
                    if (startDate != null && startDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("StartDate", startDate);
                    }
                    if (endDate != null && endDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("EndDate", endDate);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Report r = new Report();

                            r.Id = reader.SafeGetInt(0);
                            r.ProjectId = reader.SafeGetInt(1);
                            r.ReportType = reader.SafeGetString(2);
                            r.FileName = reader.SafeGetString(3);
                            r.ReportDate = reader.SafeGetDateTime(4);
                            r.StartDate = reader.SafeGetDateTime(5);
                            r.EndDate = reader.SafeGetDateTime(6);
                            r.EmailTo = Util.LinesToStringList(reader.SafeGetString(7));
                            r.EmailCC = Util.LinesToStringList(reader.SafeGetString(8));
                            r.Released = (byte)reader.SafeGetByte(9);
                            r.ReleaseUser = reader.SafeGetString(10);
                            r.ReleaseDate = reader.SafeGetDateTime(11);

                            reports.Add(r);
                        }

                        status = true;
                    }
                }

                foreach (Report r in reports)
                {
                    string projectCode = Global.db.GetProjectCode(r.ProjectId);
                    r.ProjectCode = projectCode;
                }

                return status;
            }
        }

        public bool ReportExists(ReportState reportState)
        {
            string sql;
            int cnt = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                if (reportState.reportType == "Daily")
                {
                    sql = string.Format("SELECT COUNT(*) FROM Reports " +
                          "WHERE ProjectId=@ProjectId AND ReportType=@ReportType AND " +
                          "CAST(StartDate AS DATE)=CAST(@StartDate AS DATE);");

                }
                else
                {
                    sql = string.Format("SELECT COUNT(*) FROM Reports " +
                          "WHERE ProjectId=@ProjectId AND ReportType=@ReportType AND " +
                          "CAST(StartDate AS DATE)=CAST(@StartDate AS DATE) AND " +
                          "CAST(EndDate AS DATE)=CAST(@EndDate AS DATE);");
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", reportState.project.Id);
                    cmd.Parameters.AddWithValue("ReportType", reportState.reportType);
                    cmd.Parameters.AddWithValue("StartDate", reportState.date1);
                    if (reportState.date2.HasValue)
                    {
                        cmd.Parameters.AddWithValue("EndDate", reportState.date2);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cnt = reader.GetInt32(0);
                        }
                    }
                }

                return (cnt > 0);
            }
        }

        public Report GetReport(int reportId)
        {
            string sql;
            bool status = false;
            Report r = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT Id, ProjectId, ReportType, FileName, ReportDate, StartDate, " +
                    "EndDate, EmailTo, EmailCC, Released, ReleaseUser, ReleaseDate " +
                    "FROM Reports WHERE Id=@Id;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("Id", reportId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            r = new Report();

                            r.Id = reader.SafeGetInt(0);
                            r.ProjectId = reader.SafeGetInt(1);
                            r.ReportType = reader.SafeGetString(2);
                            r.FileName = reader.SafeGetString(3);
                            r.ReportDate = reader.SafeGetDateTime(4);
                            r.StartDate = reader.SafeGetDateTime(5);
                            r.EndDate = reader.SafeGetDateTime(6);
                            r.EmailTo = Util.LinesToStringList(reader.SafeGetString(7));
                            r.EmailCC = Util.LinesToStringList(reader.SafeGetString(8));
                            r.Released = (byte)reader.SafeGetByte(9);
                            r.ReleaseUser = reader.SafeGetString(10);
                            r.ReleaseDate = reader.SafeGetDateTime(11);
                        }

                        status = true;
                    }
                }

                if (status)
                {
                    string projectCode = Global.db.GetProjectCode(r.ProjectId);
                    r.ProjectCode = projectCode;
                }

                return r;
            }
        }

        public bool InsertReport(Report r, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    r.ReportDate = DateTime.Now;

                    string sql = "INSERT INTO Reports (ProjectId, ReportType, FileName, ReportDate, StartDate, EndDate, " +
                        "EmailTo, EmailCC, Released, ReleaseUser, ReleaseDate) " +
                        "VALUES (@ProjectId, @ReportType, @FileName, @ReportDate, @StartDate, @EndDate, " +
                        "@EmailTo, @EmailCC, @Released, @ReleaseUser, @ReleaseDate);" +
                        "SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("ProjectId", r.ProjectId);
                        cmd.Parameters.AddWithValue("ReportType", r.ReportType);
                        cmd.Parameters.AddWithValue("FileName", r.FileName);
                        cmd.Parameters.AddWithValue("ReportDate", r.ReportDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("StartDate", r.StartDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("EndDate", r.EndDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("EmailTo", Util.StringListToLines(r.EmailTo));
                        cmd.Parameters.AddWithValue("EmailCC", Util.StringListToLines(r.EmailCC));
                        cmd.Parameters.AddWithValue("Released", r.Released);
                        cmd.Parameters.AddWithValue("ReleaseUser", r.ReleaseUser.SafeString());
                        cmd.Parameters.AddWithValue("ReleaseDate", r.ReleaseDate.SafeDateTime());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                r.Id = (int)reader.GetDecimal(0);
                            }
                        }
                    }

                    if (r.Id > 0)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added Report {0}", r.Id),
                            TableName = "Report",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting report {0}: {1}", r.Id, ex.Message);
                return false;
            }
        }

        public bool UpdateReport(Report r, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "UPDATE Reports SET ProjectId=@ProjectId, ReportType=@ReportType, FileName=@FileName, " +
                        "ReportDate=@ReportDate, StartDate=@StartDate, EndDate=@EndDate, EmailTo=@EmailTo, EmailCC=@EmailCC, " +
                        "Released=@Released, ReleaseUser=@ReleaseUser, ReleaseDate=@ReleaseDate " +
                        "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", r.Id);
                        cmd.Parameters.AddWithValue("ProjectId", r.ProjectId);
                        cmd.Parameters.AddWithValue("ReportType", r.ReportType);
                        cmd.Parameters.AddWithValue("FileName", r.FileName);
                        cmd.Parameters.AddWithValue("ReportDate", r.ReportDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("StartDate", r.StartDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("EndDate", r.EndDate.SafeDateTime());
                        cmd.Parameters.AddWithValue("EmailTo", Util.StringListToLines(r.EmailTo));
                        cmd.Parameters.AddWithValue("EmailCC", Util.StringListToLines(r.EmailCC));
                        cmd.Parameters.AddWithValue("Released", r.Released);
                        cmd.Parameters.AddWithValue("ReleaseUser", r.ReleaseUser);
                        cmd.Parameters.AddWithValue("ReleaseDate", r.ReleaseDate.SafeDateTime());

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated Report {0}", r.Id),
                            TableName = "Reports",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating report {0}: {1}", r.Id, ex.Message);
                return false;
            }
        }

        public bool ReleaseReport(Report r, bool auto, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    r.Released = (byte)(auto ? 2 : 1);
                    r.ReleaseDate = DateTime.Now;
                    r.ReleaseUser = userId;

                    string sql = "UPDATE Reports " +
                                 "SET Released=@Released, ReleaseUser=@ReleaseUser, ReleaseDate=@ReleaseDate " +
                                 "WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", r.Id);
                        cmd.Parameters.AddWithValue("Released", r.Released);
                        cmd.Parameters.AddWithValue("ReleaseUser", r.ReleaseUser);
                        cmd.Parameters.AddWithValue("ReleaseDate", r.ReleaseDate.SafeDateTime());

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        status = Global.emailer.SendDailyReport(r.ProjectId);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Release Report {0}", r.Id),
                            TableName = "Reports",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating report {0}: {1}", r.Id, ex.Message);
                return false;
            }
        }

        public bool DeleteReport(int reportId, string userId)
        {
            bool status = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    string sql = "DELETE FROM Reports WHERE Id=@Id;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Id", reportId);

                        status = (cmd.ExecuteNonQuery() == 1);
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Deleted Report {0}", reportId),
                            TableName = "Reports",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error deleting report {0}: {1}", reportId, ex.Message);
                return false;
            }
        }


        #endregion

        #region ScheduledTasks

        public bool GetScheduledTasks(List<ScheduledTask> taskList)
        {
            string sql;
            bool status = false;
            taskList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT Name, DayOfMonth, Hour, Minute, LastRun, NextRun " +
                    "FROM ScheduledTasks;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ScheduledTask task = new ScheduledTask();

                            task.Name = reader.SafeGetString(0);
                            task.DayOfMonth = reader.SafeGetInt(1);
                            task.Hour = reader.SafeGetInt(2);
                            task.Minute = reader.SafeGetInt(3);
                            task.LastRun = reader.SafeGetDateTime(4);
                            task.NextRun = reader.SafeGetDateTime(5);

                            taskList.Add(task);

                        }
                        status = true;
                    }
                }
            }

            return status;

        }

        public bool UpdateScheduledTaskTime(ScheduledTask task, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "UPDATE ScheduledTasks SET DayOfMonth=@DayOfMonth, Hour=@Hour, Minute=@Minute " +
                        "WHERE Name=@Name;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("DayOfMonth", task.DayOfMonth);
                        cmd.Parameters.AddWithValue("Hour", task.Hour);
                        cmd.Parameters.AddWithValue("Minute", task.Minute);
                        cmd.Parameters.AddWithValue("Name", task.Name);

                        cmd.ExecuteNonQuery();
                        status = true;
                    }

                    if (status)
                    {

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Update Scheduled {0} Task Date Time", task.Name),
                            TableName = "ScheduledTasks",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating time for scheduled task {0}: {1}", task.Name, ex.Message);
                return false;
            }
        }

        public bool UpdateScheduledRunTimes(ScheduledTask task, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "UPDATE ScheduledTasks SET LastRun=@LastRun, NextRun=@NextRun " +
                        "WHERE Name=@Name;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("LastRun", task.LastRun.SafeDateTime());
                        cmd.Parameters.AddWithValue("NextRun", task.NextRun);
                        cmd.Parameters.AddWithValue("Name", task.Name);

                        status = cmd.ExecuteNonQuery() > 0;
                    }

                    //if (status)
                    //{

                    //    ActivityLog log = new ActivityLog()
                    //    {
                    //        Activity = string.Format("Update Scheduled {0} Task Run Times", task.Name),
                    //        TableName = "ScheduledTasks",
                    //        UserId = userId
                    //    };

                    //    status = LogActivity(log, conn, trans);
                    //}

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating run times for scheduled task {0}: {1}", task.Name, ex.Message);
                return false;
            }
        }

        #endregion

        #region Utilities
        public bool GetConcreteGrades(List<int> cgrades)
        {
            bool status = false;
            string sql;
            cgrades.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT Grade FROM ConcreteGrades;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int grade = reader.SafeGetInt(0);

                            cgrades.Add(grade);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public bool UpdateConcreteGrades(List<int> cgrades, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "DELETE FROM ConcreteGrades";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        status = cmd.ExecuteNonQuery() >= 0;
                    }

                    foreach (int g in cgrades)
                    {
                        sql = "INSERT INTO ConcreteGrades (Grade) VALUES (@Grade);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Grade", g);

                            cmd.ExecuteNonQuery();
                            status = true;
                        }

                        if (!status) break;

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Insert/Update Concrete Grade {0}", g),
                            TableName = "ConcreteGrades",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating concrete grades: {0}", ex.Message);
                return false;
            }
        }

        public bool GetTestCriteria(List<string> criteria)
        {
            bool status = false;
            string sql;
            criteria.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT Criterion FROM TestCriteria;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string c = reader.SafeGetString(0);

                            criteria.Add(c);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public bool UpdateTestCriteria(List<string> testCri, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "DELETE FROM TestCriteria;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.ExecuteNonQuery();
                        status = true;
                    }

                    foreach (string t in testCri)
                    {
                        sql = "INSERT INTO TestCriteria (Criterion) VALUES (@Criterion);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Criterion", t);

                            cmd.ExecuteNonQuery();
                            status = true;
                        }

                        if (!status) break;

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Insert/Update Test Criterion {0}", t),
                            TableName = "TestCriteria",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating test criteria: {0}", ex.Message);
                return false;
            }

        }

        public bool GetConcreteTypes(List<string> ctypes)
        {
            bool status = false;
            string sql;
            ctypes.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT Type FROM ConcreteTypes;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string t = reader.SafeGetString(0);

                            ctypes.Add(t);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public bool UpdateConcreteTypes(List<string> ctypes, string userId)
        {
            bool status = false;
            string sql;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    sql = "DELETE FROM ConcreteTypes;";
                    using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                    {
                        cmd.ExecuteNonQuery();
                        status = true;
                    }

                    foreach (string t in ctypes)
                    {
                        sql = "INSERT INTO ConcreteTypes (Type) VALUES (@Type);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Type", t);

                            cmd.ExecuteNonQuery();
                            status = true;
                        }

                        if (!status) break;

                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Insert/Update Concrete Type {0}", t),
                            TableName = "ConcreteTypes",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error updating concrete types: {0}", ex.Message);
                return false;
            }

        }

        public bool GetSCONumbers(int projectId, List<SCONumber> scoList)
        {
            bool status = false;
            string sql;
            scoList.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // get avail barcode
                sql = "SELECT SCONum, ProjectId, LastUpdateUser, LastUpdate " +
                    "FROM SCONumbers WHERE ProjectId=@ProjectId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ProjectId", projectId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SCONumber sco = new SCONumber();
                            sco.ScoNumber = reader.SafeGetInt(0);
                            sco.ProjectId = reader.SafeGetInt(1);
                            sco.LastUpdateUser = reader.SafeGetString(2);
                            sco.LastUpdate = reader.SafeGetDateTime(3);

                            scoList.Add(sco);

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        public bool HasScoNum(int scoNum)
        {
            string sql;
            int cnt = 0;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                sql = "SELECT COUNT(*) FROM SCONumbers WHERE ScoNum=@ScoNum;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("SCONum", scoNum);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cnt = reader.SafeGetInt(0);
                        }
                    }
                }

                return cnt > 0;
            }
        }


        public bool InsertSCONum(Project p, SCONumber sco, string userId)
        {
            bool status = false;
            if (sco.ScoNumber <= 0) return false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();

                    try
                    {
                        string sql = "INSERT INTO SCONumbers (SCONum, ProjectId, LastUpdateUser) " +
                            "VALUES (@SCONum, @ProjectId, @LastUpdateUser);";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("SCONum", sco.ScoNumber);
                            cmd.Parameters.AddWithValue("ProjectId", sco.ProjectId);
                            cmd.Parameters.AddWithValue("LastUpdateUser", sco.LastUpdateUser);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }
                    }
                    catch
                    {
                        status = false;
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Added New SCO Number {0}", sco.ScoNumber),
                            TableName = "SCO Numbers",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status)
                    {
                        p.CurSCONum = sco.ScoNumber;
                        p.LastUpdateUser = userId;

                        string sql = "UPDATE Projects SET CurSCONum=@CurSCONum, LastUpdateUser=@LastUpdateUser, " +
                            "LastUpdate=@LastUpdate " +
                              "WHERE Id=@Id;";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("Id", p.Id);
                            cmd.Parameters.AddWithValue("CurSCONum", p.CurSCONum);
                            cmd.Parameters.AddWithValue("LastUpdateUser", p.LastUpdateUser);
                            cmd.Parameters.AddWithValue("LastUpdate", DateTime.Now);

                            status = (cmd.ExecuteNonQuery() == 1);
                        }
                    }

                    if (status)
                    {
                        ActivityLog log = new ActivityLog()
                        {
                            Activity = string.Format("Updated SCO Number {2} for Project {0}{1:0000}",
                                p.PaymentCode, p.Id, p.CurSCONum),
                            TableName = "Projects",
                            UserId = userId
                        };

                        status = LogActivity(log, conn, trans);
                    }

                    if (status) trans.Commit();
                    else trans.Rollback();

                    return status;
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error inserting sco num {0} for project {1}: {2}", sco, p.ProjectCode, ex.Message);
                return false;
            }

        }

        #endregion
    }
}
