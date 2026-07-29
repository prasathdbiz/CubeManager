using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public enum TpSeqType
    {
        Approach,
        MoveTo,
        Depart
    }

    public class TeachpointSequence : Sequence
    {
        enum SeqState
        {
            Start,
            MoveAxis,
            WaitAxis,
            End
        }

        public double posTol = 0.005;
        public double rotPosTol = 0.1;


        SeqState curState, nextState;
        Teachpoint tp;
        int moveIdx;
        double speed, accel, decel;
        Dictionary<int, Axis> axisList;
        TpSeqType seqtype;
        MotionPar motionPar;
        double[] targetPos;

        public TeachpointSequence() : base("TeachpointSequence")
        {
            axisList = new Dictionary<int, Axis>();
        }

        public bool Init(Teachpoint tp, TpSeqType seqtype, double speedPct = 100)
        {
            this.tp = tp;
            this.seqtype = seqtype;
            this.speedPct = speedPct;

            stop = false;

            switch (seqtype)
            {
                case TpSeqType.Approach:
                    motionPar = tp.approachPar;
                    break;

                case TpSeqType.MoveTo:
                    motionPar = tp.nominalPar;
                    break;

                case TpSeqType.Depart:
                    motionPar = tp.departPar;
                    break;
            }

            targetPos = new double[tp.axisCount];

            curState = SeqState.Start;
            nextState = SeqState.Start;
            return true;
        }

        public void HandleStop()
        {
            if (stop)
            {
                nextState = SeqState.End;
                tp.axisGroup.Stop();
                stop = false;
            }

        }

        public override SeqStatus Run()
        {
            HandleStop();

            curState = nextState;

            switch (curState)
            {
                case SeqState.Start:
                    status = SeqStatus.Running;

                    moveIdx = 1;

                    if (!tp.axisGroup.AllAxisHomed())
                    {
                        Alarm(tp.axisGroup.name, "Axes for axis group {0} not homed", tp.axisGroup.name);
                        break;
                    }

                    if (!tp.axisGroup.ServoOn(true))
                    {
                        Alarm(tp.axisGroup.name, "Error enabling axes for axis group {0}.", tp.axisGroup.name);
                        break;
                    }

                    nextState = SeqState.MoveAxis;
                    break;

                case SeqState.MoveAxis:
                    if (moveIdx <= tp.axisCount)
                    {
                        axisList.Clear();
                        for (int i = 0; i < tp.axisCount; i++)
                        {
                            if (tp.moveOrder[i] == moveIdx)
                            {
                                Axis axis = tp.axisGroup.axes[tp.axisName[i]];

                                if (axis.type == AxisType.Linear || axis.type == AxisType.LinearGantry || axis.type == AxisType.LinearStepper)
                                {
                                    speed = motionPar.speed * speedPct / 100.0;
                                    accel = motionPar.accel;
                                    decel = motionPar.decel;
                                }
                                else if (axis.type == AxisType.Rotary || axis.type == AxisType.RotaryStepper)
                                {
                                    speed = tp.rotaryPar.speed * speedPct / 100.0;
                                    accel = tp.rotaryPar.accel;
                                    decel = tp.rotaryPar.decel;
                                }

                                if (!axis.SetAccelDecel(accel, decel))
                                {
                                    Alarm(tp.axisGroup.name, "Unable to set accel and decel for axis {0}", axis.name);
                                    break;
                                }

                                targetPos[i] = tp.pos[i];
                                if (axis.name.Contains("Z")) // approach, depart
                                {
                                    targetPos[i] += motionPar.height;
                                }

                                if (tp.pallet != null)
                                {
                                    Point2D pt = tp.pallet.GetCurrentPos();
                                    for (int j = 0; j < 2; j++)
                                    {
                                        if (axis.name == tp.pallet.axisName[j])
                                        {
                                            targetPos[i] += pt.pos[j];
                                        }
                                    }
                                }
                                else if (tp.pallet3D != null)
                                {
                                    Point3D pt = tp.pallet3D.GetCurrentPos();
                                    for (int j=0; j<3; j++)
                                    {
                                        if (axis.name == tp.pallet3D.axisName[j])
                                        {
                                            targetPos[i] += pt.pos[j];
                                        }
                                    }
                                }
                                else if (seqtype == TpSeqType.MoveTo)
                                {
                                    targetPos[i] += tp.delta[i];
                                }

                                if (!axis.MoveAbs(targetPos[i], speed))
                                {
                                    Alarm(tp.axisGroup.name, "Unable to move to teachpoint position for axis {0}", axis.name);
                                    break;
                                }

                                axisList.Add(i, axis);
                            }
                        }

                        timer.Start(30000, speedPct);
                        nextState = SeqState.WaitAxis;
                    }
                    else
                    {
                        nextState = SeqState.End;
                    }
                    break;

                case SeqState.WaitAxis:
                    bool inpos = true;

                    foreach (int i in axisList.Keys)
                    {
                        Axis axis = axisList[i];
                        double tol = (axis.type == AxisType.Rotary || axis.type == AxisType.RotaryStepper) ? rotPosTol : posTol;

                        inpos &= axis.AtTarget(targetPos[i], tol);
                    }

                    if (inpos)
                    {
                        moveIdx++;
                        if (moveIdx > tp.axisCount)
                        {
                            nextState = SeqState.End;
                        }
                        else
                        {
                            nextState = SeqState.MoveAxis;
                        }
                    }
                    else if (timer.Done())
                    {
                        Alarm(tp.axisGroup.name, "Timeout waiting for teachpoint to complete move.");
                    }
                    break;

                case SeqState.End:
                    status = SeqStatus.Done;
                    break;
            }

            return status;
        }

        public override void Continue()
        {
            base.Continue();

            nextState = SeqState.Start; // restart teachpoint
        }
    }

    public class ApproachMoveToSequence : Sequence
    {
        enum SeqState
        {
            Start,
            Approach,
            MoveTo,
            End
        }

        SeqState curState, nextState;
        Teachpoint tp;
        TeachpointSequence seqTeachpoint;

        public ApproachMoveToSequence() : base("ApproachAndMoveToTeachpoint")
        {
            seqTeachpoint = new TeachpointSequence();

            RegisterSequence(seqTeachpoint);
        }

        public bool Init(Teachpoint tp, double speedPct)
        {
            this.tp = tp;
            this.speedPct = speedPct;

            curState = SeqState.Start;
            nextState = SeqState.Start;
            return true;
        }

        public override SeqStatus Run()
        {
            SeqStatus seqStatus;
            curState = nextState;

            switch (curState)
            {
                case SeqState.Start:
                    status = SeqStatus.Running;

                    seqTeachpoint.Init(tp, TpSeqType.Approach, speedPct);
                    nextState = SeqState.Approach;
                    break;

                case SeqState.Approach:
                    seqStatus = seqTeachpoint.Run();
                    if (seqStatus == SeqStatus.Done)
                    {
                        seqTeachpoint.Init(tp, TpSeqType.MoveTo, speedPct);
                        nextState = SeqState.MoveTo;
                    }
                    else if (seqStatus == SeqStatus.Error)
                    {
                        Alarm("System", "Error approaching teachpoint {0}", tp.name);
                    }
                    break;

                case SeqState.MoveTo:
                    seqStatus = seqTeachpoint.Run();
                    if (seqStatus == SeqStatus.Done)
                    {
                        nextState = SeqState.End;
                    }
                    else if (seqStatus == SeqStatus.Error)
                    {
                        Alarm("System", "Error moving to teachpoint {0}", tp.name);
                    }
                    break;

                case SeqState.End:
                    status = SeqStatus.Done;
                    break;
            }

            return status;
        }
    }

    public class ApproachSequence : TeachpointSequence
    {
        public ApproachSequence()
        {

        }

        public void Init(Teachpoint tp, double speedPct)
        {
            base.Init(tp, TpSeqType.Approach, speedPct);
        }
    }

    public class MoveToSequence : TeachpointSequence
    {
        public MoveToSequence()
        {

        }

        public void Init(Teachpoint tp, double speedPct)
        {
            base.Init(tp, TpSeqType.MoveTo, speedPct);
        }
    }

    public class DepartSequence : TeachpointSequence
    {
        public DepartSequence()
        {

        }

        public void Init(Teachpoint tp, double speedPct)
        {
            base.Init(tp, TpSeqType.Depart, speedPct);
        }
    }
}
