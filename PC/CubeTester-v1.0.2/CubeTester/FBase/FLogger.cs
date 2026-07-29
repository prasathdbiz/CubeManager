using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace FBase
{
    public class FLogMessage
    {
        public string datetime;
        public string domain;
        public string message;
    }

    public class FLogger
    {
        string[] sep;

        List<string> domains;
        Dictionary<FLogWriter, List<string>> logwriters;
        List<FLogMessage> buffer;

        public bool enableException { get; set; }

        public FLogger()
        {
            domains = new List<string>();
            logwriters = new Dictionary<FLogWriter, List<string>>();
            buffer = new List<FLogMessage>();
            sep = new string[2] { " ", "," };
        }

        public void AddDomain(string domain)
        {
            if (domains.Contains(domain)) return;

            domains.Add(domain);
        }

        public bool AddLogWriter(FLogWriter logwriter, string domainlist)
        {
            string[] list = domainlist.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in list)
            {
                if (!DomainIsRegistered(s)) return false;
            }

            foreach (string s in list)
            {
                if (logwriters.ContainsKey(logwriter))
                {
                    if (!logwriters[logwriter].Contains(s))
                    {
                        logwriters[logwriter].Add(s);
                    }
                }
                else
                {
                    List<string> domlist = new List<string>();
                    domlist.Add(s);
                    logwriters.Add(logwriter, domlist);
                }
            }

            return true;
        }

        public bool LogMessageEx(string domain, string format, params object[] arg)
        {
            string msg = string.Format(format, arg);
            return LogMessage(domain, msg);
        }

        public bool LogMessage(string domain, string msg, bool immediate = false)
        {
            if (!DomainIsRegistered(domain)) return false;

            FLogMessage logmsg = new FLogMessage();
            logmsg.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
            logmsg.domain = domain;
            logmsg.message = msg;

            foreach (FLogWriter w in logwriters.Keys)
            {
                if (logwriters[w].Contains(domain))
                {
                    w.LogMessage(logmsg);
                    if (immediate) w.FlushMessages();
                }
            }

            return true;
        }

        bool DomainIsRegistered(string domain)
        {
            if (!domains.Contains(domain))
            {
                if (enableException)
                {
                    throw new ArgumentException("Domain not found.", "domain");
                }
                else {
                    return false;
                }
            }

            return true;
        }

        public void FlushMessages()
        {
            foreach (FLogWriter w in logwriters.Keys)
            {
                w.FlushMessages();
            }
            Application.DoEvents();
        }

        public void Shutdown()
        {
            foreach (FLogWriter w in logwriters.Keys)
            {
                w.StopLogging();
            }
        }
    }

    public abstract class FLogWriter
    {
        protected static Mutex mutex = new Mutex();

        const int maxMessageCount = 1000;
        const int maxBufferSize = 10000; // bytes

        int loginterval;
        int bufferSize;
        global::System.Timers.Timer timer;
        bool enabled;

        protected List<FLogMessage> messages;
        public FLogWriter(int loginterval = 0)
        {
            this.loginterval = loginterval;
            messages = new List<FLogMessage>();
            timer = new global::System.Timers.Timer();
            bufferSize = 0;
            enabled = true;

            if (loginterval > 0)
            {
                timer.Interval = loginterval;
                timer.Elapsed += Timer_Elapsed;
                timer.Enabled = true;
            }
        }

        public void EnableLogging(bool enb)
        {
            enabled = enb;
            if (!enb)
            {
                FlushMessages();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FlushMessages();
        }

        public void LogMessage(FLogMessage msg)
        {
            mutex.WaitOne();
            messages.Add(msg);
            mutex.ReleaseMutex();

            bufferSize += msg.datetime.Length + msg.domain.Length + msg.message.Length;

            if (loginterval <= 0 || messages.Count > maxMessageCount)
            {
                FlushMessages();
            }
        }

        public void SetLogInterval(int interval)
        {
            loginterval = interval;
            timer.Interval = interval;
        }
        public void StopLogging()
        {
            timer.Enabled = false;
            FlushMessages();
        }

        public virtual void FlushMessages()
        {
            messages.Clear();
            bufferSize = 0;
        }
    }

    public class FLogFileWriter : FLogWriter
    {
        string filename;
        StreamWriter writer;

        public FLogFileWriter(string filename, int loginterval = 0) : base(loginterval)
        {
            this.filename = filename;
            string fn = string.Format("{0}-{1}.log", filename, DateTime.Now.ToString("yyyyMMdd"));

            string directory = global::System.IO.Path.GetDirectoryName(fn);
            if (!global::System.IO.Directory.Exists(directory))
                global::System.IO.Directory.CreateDirectory(directory);

            writer = new StreamWriter(fn, true);
        }

        public override void FlushMessages()
        {
            mutex.WaitOne();

            StringBuilder sb = new StringBuilder();
            foreach (FLogMessage msg in messages)
            {
                sb.AppendFormat("{0} [{1}] {2}\r\n", msg.datetime, msg.domain, msg.message);
            }

            if (sb.Length > 0)
            {
                writer.Write(sb.ToString());
                writer.Flush();
            }

            base.FlushMessages();

            mutex.ReleaseMutex();
        }
    }

    public class FLogTextBoxWriter : FLogWriter
    {
        const int maxLines = 1000;
        const int skipLines = 500;

        TextBox textbox;
        SynchronizationContext uicontext;
        bool logDateTime = true;

        public FLogTextBoxWriter(TextBox textbox, SynchronizationContext uicontext, int loginterval = 0) : base(loginterval)
        {
            this.textbox = textbox;
            this.uicontext = uicontext;
        }

        public void SetNoDateTime()
        {
            logDateTime = false;
        }

        public override void FlushMessages()
        {
            mutex.WaitOne();

            StringBuilder sb = new StringBuilder();
            foreach (FLogMessage msg in messages)
            {
                if (logDateTime) sb.AppendFormat("{0}  {1}\r\n", msg.datetime, msg.message);
                else sb.AppendFormat("{0}\r\n", msg.message);
            }

            if (sb.Length > 0)
            {
                uicontext.Post(this.OutputMessage, sb.ToString());
            }
            base.FlushMessages();

            mutex.ReleaseMutex();
        }

        public void OutputMessage(object obj)
        {
            textbox.Text += obj.ToString();
            textbox.Select(Math.Max(0, textbox.TextLength - 1), 1);
            textbox.ScrollToCaret();

            if (textbox.Lines.Length > maxLines)
            {
                var lines = textbox.Lines.Skip(skipLines);
                textbox.Lines = lines.ToArray();
            }
        }
    }
}
