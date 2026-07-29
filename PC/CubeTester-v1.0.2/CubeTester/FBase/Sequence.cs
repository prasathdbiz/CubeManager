using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBase
{
    public enum SeqStatus
    {
        Idle,
        Running,
        Stopped,
        Done,
        Error,
        Init
    }

    public class SequenceData
    {
        public string seqName;
        public Sequence seq;
        public double speedPct = 100;
        public Teachpoint teachpoint;
        public TpSeqType tpSeqType;
        public string transformName;
        public int index;
        public bool useVision;
        public object customPar;
    }

    public class MainSequence : Sequence
    {
        public MainSequence(string nameid) : base(nameid)
        {
        }

        public virtual bool StartManualSequence(SequenceData data)
        {
            return true;
        }

        public virtual void StopManualSequence()
        {
        }

        public virtual long GetRepeatabilityTestRunTime()
        {
            return 0;
        }
    }

    public class Sequence
    {
        public string name;
        public SeqStatus status;
        public List<Sequence> sequences;

        public readonly object lckGUI = 1;

        public string[] reqCameras;
        public string[] reqVisionJobs;
        public string[] reqTeachpoints;
        public bool reqChecked = false;

        protected FTimer timer;
        protected static BaseApp app;
        protected static FLogger logger;
        protected static SequenceManager mgr;
        protected static AlarmManager alarmMgr;
        protected bool stop;
        protected double speedPct;

        public Sequence(string name)
        {
            this.name = name;
            status = SeqStatus.Idle;
            timer = new FTimer();

        }

        protected virtual void RegisterSequence(params Sequence[] seqs)
        {
            if (sequences == null) sequences = new List<Sequence>();

            foreach (Sequence s in seqs)
            {
                if (s == null)
                {
                    string msg = string.Format("Sequence for parent sequence {0} to be registered is null", this.name);
                    throw new ArgumentException(msg);
                }
                sequences.Add(s);
            }
        }

        public virtual void Init()
        {

        }

        public virtual void Init(double speedPct=100)
        {
            this.speedPct = speedPct;
        }

        public virtual void Continue()
        {
            this.status = SeqStatus.Running;
            if (sequences != null)
            {
                foreach (Sequence seq in sequences)
                {
                    if (seq is TeachpointSequence)
                    {
                        ((TeachpointSequence)seq).Continue();
                    }
                    else
                    {
                        seq.Continue();
                    }
                }
            }

            this.timer.Reset();
        }

        public virtual bool CheckRequirements()
        {
            if (reqChecked) return true;

            // program
            if (app == null)
            {
                alarmMgr.AddAlarm("System", "Configuration Not Loaded");
                return false;
            }

            // teachpoints
            if (reqTeachpoints != null)
            {
            }

            reqChecked = true;

            return true;
        }

        public virtual SeqStatus Run()
        {
            return SeqStatus.Done;
        }

        public virtual void Stop()
        {
            stop = true;
        }

        public virtual void Alarm(string location, string format, params object[] args)
        {
            alarmMgr.AddAlarm(location, format, args);
            status = SeqStatus.Error;
        }

    }

}
