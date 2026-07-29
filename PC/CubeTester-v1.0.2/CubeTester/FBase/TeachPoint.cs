using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class MotionPar
    {
        public double height = 0;
        public double speed = 10;
        public double accel = 100;
        public double decel = 100;

        public MotionPar()
        {

        }

        public MotionPar(MotionPar par)
        {
            height = par.height;
            speed = par.speed;
            accel = par.accel;
            decel = par.decel;
        }

        public string ToText()
        {
            string s = string.Format("{0},{1},{2},{3}", height, speed, accel, decel);
            return s;
        }

        public void FromText(string s)
        {
            string[] t = s.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            //if (t.Length != 4) return false;
            height = Convert.ToDouble(t[0]);
            speed = Convert.ToDouble(t[1]);
            accel = Convert.ToDouble(t[2]);
            decel = Convert.ToDouble(t[3]);
        }
    }

    public class Teachpoint
    {
        public string name;
        public string axes;
        public AxisGroup axisGroup;
        public string desc;
        public string[] axisName;
        public double[] pos;
        public int[] moveOrder;
        public MotionPar nominalPar;
        public MotionPar approachPar;
        public MotionPar departPar;
        public MotionPar rotaryPar;

        public Pallet pallet;
        public Pallet3D pallet3D;
        public int curPalletIdx;

        // calculated
        public int axisCount;

        // delta
        public double[] delta;

        public Teachpoint()
        {
            nominalPar = new MotionPar();
            approachPar = new MotionPar();
            departPar = new MotionPar();
            rotaryPar = new MotionPar();
        }

        public Teachpoint(string name, Teachpoint tp)
        {
            this.name = name;
            axes = string.Copy(tp.axes);
            axisGroup = tp.axisGroup;
            desc = string.Copy(tp.desc);

            axisName = new string[tp.axisName.Length];
            Array.Copy(tp.axisName, axisName, tp.axisName.Length);

            pos = new double[tp.pos.Length];
            Array.Copy(tp.pos, pos, tp.pos.Length);

            moveOrder = new int[tp.moveOrder.Length];
            Array.Copy(tp.moveOrder, moveOrder, tp.moveOrder.Length);

            nominalPar = new MotionPar(tp.nominalPar);
            approachPar = new MotionPar(tp.approachPar);
            departPar = new MotionPar(tp.departPar);
            rotaryPar = new MotionPar(tp.rotaryPar);

            pallet = tp.pallet;
            pallet3D = tp.pallet3D;
            axisCount = tp.axisCount;
            curPalletIdx = 1;
        }

        public void AttachPallet(Pallet pallet)
        {
            this.pallet = pallet;
        }

        public void AttachPallet3D(Pallet3D pallet)
        {
            this.pallet3D = pallet;
        }

        public bool SetAxes(string axes)
        {
            this.axes = axes;

            axisName = axes.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            axisCount = axisName.Length;
            if (axisCount <= 0) return false;

            pos = new double[axisCount];
            delta = new double[axisCount];
            moveOrder = new int[axisCount];

            for (int i=0; i<axisCount; i++)
            {
                moveOrder[i] = 1;
            }

            return true;
        }

        public void SetDelta(string axisName, double delta)
        {
            for (int i=0; i<axisCount; i++)
            {
                if (axisName == this.axisName[i])
                {
                    this.delta[i] = delta;
                    break;
                }
            }
        }

        public void ResetDeltas()
        {
            for (int i=0; i<axisCount; i++)
            {
                this.delta[i] = 0;
            }
        }

        public bool GetAxisPosition(string axis, out double pos)
        {
            pos = 0;

            for (int i=0; i<axisCount; i++)
            {
                if (axisName[i] == axis)
                {
                    pos = this.pos[i];
                    return true;
                }
            }

            return false;
        }

        public double GetLinearSpeed()
        {
            return nominalPar.speed;
        }

        public double GetRotarySpeed()
        {
            return rotaryPar.speed;
        }

        public bool SetPosition(string posStr)
        {
            string[] p = posStr.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length != axisCount) return false;

            for (int i=0; i<axisCount; i++)
            {
                if (!double.TryParse(p[i], out pos[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool SetMoveOrder(string order)
        {
            string[] s = order.Split(Util.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length != axisCount) return false;

            for (int i = 0; i < axisCount; i++)
            {
                if (!int.TryParse(s[i], out moveOrder[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetPositionString()
        {
            string s = "";
            for (int i=0; i<pos.Length; i++)
            {
                s += string.Format("{0},", pos[i].ToString(Util.positionFormat));
            }

            s = s.Remove(s.Length - 1); // remove last comma
            return s;
        }

        public string GetMoveOrderString()
        {
            string s = "";
            for (int i = 0; i < pos.Length; i++)
            {
                s += string.Format("{0},", moveOrder[i]);
            }

            s = s.Remove(s.Length - 1); // remove last comma
            return s;
        }

        public bool DistanceFrom(Teachpoint reftp, Dictionary<string, double> dist)
        {
            if (this.axisGroup != reftp.axisGroup) return false;

            dist.Clear();
            for (int i=0; i<axisCount; i++)
            {
                dist.Add(axisName[i], pos[i] - reftp.pos[i]);
            }

            return true;
        }
    }
}
