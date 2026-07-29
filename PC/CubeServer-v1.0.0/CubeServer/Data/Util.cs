using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace CubeServer.Data
{
    public static class Util
    {
        public static string GetLocalIP()
        {
            string localIP = null;

            // Resolves a host name or IP address to an IPHostEntry instance.
            // IPHostEntry - Provides a container class for Internet host address information.
            IPHostEntry ipHostEntry = Dns.GetHostEntry(System.Net.Dns.GetHostName());

            // IPAddress class contains the address of a computer on an IP network.
            foreach (System.Net.IPAddress ipAddr in ipHostEntry.AddressList)
            {
                // InterNetwork indicates that an IP version 4 address is expected
                // when a Socket connects to an endpoint
                if (ipAddr.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ipAddr.ToString();
                }
            }
            return localIP;
        }

        public static bool ValidateIPv4Address(string s)
        {
            string[] quads = s.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (quads.Length != 4) return false;

            foreach (string quad in quads)
            {
                int q;
                if (!Int32.TryParse(quad, out q) || q < 0 || q > 255)
                {
                    return false;
                }

            }

            return true;
        }

        public static string EncryptPassword(string password, int privilege)
        {
            byte[] saltedHash;

            using (SHA256 sha = SHA256.Create())
            {
                byte[] salt = { 128, 99, (byte)privilege };
                byte[] bytes = Encoding.ASCII.GetBytes(password);
                byte[] combi = new byte[salt.Length + bytes.Length];
                Buffer.BlockCopy(salt, 0, combi, 0, salt.Length);
                Buffer.BlockCopy(bytes, 0, combi, salt.Length, bytes.Length);

                saltedHash = sha.ComputeHash(combi);
            }

            return Convert.ToBase64String(saltedHash);
        }

        public static int FindByte(byte[] bytes, byte b, int start = 0, int cnt = 0)
        {
            if (cnt == 0) cnt = bytes.Length;

            for (int i = start; i < start + cnt; i++)
            {
                if (bytes[i] == b) return i;
            }

            return -1;
        }

        public static int DoubleToInt(double d)
        {
            if (d < 0) return (int)(d - 0.5);
            else return (int)(d + 0.5);
        }

        public static byte DoubleToByte(double d)
        {
            if (d > 255) return 255;
            else return (byte)(int)d;
        }

        public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        // Adapted From http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) return null;
            if (String.IsNullOrEmpty(toPath)) return null;

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static void CreateBackupFiles(string filePath)
        {
            string fn1 = filePath + ".1";
            string fn2 = filePath + ".2";
            if (File.Exists(fn1)) File.Copy(fn1, fn2, true);

            if (File.Exists(filePath))
            {
                File.Copy(filePath, fn1, true);
            }
        }

        public static string SplitCamelCase(string cs)
        {
            string s = Regex.Replace(cs, "([A-Z])", " $1").Trim();

            return s;
        }

        public static bool CompareByteArray(byte[] a, byte[] b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        public static int FindByte(byte[] a, int start, byte b)
        {
            for (int i = start; i < a.Length; i++)
            {
                if (a[i] == b) return i;
            }

            return -1;
        }

        public static string CalcCRC8(string s)
        {
            byte[] data = Encoding.ASCII.GetBytes(s);
            byte loopCtr;
            byte crc8;
            byte A = 0;
            int i;

            crc8 = 0; // reset CRC8
            for (i = 0; i < data.Length; i++)
            { // data loop

                A = data[i]; // get first data byte

                for (loopCtr = 0; loopCtr < 8; loopCtr++, A >>= 1)
                { // 8 bit loop

                    if ((((int)A ^ (int)crc8) & 1) > 0)
                    { // test bit 0 of (OneWire.Data XOR CRC8)
                        crc8 ^= 0x18; // toggle bits 3 and 4 of CRC8
                        crc8 >>= 1; // rotate right CRC8, 1 time
                        crc8 |= 0x80; // set bit 7 of CRC8
                    }
                    else
                    {
                        crc8 >>= 1; // rotate right CRC8, 1 time
                    }
                }
            }

            string hexOutput = string.Format("{0:X2}", crc8);
            return hexOutput;
        }

        public static byte[] CalcChecksum(byte[] data, int len)
        {
            int checksum = 0;
            for (int i = 0; i < len; i++)
            {
                checksum += data[i];
            }
            checksum = (~checksum) + 1;

            return Int16ToByte(checksum);
        }

        public static byte[] Int16ToByte(int i)
        {
            byte[] array = new byte[2];
            array[0] = (byte)(i >> 8);
            array[1] = (byte)(i);

            return array;
        }

        public static byte[] Int32ToByte(int i)
        {
            byte[] array = new byte[4];
            array[0] = (byte)(i >> 24);
            array[1] = (byte)(i >> 16);
            array[2] = (byte)(i >> 8);
            array[3] = (byte)(i);

            return array;
        }

        public static double Sqr(double x)
        {
            return x * x;
        }


        public static bool StrCompareNoCase(string s1, string s2)
        {
            bool status = string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
            return status;
        }


        public static void DeleteFiles(string directory, string wildcard)
        {
            try
            {
                foreach (string f in Directory.GetFiles(directory, wildcard))
                {
                    File.Delete(f);
                }
            }
            catch (Exception ex)
            {
                // just return. some files may be being used.
            }
        }

        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static List<string> CsvToStringList(string csv)
        {
            string[] arr = csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return arr.ToList();
        }

        public static List<int> CsvToIntList(string csv)
        {
            List<int> list = new List<int>();
            string[] arr = csv.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string s in arr)
            {
                int i;
                if (int.TryParse(s, out i))
                {
                    list.Add(i);
                }
            }

            return list;
        }

        public static string StringListToCsv(List<string> list)
        {
            if (list == null) return "";

            string csv = "";
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i];
                if (i < list.Count - 1)
                {
                    csv += s + ", ";
                }
                else
                {
                    csv += s;
                }
            }

            return csv;
        }

        public static string StringListToCsvLinks(List<string> list, string prefix)
        {
            if (list == null) return "";

            string csv = "";
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i];
                if (i < list.Count - 1)
                {
                    csv += string.Format("<a href=\"{0}/{1}\">{1}</a>, ", prefix, s);
                }
                else
                {
                    csv += string.Format("<a href=\"{0}/{1}\">{1}</a>", prefix, s);
                }
            }

            return csv;
        }

        public static string IntListToCsv(List<int> list)
        {
            if (list == null) return "";

            string csv = "";
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i].ToString();
                if (i < list.Count - 1)
                {
                    csv += s + ", ";
                }
                else
                {
                    csv += s;
                }
            }

            return csv;
        }

        public static string IntListToCsvLinks(List<int> list, string prefix)
        {
            if (list == null) return "";

            string csv = "";
            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i].ToString();
                if (i < list.Count - 1)
                {
                    csv += string.Format("<a href=\"{0}/{1}\">{1}</a>, ", prefix, s);
                }
                else
                {
                    csv += string.Format("<a href=\"{0}/{1}\">{1}</a>", prefix, s);
                }
            }

            return csv;
        }

        public static string ToString(this DateTime? dt, string format)
        {
            if (dt == null) return "";
            return ((DateTime)dt).ToString(format);
        }

        public static string ToString(this List<string> list)
        {
            return StringListToCsv(list);
        }

        public static string SecondsToHours(int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);

            string str;
            if (seconds >= 60 * 60)
            {
                str = string.Format("{0}h {1}m {2}s", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            }
            else if (seconds >= 60)
            {
                str = string.Format("{0}m {1}s", (int)ts.TotalMinutes, ts.Seconds);
            }
            else
            {
                str = string.Format("{0}s", (int)ts.TotalSeconds);
            }
            return str;
        }

        public static bool CanEdit(ClaimsPrincipal user, string[] editRoles)
        {
            foreach (string r in editRoles)
            {
                if (user.IsInRole(r)) return true;
            }

            return false;
        }

        public static bool CanView(ClaimsPrincipal user, string[] viewRoles)
        {
            foreach (string r in viewRoles)
            {
                if (user.IsInRole(r)) return true;
            }

            return false;
        }

        public static string ToYesNo(bool b)
        {
            if (b) return "Yes";
            else return "No";
        }

        public static string DateOnly(DateTime? dt)
        {
            if (dt == null) return ""; // "0000-00-00";
            else
            {
                return dt.ToString("yyyy-MM-dd");
            }
        }

        public static string TestResultStr(int result)
        {
            if (result == 1) return "Pass";
            else if (result == 2) return "Fail";
            else if (result == 3) return "Pending";
            else return "Not Tested";
        }

        public static void ReloadPage(this NavigationManager manager)
        {
            manager.NavigateTo(manager.Uri, true);
        }

        public static void CopyFileIfNewer(string src, string dest)
        {
            FileInfo srcFile = new FileInfo(src);
            FileInfo destFile = new FileInfo(dest);
            if (destFile.Exists)
            {
                if (srcFile.LastWriteTime > destFile.LastWriteTime)
                {
                    srcFile.CopyTo(destFile.FullName, true);
                }
            }
            else
            {
                srcFile.CopyTo(destFile.FullName, true);
            }
        }

        public static List<string> LinesToStringList(string lines)
        {
            string[] s = lines.Split(Global.crSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            List<string> list = new List<string>();
            list.AddRange(s);

            return list;
        }

        public static string StringListToLines(List<string> list)
        {
            string lines = string.Join("\r\n", list);
            return lines;
        }

        public static bool ClearDirectory(string dirname)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(dirname);
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
