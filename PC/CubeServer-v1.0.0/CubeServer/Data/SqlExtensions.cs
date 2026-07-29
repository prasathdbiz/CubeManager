using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeServer.Data
{
    public static class SqlExtensions
    {
        // Sql Extension Methods
        public static string SafeGetString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetString(colIndex);
            }
            else
            {
                return string.Empty;
            }
        }

        public static char SafeGetChar(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                string s = reader.GetString(colIndex);
                if (s == null || s.Length == 0) return ' ';
                else return s[0];
            }
            else
            {
                return ' ';
            }
        }

        public static int SafeGetInt(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static int SafeGetDecimal(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return (int)reader.GetDecimal(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static int SafeGetByte(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetByte(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static long SafeGetLong(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt64(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static double SafeGetDouble(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDouble(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static uint SafeGetUInt(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return (uint)reader.GetInt32(colIndex);
            }
            else
            {
                return 0;
            }
        }

        public static DateTime? SafeGetDateTime(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetDateTime(colIndex);
            }
            else
            {
                return null;
            }
        }

        public static object SafeDateTime(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                DateTime minDate = new DateTime(1900, 1, 1);
                if (dt < minDate)
                {
                    dt = minDate;
                }
                return (DateTime)dt;
            }
            else return DBNull.Value;
        }

        public static object SafeString(this string s)
        {
            if (s == null) return DBNull.Value;
            else return s;
        }

    }
}
