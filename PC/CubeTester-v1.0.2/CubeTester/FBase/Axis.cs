using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace FBase
{
    public enum AxisType
    {
        Linear,
        Rotary,
        LinearGantry,
        LinearStepper,
        RotaryStepper
    }

    public enum MotionStatus
    {
        Unknown,
        Stop,
        Moving,
        Homing,
        Error
    }

    public class Axis
    {
        // parameters
        public string name;
        public string axisGroup;
        public int cardId;
        public int axisId;
        public AxisType type;

        // pulse per unit
        public double scale;
        public string unit;

        // homing parameters
        public bool homeDirReverse;
        public int homeMode;
        public double homeSpeed;
        public double homeAccel;
        public double homeOffset;

        // limits
        public double speedMax;
        public double accelMax;
        public double decelMax;
        public double posLimit;     // software limit
        public double negLimit;     // software limit

        // brake
        public int brakeOutput;

        // axis enable
        public bool enabled;

        // motor direction
        public int motorDir;

        // status
        public bool homingDone = false;
        public bool isGantry = false;

        // encoder scaling
        public double encoderScale = 1;

        // tolerance
        public double inposTol = 0.005;
        public const double defaultTol = 0.005;

        // disable interlock temporarily
        protected bool disableInterlock;

        public Axis()
        {
        }

        public static AxisType StringToAxisType(string s)
        {
            foreach (AxisType t in Enum.GetValues(typeof(AxisType)))
            {
                if (s.ToUpper() == t.ToString().ToUpper()) return t;
            }

            // default
            return AxisType.Linear;
        }

        public static string AxisTypeToString(AxisType t)
        {
            return t.ToString();
        }

        public virtual bool Init()
        {
            return true;
        }

        public virtual bool ServoOn(bool enb)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual void ReleaseBrake(bool enb)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool IsServoOn()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool AxisInterlock(double delta)
        {
            // interlocks

            return true;
        }

        public void DisableInterlock()
        {
            disableInterlock = true;
        }

        public void EnableInterlock()
        {
            disableInterlock = false;
        }

        public virtual bool MoveAbs(double pos, double speed)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool MoveAbs2(double pos, double speed)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool OnTheFlyChangePos(double pos)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool MoveRelative(double pos, double speed)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool Jog(double speed)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual double GetSpeed()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual double GetRpm()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool SetSpeed(double speed)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool SetAccelDecel(double accel, double decel)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual int GetMotionIoStatus()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual int GetMotionStatus()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool GetAlarmStatus()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual void ResetAlarms()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool StopMotion()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool Home()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool SetHomingType(int type)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual double GetPosition()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool GetPosition(out double pos)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool GetLimitSwitchStatus(out bool neglim, out bool poslim)
        {
            neglim = false; poslim = false;
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool GetLimitSwitchStatus(out bool neglim, out bool poslim, out bool neglimSw, out bool poslimSw)
        {
            neglim = false; poslim = false; neglimSw = false; poslimSw = false;
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool IsMotionDone()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool InPosition(double tol=defaultTol)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool AtTarget(double target, double tol)
        {
            if (!InPosition(tol)) return false;

            double pos;
            bool bStatus = GetPosition(out pos);
            if (!bStatus) return false;

            bool withinTarget = (pos < target + tol && pos > target - tol);
            return withinTarget;
        }

        public virtual bool Homed()
        {
            return homingDone;
        }

        public virtual bool Homing()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool Stopped()
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }

        public virtual bool SetPosition(double pos)
        {
            throw new NotImplementedException("Use Derived Class Instead");
        }
    }
}
