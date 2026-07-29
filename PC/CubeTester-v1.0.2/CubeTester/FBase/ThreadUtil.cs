using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBase
{
    public static class ThreadUtil
    {
        public static bool IsThreadRunning(Thread thr)
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
            thr.Priority = priority;
            thr.Start(par);

            return thr;
        }

        public static bool StopThread(Thread thr, int timeoutMs)
        {
            if (thr == null) return false;

            if (thr.IsAlive)
            {
                if (!thr.Join(timeoutMs))
                {
                    thr.Abort();
                    if (!thr.Join(timeoutMs))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
