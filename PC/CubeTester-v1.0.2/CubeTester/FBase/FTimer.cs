using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class FTimer
    {
        protected Stopwatch stopwatch;
        long interval;
        long startTime;
        bool running;

        public FTimer(long interval = 0)
        {
            this.interval = interval;
            stopwatch = Util.stopwatch;
            running = false;
            startTime = 0;
        }

        public void Enable(bool enb)
        {
            if (enb)
            {
                if (!running)
                {
                    running = true;
                    startTime = stopwatch.ElapsedMilliseconds;
                }
            }
            else
            {
                running = false;
            }
        }

        public bool Done()
        {
            if (!running) return false;

            bool done = (stopwatch.ElapsedMilliseconds - startTime > interval);
            return done;
        }

        public void Start(long interval, double speedPct = 100)
        {
            if (speedPct == 100)
            {
                this.interval = interval;
            }
            else
            {
                this.interval = (long)(interval * 100.0 / speedPct);
            }
            startTime = stopwatch.ElapsedMilliseconds;
            running = true;
        }

        public void Stop()
        {
            running = false;
        }

        public bool Running()
        {
            return running;
        }

        public void Reset()
        {
            startTime = stopwatch.ElapsedMilliseconds;
        }

        public long ElapsedMilliseconds()
        {
            return stopwatch.ElapsedMilliseconds - startTime;
        }
    }

    public class GPulseTimer
    {
        protected Stopwatch time;
        long period;
        long halfPeriod;
        bool pulse;
        long ctr;

        public GPulseTimer(long period, bool startVal = false)
        {
            this.period = period;
            halfPeriod = period / 2;

            time = new Stopwatch();
            time.Start();
            pulse = startVal;
            ctr = 1;
        }

        public bool IsOn()
        {
            if (time.ElapsedMilliseconds > halfPeriod * ctr)
            {
                pulse = !pulse;
                ctr++;
            }

            return pulse;
        }

        public void SetPeriod(long period, bool startVal = false)
        {
            this.period = period;
            halfPeriod = period / 2;

            time.Restart();
            pulse = startVal;
            ctr = 1;
        }

        public void Reset()
        {
            time.Restart();
        }
    }
}