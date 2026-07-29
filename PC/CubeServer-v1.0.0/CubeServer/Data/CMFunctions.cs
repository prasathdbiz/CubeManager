using CubeServer.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CubeServer.Data
{
    public static class CMFunctions
    {
        public static Quotation PdfToQuotation(string filename, string username)
        {
            Quotation q = null;
            int idx = 0;
            int idx1, idx2, idx3, idx4, idx5, idx6;
            string line;

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "pdftotext.exe";
                //p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
                p.StartInfo.Arguments = string.Format(@"-raw ""{0}"" -", filename);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                //string stderr = p.StandardError.ReadToEnd();
                string content = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                //File.WriteAllText(filename + ".txt", content);
                (idx1, idx2) = GetLineWithPat(content, "Quotation:", 0, out line);

                // get quotation number
                Regex rgx1 = new Regex(@"Quotation\:\s+(\d+)", RegexOptions.IgnoreCase);

                MatchCollection matches = rgx1.Matches(line);
                if (matches.Count == 0)
                {
                    Global.logger.LogMessageEx("Error", "Error Extracting Quotation Data: Quotation Number Not Found");
                    return null;
                }

                string quoNumStr = matches[0].Groups[1].Value;
                idx = idx2 + 1;

                // get bill-to-party
                (idx1, idx2) = GetLineWithPat(content, "Bill-to-party", idx, out line);
                (idx3, idx4) = GetLineWithPat(content, "Sold-to-party", idx2 + 1, out line);

                string billToParty = content.Substring(idx2 + 1, idx3 - idx2 - 1).Trim();

                (idx5, idx6) = GetLineWithPat(content, "Quotation info", idx4 + 1, out line);

                string soldToParty = content.Substring(idx4 + 1, idx5 - idx4 - 1).Trim();

                (idx1, idx2) = GetLineWithPat(content, "Quote no", idx6 + 1, out line);
                Regex rgx21 = new Regex(@"Quote no./date\s+(\d+)\s+/\s+([\d\.]+)", RegexOptions.IgnoreCase);
                matches = rgx21.Matches(line);
                string quoDate = "";
                if (matches.Count > 0) 
                { 
                    quoDate = matches[0].Groups[2].Value;
                }

                (idx1, idx2) = GetLineWithPat(content, "Customer number", idx2 + 1, out line);
                Regex rgx22 = new Regex(@"Customer number\s+(\d+)", RegexOptions.IgnoreCase);
                matches = rgx22.Matches(line);
                string custNum = "";
                if (matches.Count > 0)
                {
                    custNum = matches[0].Groups[1].Value;
                }

                (idx1, idx2) = GetLineWithPat(content, "Currency", idx2 + 1, out line);
                Regex rgx23 = new Regex(@"Currency\s+(\w+)", RegexOptions.IgnoreCase);
                matches = rgx23.Matches(line);
                string currency = "";
                if (matches.Count > 0)
                {
                    currency = matches[0].Groups[1].Value;
                }

                (idx1, idx2) = GetLineWithPat(content, "Attention to", idx2 + 1, out line);
                if (idx1 < 0)
                {
                    Global.logger.LogMessageEx("Error", "Error Extracting Quotation Data: \"Attention to\" Not Found");
                    return null;
                }

                string attnName = "";
                (idx1, idx2) = GetLineWithPat(content, "Name", idx2 + 1, out line);
                if (idx >= 0)
                {
                    Regex rgx2 = new Regex(@"Name\s+(.*)", RegexOptions.IgnoreCase);
                    matches = rgx2.Matches(line);
                    if (matches.Count > 0)
                    {
                        attnName = matches[0].Groups[1].Value.Trim();
                    }
                }

                string attnTel = "";
                (idx1, idx2) = GetLineWithPat(content, "Telephone", idx2 + 1, out line);
                if (idx1 >= 0)
                {
                    Regex rgx3 = new Regex(@"Telephone\s+(.*)", RegexOptions.IgnoreCase);
                    matches = rgx3.Matches(line);
                    if (matches.Count > 0)
                    {
                        attnTel = matches[0].Groups[1].Value.Trim();
                    }
                }

                string attnMobile = "";
                (idx1, idx2) = GetLineWithPat(content, "Mobile", idx2 + 1, out line);
                if (idx1 >= 0)
                {
                    Regex rgx4 = new Regex(@"Mobile\s+(.*)", RegexOptions.IgnoreCase);
                    matches = rgx4.Matches(line);
                    if (matches.Count > 0)
                    {
                        attnMobile = matches[0].Groups[1].Value.Trim();
                    }
                }

                string attnFax = "";
                (idx1, idx2) = GetLineWithPat(content, "Fax", idx2 + 1, out line);
                if (idx1 >= 0)
                {
                    Regex rgx5 = new Regex(@"Fax\s+(.*)", RegexOptions.IgnoreCase);
                    matches = rgx5.Matches(line);
                    if (matches.Count > 0)
                    {
                        attnFax = matches[0].Groups[1].Value.Trim();
                    }
                }

                string attnEmail = "";
                (idx1, idx2) = GetLineWithPat(content, "E-mail", idx2 + 1, out line);
                if (idx1 >= 0)
                {
                    Regex rgx6 = new Regex(@"E-mail\s+(.*)", RegexOptions.IgnoreCase);
                    matches = rgx6.Matches(line);
                    if (matches.Count > 0)
                    {
                        attnEmail = matches[0].Groups[1].Value.Trim();
                    }
                }

                (idx1, idx2) = GetLineWithPat(content, "Subject:", idx2 + 1, out line);
                if (idx1 < 0)
                {
                    Global.logger.LogMessageEx("Error", "Error Extracting Quotation Data: \"Subject:\" Not Found");
                    return null;
                }

                (idx3, idx4) = GetLineWithPat(content, "Quotation details", idx2 + 1, out line);
                if (idx3 < 0)
                {
                    Global.logger.LogMessageEx("Error", "Error Extracting Quotation Data: \"Quotation details\" Not Found");
                    return null;
                }

                string subject = content.Substring(idx2 + 1, idx3 - idx2 - 1);

                Regex rgx7 = new Regex(@"\$([\d\.]+)/\s*cube", RegexOptions.IgnoreCase);
                matches = rgx7.Matches(content, idx4 + 1);
                if (matches.Count == 0)
                {
                    Global.logger.LogMessageEx("Error", "Extracting Quotation Data: Price per cube not found");
                    return null;
                }

                string pricePerCube = matches[0].Groups[1].Value;

                (idx1, idx2) = GetLineWithPat(content, "Payment terms:", idx2 + 1, out line);
                if (idx1 < 0)
                {
                    Global.logger.LogMessageEx("Error", "Error Extracting Quotation Data: \"Payment terms:\" Not Found");
                    return null;
                }
                Regex rgx8 = new Regex(@"""(.+)""", RegexOptions.IgnoreCase);
                matches = rgx8.Matches(line);
                string paymentTerms = "";
                if (matches.Count > 0)
                {
                    paymentTerms = matches[0].Groups[1].Value;
                }

                q = new Quotation();
                int i;
                if (int.TryParse(quoNumStr, out i))
                {
                    q.QuoNum = i;
                }
                q.BillToParty = billToParty;
                q.SoldToParty = soldToParty;
                q.QuoDate = QuoDateToDateTime(quoDate);

                long l;
                if (long.TryParse(custNum, out l))
                {
                    q.CustomerNum = l;
                }

                q.Currency = currency;
                q.AttnName = attnName;
                q.AttnTel = attnTel;
                q.AttnMobile = attnMobile;
                q.AttnFax = attnFax;
                q.AttnEmail = attnEmail;
                q.QuoSubject = subject;
                
                double d;
                if (double.TryParse(pricePerCube, out d))
                {
                    q.Price = d;
                }

                q.PaymentTerms = paymentTerms;
                q.Confirmed = false;
                q.Filename = filename;
                q.UploadUser = username;
                q.Uploaded = DateTime.Now;
                q.LastUpdateUser = username;
                q.LastUpdate = q.Uploaded;

                return q;
            }
            catch(Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error converting pdf file \"{0}\" to Quotation: {1}", filename, ex.Message);
                return null;
            }
        }

        public static DateTime? QuoDateToDateTime(string date)
        {
            string[] s = date.Split(Global.dotSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length < 3) return null;
            string sdt = string.Format("{0}-{1}-{2}", s[2], s[1], s[0]);
            DateTime dt;
            if (DateTime.TryParse(sdt, out dt))
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        public static (int, int) GetLineWithPat(string txt, string pat, int start, out string line)
        {
            int idx1 = -1;
            int idx2 = -1;
            line = "";

            idx1 = txt.IndexOf(pat, start, StringComparison.OrdinalIgnoreCase);
            if (idx1 >= 0)
            {
                idx2 = txt.IndexOf("\n", idx1);
            }
            if (idx1 < 0 || idx2 < 0)
            {
                Global.logger.LogMessageEx("Error", "Extracting Quotation Data: Quotation Number Not Found");
                return (-1, -1);
            }
            line = txt.Substring(idx1, idx2 - idx1 + 1);

            return (idx1, idx2);
        }
    }
}
