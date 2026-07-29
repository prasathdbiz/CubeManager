using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.ComponentModel;
using System.Threading;
using System.Drawing;

namespace FBase
{
    public class SqliteDB
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

        public bool SaveSettings(string name, string value)
        {
            bool status = false;
            string sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Value", value);

                status = (cmd.ExecuteNonQuery() == 1);
                if (!status) return false;
            }

            return status;
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
            string sql;
            int rows_affected = 0;

            foreach (var pair in par)
            {
                sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("Name", pair.Key);
                    cmd.Parameters.AddWithValue("Value", pair.Value);

                    rows_affected = cmd.ExecuteNonQuery();
                }

                if (rows_affected < 1)
                {   // do an insert
                    sql = "INSERT OR IGNORE INTO Settings (Name, Value) VALUES (@Name,@Value)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Name", pair.Key);
                        cmd.Parameters.AddWithValue("Value", pair.Value);

                        rows_affected = cmd.ExecuteNonQuery();
                    }
                }

                if (rows_affected < 1) return false;
            }

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
            string sql;
            int rows_affected = 0;

            foreach(var par in dblPar)
            {
                sql = "UPDATE Settings SET Value=@Value WHERE Name=@Name;";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                {
                    cmd.Parameters.AddWithValue("Name", par.Key);
                    cmd.Parameters.AddWithValue("Value", par.Value);

                    rows_affected = cmd.ExecuteNonQuery();
                }

                if (rows_affected < 1)
                {   // do an insert
                    sql = "INSERT OR IGNORE INTO Settings (Name, Value) VALUES (@Name,@Value)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("Name", par.Key);
                        cmd.Parameters.AddWithValue("Value", par.Value);

                        rows_affected = cmd.ExecuteNonQuery();
                    }
                }

                if (rows_affected < 1) return false;
            }

            return true;
        }



    }

}
