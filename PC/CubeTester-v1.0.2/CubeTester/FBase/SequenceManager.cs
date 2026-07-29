using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBase
{
    public enum ProcessState
    {
        Startup,
        PowerOn,
        WaitForSafetyRelay,
        Idle,
        Initializing,
        ReadyToRun,
        Starting,
        Running,
        ManualRun,
        WaitingForOperator,
        Stopping,
        Stopped,
        Error,
        ResetError
    }

    public class SequenceManager
    {
        ProcessState lastState;
        ProcessState state;

        protected FLogger logger;

        public FTimer repTestDwellTimer; // repeatability test: dwell timer for waiting for operator mode
        const long repTestDwellTimeMs = 3000; // dwell for 3s

        bool stateChanged = false;

        object lckState = 1;

        public SequenceManager()
        {
            this.logger = Util.logger;

            state = ProcessState.Startup;

            repTestDwellTimer = new FTimer();
        }

        public void Shutdown()
        {
        }

        public ProcessState CurrentState()
        {
            return state;
        }

        public ProcessState LastState()
        {
            return lastState;
        }

        public void SetProcessState(ProcessState newState)
        {
            if (newState == state) return;

            lock (lckState)
            {
                lastState = state;
                state = newState;
                stateChanged = true;
            }
        }

        public bool StateChanged()
        {
            lock (lckState)
            {
                if (stateChanged)
                {
                    stateChanged = false;
                    return true;
                }

                return false;
            }
        }

        public static void Task(object obj)
        {
            SeqStatus status;

            Sequence seq = (Sequence)obj;
            seq.Init();

            do
            {
                status = seq.Run();
                if (status == SeqStatus.Done)
                {
                    break;
                }
                else if (status == SeqStatus.Error)
                {
                    string msg = string.Format("Error running sequence {0}.", seq.name);
                    Util.logger.LogMessage("Error", msg);
                    break;
                }

                Thread.Sleep(0);
            }
            while (status == SeqStatus.Running);
        }

        public Thread RunSequence(Sequence seq, ThreadPriority priority = ThreadPriority.Normal)
        {
            Thread thr = StartThread(Task, seq, seq.name, priority);
            return thr;
        }

        public Thread RunSequence(List<object> par)
        {
            if (par.Count < 1) return null;
            Sequence seq = (Sequence) par[0];
            Thread thr = StartThread(Task, par, seq.name);
            return thr;
        }

        public bool IsThreadRunning(Thread thr)
        {
            return (thr != null && thr.IsAlive);
        }

        public static Thread StartThread(ThreadStart task, string thrName = "", ThreadPriority priority = ThreadPriority.Normal)
        {
            Thread thr = new Thread(task);
            thr.Name = thrName;
            thr.SetApartmentState(ApartmentState.STA);
            thr.IsBackground = true;
            thr.Priority = priority;
            thr.Start();

            return thr;
        }
        public static Thread StartThread(ParameterizedThreadStart task, object par, string thrName = "", ThreadPriority priority = ThreadPriority.Normal)
        {
            Thread thr = new Thread(task);
            thr.Name = thrName;
            thr.SetApartmentState(ApartmentState.STA);
            thr.IsBackground = true;
            thr.Start(par);

            return thr;
        }

    }

}
