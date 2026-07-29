using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class AxisGroup
    {
        public string name;
        public string[] axisList;
        public Dictionary<string, Axis> axes;

        public AxisGroup()
        {
            axes = new Dictionary<string, Axis>();
        }

        public bool DownloadAxisConfig()
        {
            foreach (Axis ax in axes.Values)
            {
                if (!ax.enabled) continue;

                if (!ax.Init())
                {
                    string msg = string.Format("Error downloading to axis {0}:{1}", name, ax.name);
                    Util.ErrorMessageBox(msg, "Download Axis Config");
                    return false;
                }
            }

            return true;
        }

        public bool ServoOn(bool enb)
        {
            foreach (Axis ax in axes.Values)
            {
                if (!ax.ServoOn(enb)) return false;
            }

            return true;
        }

        public bool AllServoOn()
        {
            foreach (Axis ax in axes.Values)
            {
                if (!ax.IsServoOn()) return false;
            }

            return true;
        }

        public bool Stop()
        {
            bool status = true;
            foreach (Axis ax in axes.Values)
            {
                status &= ax.StopMotion();
            }

            return status;
        }

        public bool MotionDone()
        {
            foreach (Axis ax in axes.Values)
            {
                if (!ax.IsMotionDone() || !ax.InPosition()) return false;
            }

            return true;
        }

        public bool AllAxisHomed()
        {
            foreach (Axis ax in axes.Values)
            {
                if (!ax.homingDone) return false;
            }

            return true;
        }

        public bool AtTeachpoint(Teachpoint tp, double tol)
        {
            if (tp.axisGroup != this) return false;

            for (int i=0; i<tp.axisCount; i++)
            {
                if (!axes.ContainsKey(tp.axisName[i])) return false;

                Axis axis = axes[tp.axisName[i]];
                if (!axis.AtTarget(tp.pos[i], tol)) return false;
            }

            return true;
        }

        public bool MoveRelative(Dictionary<string, double> dist, Teachpoint tp)
        {
            for (int i=0; i<axes.Count; i++)
            {
                string ax = axisList[i];
                if (dist.ContainsKey(ax))
                {
                    if (!axes[ax].MoveRelative(dist[ax], ax == "R" ? tp.rotaryPar.speed : tp.nominalPar.speed))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
