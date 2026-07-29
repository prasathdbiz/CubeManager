using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FBase
{
	public static class Util 
	{
        // Definitions
        public static string[] dotSeparator = { "." };
        public static string[] commaSeparator = { "," };
        public static string[] equalSeparator = { "=" };
        public static string[] spaceSeparator = { " " };
        public static string[] doublebackslash = { "\\" };
        public static string[] vbarSeparator = { "|" };
        public static string[] semicolonSeparator = { ";" };
        public static string[] colonSeparator = { ":" };
        public static string[] crlfSeparator = { "\r", "\n" };

        // Formats
        public static string positionFormat = "0.000";

        // Math
        public const double RadToDeg = 180 / Math.PI;
        public const double DegToRad = Math.PI / 180;
        public const float mmToMetre = 0.001f;
        public const float mgToM_S2 = 9.80665f * 0.001f;

        // Random
        public static Random rand = new Random();
        public static Stopwatch stopwatch;


        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);
        const int LOGON32_PROVIDER_DEFAULT = 0;
        //This parameter causes LogonUser to create a primary token.   
        const int LOGON32_LOGON_INTERACTIVE = 2;

        [global::System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);
        public const int CXFRAME = 0x20;
        public const int CYFRAME = 0x21;

        //public static object MessageBox { get; private set; }

        public static FLogger logger { get; private set; } // need to set this before use

        public static void Init(FLogger logger)
        {
            rand = new Random();
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static void SetLogger(FLogger logger)
        {
            Util.logger = logger;
        }

        public static bool ImpersonateUser(string username, string domain, string password, 
                                           out SafeAccessTokenHandle safeAccessTokenHandle)
        {
            bool status = LogonUser(username, domain, password,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeAccessTokenHandle);

            return status;
        }

        public static string GetLocalIP()
		{
			string localIP = null;

            // Resolves a host name or IP address to an IPHostEntry instance.
            // IPHostEntry - Provides a container class for Internet host address information.
            IPHostEntry ipHostEntry = Dns.GetHostEntry(global::System.Net.Dns.GetHostName());
	 
			// IPAddress class contains the address of a computer on an IP network.
			foreach (global::System.Net.IPAddress ipAddr in ipHostEntry.AddressList)
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

        public static bool ValidateUrl(string url)
        {
            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri)
                && (uri.Scheme == Uri.UriSchemeHttp
                 || uri.Scheme == Uri.UriSchemeHttps);
        }


        public static bool ValidateIPv4Address(string s)
		{
			string[] quads = s.Split(dotSeparator, StringSplitOptions.RemoveEmptyEntries);

			if (quads.Length != 4) return false;

			foreach (string quad in quads)
			{
				int q;
				if (!Int32.TryParse(quad, out q) || q < 0 || q > 255) {
					return false;
				}

			}

			return true;
		}

        public static bool ValidateAdsAddress(string s)
        {
            string[] quads = s.Split(dotSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (quads.Length != 6) return false;

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

        public static TcpState GetState(this TcpClient tcpClient)
        {
            if (tcpClient == null || tcpClient.Client == null) return TcpState.Unknown;

            var props = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint)
                                 && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint)
              );

            return props != null ? props.State : TcpState.Unknown;
        }

        public static DialogResult ErrorMessageBox(string msg, string title)
		{
			return MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static DialogResult InfoMessageBox(string msg, string title)
		{
			return MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public static DialogResult QuestionMessageBox(string msg, string title)
		{
			return MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}

		public static DialogResult OkCancelMessageBox(string msg, string title)
		{
			return MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
		}

		public static TreeNode FindNode(TreeNodeCollection nodes, string text)
		{
			foreach (TreeNode n in nodes)
			{
				if (n.Text == text)
				{
					return n;
				}
			}

			return null;
		}

		// SQLite Extension Methods
		public static string SafeGetString(this SQLiteDataReader reader, int colIndex)
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

        public static char SafeGetChar(this SQLiteDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetChar(colIndex);
            }
            else
            {
                return ' ';
            }
        }

        public static long SafeGetLong(this SQLiteDataReader reader, int colIndex)
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

        public static int SafeGetInt(this SQLiteDataReader reader, int colIndex)
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

        public static double SafeGetDouble(this SQLiteDataReader reader, int colIndex)
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

        public static DateTime? SafeGetDateTime(this SQLiteDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
			{
                try
                {
                    return reader.GetDateTime(colIndex);
                }
                catch
                {
                    return null;
                }
			}
			else
			{ 
				return null; 
			}
		}

        // SqlClient Extension Methods
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

        public static int SafeGetInt(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                return reader.GetInt32(colIndex);
            }
            else
            {
                return -1;
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

        public static string ToString(this DateTime? dt, string format)
        {
            if (dt == null) return "";
            else
            {
                return ((DateTime)dt).ToString(format);
            }
        }

        public static string EncryptPassword(byte[] salt, string password, int privilege)
		{
            byte[] salt2 = new byte[salt.Length + 1];
            salt2[0] = (byte)privilege;
            Array.Copy(salt, 0, salt2, 1, salt.Length);
			byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] salted = new byte[salt.Length + bytes.Length + 1];
            salt.CopyTo(salted, 0);
            salted[salt.Length] = (byte)privilege;
            bytes.CopyTo(salted, salt.Length + 1);

            SHA1 sha = SHA1.Create();
			var saltedHash = sha.ComputeHash(salted);

			return Convert.ToBase64String(saltedHash);
		}

		public static int FindByte(byte[] bytes, byte b, int len=0)
		{
            int size;
            size = (len <= 0 ? bytes.Length : len);
			for (int i=0; i<size; i++)
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

		public static void InitExceptionHandler()
		{
			//Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
		}

		private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			logger?.LogMessageEx("Fatal", "Program exception: {0}", e.ToString());
		}

		public static bool SleepOn(bool bval, int sleepInterval=100)
		{
			if (bval) Thread.Sleep(sleepInterval);

			return bval;
		}

		public static string GetUniqueName(string prefix="")
		{
			string name = prefix + rand.Next(99999);
			return name;
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
				relativePath = relativePath.Replace(global::System.IO.Path.AltDirectorySeparatorChar, global::System.IO.Path.DirectorySeparatorChar);
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

        public static UInt16 CalcCrcModbus(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;
            int pos;
            for (pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos]; // XOR byte into least sig. byte of crc
                int i;
                for (i = 8; i != 0; i--) // Loop over each bit
                {
                    if ((crc & 0x0001) != 0) // If the LSB is set
                    {
                        crc >>= 1; // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else  // Else LSB is not set
                        crc >>= 1; // Just shift right
                }
            }
            return crc;
        }

        public static void SetFocus(Form fm)
        {
            if (fm.WindowState == FormWindowState.Minimized)
            {
                fm.WindowState = FormWindowState.Normal;
                fm.Focus();
                fm.BringToFront();

                return;
            }

        }

        //public static object DeepClone(object obj)
        //{
        //    object objResult = null;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        bf.Serialize(ms, obj);

        //        ms.Position = 0;
        //        objResult = bf.Deserialize(ms);
        //    }
        //    return objResult;
        //}

        public static double Sqr(double x)
        {
            return x * x;
        }

        public static void SnapshotWindow(Form form, string id, string saveFolder)
        {
            global::System.Drawing.Rectangle bounds = form.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new global::System.Drawing.Point(bounds.Left, bounds.Top), global::System.Drawing.Point.Empty, bounds.Size);
                }

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                string dir_day = String.Format("{0}\\{1}", saveFolder, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(dir_day))
                {
                    Directory.CreateDirectory(dir_day);
                }
                string fn = string.Format("{0}\\{1}-{2}.png", dir_day, timestamp, id);
                bitmap.Save(fn, ImageFormat.Png);
            }

        }

        public static Bitmap InvertImage(Bitmap bmap)
        {
            Bitmap dst = new Bitmap(bmap.Width, bmap.Height);

            using (Graphics g = Graphics.FromImage(dst))
            {

                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {1, 1, 1, 0, 1}
                });

                ImageAttributes attributes = new ImageAttributes();

                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(bmap, new global::System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
                            0, 0, bmap.Width, bmap.Height, GraphicsUnit.Pixel, attributes);
            }

            return dst;
        }

        // FROM MSDN through StackOverflow
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        // FROM MSDN through StackOverflow
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(global::System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void Memset(byte[] bytes, int val)
        {
            Parallel.For(0, bytes.Length, i =>
            {
                bytes[i] = 0;
            });
        }

        public static int FindFirst(this byte[] arr, byte val)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == val) return i;
            }

            return -1;
        }

        public static int FindLast(this byte[] arr, byte val)
        {
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (arr[i] == val) return i;
            }

            return -1;
        }

        public static void FindLastTwo(this byte[] arr, byte val, int length, out int last1, out int last2)
        {
            last1 = -1; last2 = -1;
            int j = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                if (arr[i] == val)
                {
                    if (j++ == 0)
                    {
                        last1 = i;
                    }
                    else
                    {
                        last2 = i;
                        break;
                    }

                }
            }
        }

        public static void UShortToBytes(ushort us, byte[] arr, int offset)
        {
            arr[offset + 1] = (byte)(us>>8);
            arr[offset] = (byte)us;
        }

        public static void IntToBytes(int i, byte[] arr, int offset)
        {
            //arr[offset] = (byte)(i >> 24);
            //arr[offset + 1] = (byte)(i >> 16);
            //arr[offset + 2] = (byte)(i >> 8);
            //arr[offset + 3] = (byte)i;

            byte[] bytes = BitConverter.GetBytes(i);
            for (int j = 0; j < 4; j++) arr[offset + j] = bytes[j];
        }

        public static ushort BytesToUShort(byte[] arr, int offset=0)
        {
            ushort val;
            val = (ushort)(((ushort)arr[offset + 1]<<8) | (ushort)arr[offset]);
            return val;
        }

        public static short BytesToShort(byte[] arr, int offset = 0)
        {
            short val;
            val = BitConverter.ToInt16(arr, offset);
            return val;
        }

        public static int BytesToInt(byte[] arr, int offset)
        {
            int i = BitConverter.ToInt32(arr, offset);
            return i;
        }

        public static uint BytesToUInt(byte[] arr, int offset)
        {
            uint i = BitConverter.ToUInt32(arr, offset);
            return i;
        }

        public static float HexStrToFloat(string s)
        {
            uint num = uint.Parse(s, global::System.Globalization.NumberStyles.AllowHexSpecifier);

            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            return f;
        }

        public static double[] CsvToDoubleArray(string csv)
        {
            string[] s = csv.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            double[] d = new double[s.Length];
            for (int i=0; i<d.Length; i++)
            {
                double.TryParse(s[i], out d[i]);
            }
            return d;
        }

        public static string DoubleArrayToCsv(double[] d)
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i<d.Length; i++)
            {
                sb.AppendFormat(",", d[i].ToString());
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static List<string> CsvToStringList(string csv)
        {
            string[] list = csv.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            return list.ToList();
        }

        public static string StringListToCsv(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string s in list)
            {
                sb.AppendFormat("{0},", s);
            }
            sb.Remove(sb.Length-1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Serializes an object of the specified Type to a file.
        /// </summary>
        /// <typeparam name="T">The Type of the object to serialize</typeparam>
        /// <param name="xmlFilePath">The path to save the XML file to</param>
        /// <param name="objectToSerialize">The object instance to serialize</param>
        public static void SerializeToFile<T>(string xmlFilePath, T objectToSerialize) where T : class
        {
            using (var writer = new StreamWriter(xmlFilePath))
            {
                // Do this to avoid the serializer inserting default XML namespaces.
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                var serializer = new XmlSerializer(objectToSerialize.GetType());
                serializer.Serialize(writer, objectToSerialize, namespaces);
            }
        }

        /// <summary>
        /// Deserializes an XML file to the specified type of object.
        /// </summary>
        /// <typeparam name="T">The Type of object to deserialize</typeparam>
        /// <param name="xmlFilePath">The path to the XML file to deserialize</param>
        /// <returns>An object instance</returns>
        public static T DeserializeFromFile<T>(string xmlFilePath) where T : class
        {
            using (var reader = XmlReader.Create(xmlFilePath))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }

        public static T DeserializeFromText<T>(string xmlText) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var reader = new StringReader(xmlText))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static int Round(this float f)
        {
            return (int)Math.Round(f);
        }

        public static bool MatchPatternString(string s, string pat)
        {
            if (pat.Length != s.Length) return false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                char p = pat[i];

                if (Char.IsLetter(p))
                {
                    if (Char.IsUpper(p) && !Char.IsUpper(c)) return false;
                    else if (char.IsLower(p) && !Char.IsLower(c)) return false;
                }
                else if (char.IsDigit(p))
                {
                    if (!char.IsDigit(c)) return false;
                }
                else // other chars
                {
                    if (p != c) return false;
                }
            }

            return true;
        }

        public static void AddKeyDownEventToTextboxes(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox tb = (TextBox)ctrl;
                    if (!tb.ReadOnly)
                    {
                        tb.KeyDown += Tb_KeyDown;
                    }
                }
                else
                {
                    AddKeyDownEventToTextboxes(ctrl);
                }
            }
        }

        public static void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.BackColor = Color.LightCyan;

            if (!tb.Multiline && e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                //tb.SelectNextControl(tb, true, true, true, true);
                //tb.Focus();
                //tb.Select();
                //e.SuppressKeyPress = true;
            }
        }


        public static string UTF8ToAscii(string utf)
        {
            byte[] arr = Encoding.UTF8.GetBytes(utf);
            byte[] ascii = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, arr);

            string s = Encoding.UTF8.GetString(ascii);
            return s;
        }

        public static Image LoadImage(string path)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
                return Image.FromStream(ms);
        }

        public static bool ValidateEmailAddr(string email)
        {
            string s = email.Trim();
            if (s.EndsWith(".")) return false;

            try
            {
                var addr = new global::System.Net.Mail.MailAddress(s);
                return addr.Address == s;
            }
            catch
            {
                return false;
            }
        }

        public static string AesEncrypt(byte[] key, string msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;

                        byte[] iv = aes.IV;
                        ms.Write(iv, 0, iv.Length);

                        using (CryptoStream crypt = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter wr = new StreamWriter(crypt))
                            {
                                wr.WriteLine(msg);
                            }
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

        public static string AesDescrypt(byte[] key, string base64)
        {
            try
            {
                byte[] encrypted = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        ms.Read(iv, 0, iv.Length);

                        using (CryptoStream crypt = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                        {
                            using (StreamReader rd = new StreamReader(crypt))
                            {
                                string msg = rd.ReadToEnd();
                                return msg.Trim();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // return: array index 0 is available space
        //         array index 1 is total size of drive
        //
        public static long[] GetDiskSpace(string driveName)
        {
            long[] space = new long[2];
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach(DriveInfo drive in drives)
            {
                if (drive.Name.StartsWith(driveName))
                {
                    space[0] = drive.AvailableFreeSpace;
                    space[1] = drive.TotalSize;
                    return space;
                }
            }

            space[0] = -1;
            space[1] = -1;
            return space;
        }

        public static void SetBit(this ref uint word, int bitNum, bool val)
        {
            if (val)
            {
                word |= (1U << bitNum);
            }
            else
            {
                word &= ~(1U << bitNum);
            }
        }

        public static bool ReadBit(this uint word, int bitNum)
        {
            uint val = (word >> bitNum) & 0x01;
            return (val != 0);
        }

        public static void SetBit(this ref ushort word, int bitNum, bool val)
        {
            if (val)
            {
                word |= (ushort)(1U << bitNum);
            }
            else
            {
                word &= (ushort)~(1U << bitNum);
            }
        }

        public static bool ReadBit(this ushort word, int bitNum)
        {
            uint val = (uint)(word >> bitNum) & 0x01;
            return (val != 0);
        }
        
    }
}
