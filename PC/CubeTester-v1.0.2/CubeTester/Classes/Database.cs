using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Threading;
using FBase;

namespace CubeTester.Classes
{
    public class Database
    {
        protected SQLiteConnection conn;
        protected SQLiteTransaction trans = null;

        public string dbpath;

        public bool Open(string db)
        {
            try
            {
                string connStr = "Data Source=" + db;
                conn = new SQLiteConnection(connStr);
                conn.Open();

                dbpath = db;

                return true;
            }
            catch(Exception ex)
            {
                string msg = string.Format("Exception: {0}", ex.Message);
                Util.ErrorMessageBox(msg, "Settings Database");
                return false;
            }
        }

        public void Close()
        {
            if (conn != null)
            { 
                conn.Close(); 
            }
        }

        public SQLiteDataReader Query(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            cmd.Dispose();

            return reader;
        }

        public object QueryScalar(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = sql;
            object obj = cmd.ExecuteScalar();
            cmd.Dispose();

            return obj;
        }

        public int Execute(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = sql;
            int rows = cmd.ExecuteNonQuery();
            cmd.Dispose();

            return rows;
        }

        public SQLiteTransaction BeginTransaction()
        {
            trans = conn.BeginTransaction();
            return trans;
        }

        public void CommitTransaction()
        {
            trans.Commit();
            trans = null;
        }

        public void RollbackTransaction()
        {
            trans.Rollback();
        }

        public SQLiteCommand Prepare(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = sql;

            return cmd;
        }

        public string ReadSettings(string name)
        {
            string value = null;
            string sql = "SELECT Value FROM Settings WHERE Name=@Name;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("Name", name);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        value = reader.SafeGetString(0);
                    }
                }
            }

            return value;
        }

        public bool ReadAllSettings(Dictionary<string,string> par)
        {
            string name;

            string sql = "SELECT Name, Value FROM Settings;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
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

        public bool SaveAllSettings(Dictionary<string, string> par)
        {
            foreach (var pair in par)
            {
                if (!SaveSettings(pair.Key, pair.Value)) return false;
            }

            return true;
        }

        public bool SaveSettings(string name, string value)
        {
            string sql;
            int rows_affected;

            sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Value", value);

                rows_affected = cmd.ExecuteNonQuery();
            }

            if (rows_affected < 1)
            {   // do an insert
                sql = "INSERT OR IGNORE INTO Settings (Name, Value) VALUES (@Name,@Value)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("Name", name);
                    cmd.Parameters.AddWithValue("Value", value);

                    rows_affected = cmd.ExecuteNonQuery();
                }
            }

            if (rows_affected < 1) return false;

            return true;
        }

        public bool ReadAllNumericSettings(Dictionary<string,double> dblPar)
        {
            string name;
            double val;

            string sql = "SELECT Name, Value FROM Settings;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
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

        public bool SaveAllNumericSettings(Dictionary<string, double> dblPar)
        {
            foreach(var par in dblPar)
            {
                if (!SaveNumericSettings(par.Key, par.Value)) return false;
            }

            return true;
        }

        public bool SaveNumericSettings(string name, double value)
        {
            string sql;
            int rows_affected;

            sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Value", value);

                rows_affected = cmd.ExecuteNonQuery();
            }

            if (rows_affected < 1)
            {   // do an insert
                sql = "INSERT OR IGNORE INTO Settings (Name, Value) VALUES (@Name,@Value)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("Name", name);
                    cmd.Parameters.AddWithValue("Value", value);

                    rows_affected = cmd.ExecuteNonQuery();
                }
            }

            if (rows_affected < 1) return false;

            return true;
        }

        public bool ContainsTable(string tableName)
        {
            int cnt = 0;
            string sql = string.Format("SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = '{0}'", tableName);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cnt = reader.GetInt32(0);
                    }
                }
            }

            return (cnt > 0);
        }



        public bool GetUsers(List<User> userList)
        {
            bool status = false;
            userList.Clear();

            string sql = "SELECT UserId, Name, Privilege, Enabled, Password " +
                         "FROM Users ORDER BY UserId;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User u = new User();

                        u.id = reader.GetString(0);
                        u.name = reader.SafeGetString(1);
                        u.privilege = User.IntToUserPrivilege(reader.GetInt32(2));
                        u.enabled = reader.GetInt32(3) > 0;
                        u.password = reader.SafeGetString(4);

                        userList.Add(u);

                        status = true;
                    }
                }
            }

            return status;
        }

        public int GetUserCount()
        {
            int cnt = 0;

            string sql = "SELECT COUNT(*) FROM Users;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cnt = reader.GetInt32(0);
                    }
                }
            }

            return cnt;
        }

        public User GetUser(string userId)
        {
            User u = null;

            string sql = "SELECT UserId, Name, Privilege, Enabled, Password " +
                         "FROM Users WHERE UserId=@UserId;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("UserId", userId);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        u = new User();

                        u.id = reader.GetString(0);
                        u.name = reader.SafeGetString(1);
                        u.privilege = User.IntToUserPrivilege(reader.GetInt32(2));
                        u.enabled = reader.GetInt32(3) > 0;
                        u.password = reader.SafeGetString(4);
                    }
                }
            }

            return u;
        }

        public bool AddUser(User user)
        {
            bool status = false;

            string sql = "INSERT INTO Users (UserId, Name, Privilege, Enabled, Password) " +
                         "VALUES (@UserId, @Name, @Privilege, @Enabled, @Password);";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("UserId", user.id);
                cmd.Parameters.AddWithValue("Name", user.name);
                cmd.Parameters.AddWithValue("Privilege", user.privilege);
                cmd.Parameters.AddWithValue("Enabled", user.enabled ? 1 : 0);
                cmd.Parameters.AddWithValue("Password", user.password);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

        public bool CreateDefaultUsers()
        {
            bool status = false;

            User user = new User();

            string sql = "CREATE TABLE IF NOT EXISTS Users(UserId TEXT, Name TEXT, Privilege INTEGER, Enabled INTEGER, Password TEXT);";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }

            user.id = "admin";
            user.name = "Administrator";
            user.privilege = UserPrivilege.Administrator;
            user.enabled = true;
            user.password = Util.EncryptPassword(Global.passwordKey, "admin", (int)user.privilege);
            AddUser(user);

            user.id = "operator";
            user.name = "Operator";
            user.privilege = UserPrivilege.Operator;
            user.enabled = true;
            user.password = Util.EncryptPassword(Global.passwordKey, "operator", (int)user.privilege);
            AddUser(user);

            return status;
        }

        public bool UpdateUser(User user, bool changePassword)
        {
            bool status = false;
            string sqlSegment = changePassword ? ", Password=@Password " : "";
            string sql = "UPDATE Users SET Name=@Name, Privilege=@Privilege, Enabled=@Enabled " +
                         sqlSegment +
                         "WHERE UserId=@UserId;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("UserId", user.id);
                cmd.Parameters.AddWithValue("Name", user.name);
                cmd.Parameters.AddWithValue("Privilege", user.privilege);
                cmd.Parameters.AddWithValue("Enabled", user.enabled ? 1 : 0);
                cmd.Parameters.AddWithValue("Password", user.password);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

        public bool DeleteUser(string userId)
        {
            bool status = false;

            string sql = "DELETE FROM Users WHERE UserId=@UserId;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("UserId", userId);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

    }

}
