using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public class Point2D
    {
        public double[] pos;

        public Point2D()
        {
            pos = new double[2];
        }

        public Point2D(double x, double y)
        {
            pos = new double[2];
            pos[0] = x;
            pos[1] = y;
        }
    }

    public class Point3D
    {
        public double[] pos;

        public Point3D()
        {
            pos = new double[3];
        }
    }

    public class Pallet
    {
        public string[] axisName;
        List<Point2D> posList;
        int curIdx;
        bool maxIdxReached;

        public Pallet()
        {
            axisName = new string[2];
            posList = new List<Point2D>();
            curIdx = 0;
            maxIdxReached = false;
        }

        public void GenerateGrid(string axisX, string axisY, int cntX, int cntY, double intervalX, double intervalY, double offsetX=0, double offsetY=0, bool startAtCentre =  false)
        {
            axisName[0] = axisX;
            axisName[1] = axisY;

            double x = offsetX;
            double y = offsetY;

            posList.Clear();
            for (int i=0; i<cntX; i++)
            {
                y = 0;
                for (int j=0; j<cntY; j++)
                {
                    Point2D pt = new Point2D();
                    pt.pos[0] = x;
                    pt.pos[1] = y;

                    posList.Add(pt);

                    y += intervalY;
                }

                x += intervalX;
            }

            if (startAtCentre)
            {
                int ctrIdx = (int)(posList.Count / 2.0 + 0.5);

                Point2D p = posList[ctrIdx];
                posList.Remove(p);
                posList.Insert(0, p);
            }
        }

        public void SetAxisName(string axisX, string axisY)
        {
            axisName[0] = axisX;
            axisName[1] = axisY;
        }

        public void AddPos(double x, double y)
        {
            Point2D p = new Point2D(x, y);
            posList.Add(p);
        }

        public Point2D GetCurrentPos()
        {
            return posList[curIdx];
        }

        public int PalletPositions()
        {
            return posList.Count;
        }

        public void ClearIndex()
        {
            curIdx = 0;
            maxIdxReached = false;
        }

        public void IncrementIndex()
        {
            curIdx++;
            if (curIdx >= posList.Count)
            {
                curIdx = 0;
                maxIdxReached = true;
            }
        }

        public int GetIndex()
        {
            return curIdx + 1;
        }

        public bool SetIndex(int idx)
        {
            if (idx > 0 && idx <= posList.Count) curIdx = idx-1;
            else return false;

            return true;
        }

        public bool MaxIndexReached()
        {
            return maxIdxReached;
        }
    }


    public class Pallet3D
    {
        public string[] axisName;
        List<Point3D> posList;
        int curIdx;

        public Pallet3D()
        {
            axisName = new string[3];
            posList = new List<Point3D>();
            curIdx = 0;
        }

        public void GenerateGrid(string[] axes, int[] cnt, double[] interval, bool startAtCentre = false)
        {
            for (int i=0; i<axes.Length; i++)
            {
                axisName[i] = axes[i];
            }

            double x = 0;
            double y = 0;
            double z = 0;

            posList.Clear();
            for (int i = 0; i < cnt[0]; i++)
            {
                y = 0;
                for (int j = 0; j < cnt[1]; j++)
                {
                    z = 0;
                    for (int k = 0; k < cnt[2]; k++)
                    {
                        Point3D pt = new Point3D();
                        pt.pos[0] = x;
                        pt.pos[1] = y;
                        pt.pos[2] = z;
                        posList.Add(pt);

                        z += interval[2];
                    }


                    y += interval[1];
                }

                x += interval[0];
            }

            if (startAtCentre)
            {
                int ctrIdx = posList.Count / 2;

                Point3D p = posList[ctrIdx];
                for (int i = 0; i < posList.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        posList[i].pos[j] -= p.pos[j];
                    }
                }

                posList.Remove(p);
                posList.Insert(0, p);

            }
        }

        public Point3D GetCurrentPos()
        {
            return posList[curIdx];
        }

        public int PalletPositions()
        {
            return posList.Count;
        }

        public void ClearIndex()
        {
            curIdx = 0;
        }

        public void IncrementIndex()
        {
            curIdx++;
            if (curIdx >= posList.Count)
            {
                curIdx = 0;
            }
        }

        public int GetIndex()
        {
            return curIdx + 1;
        }

        public bool SetIndex(int idx)
        {
            if (idx >= 0 && idx < posList.Count) curIdx = idx - 1;
            else return false;

            return true;
        }
    }

}
