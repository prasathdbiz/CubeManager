using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase
{
    public static class MathUtil
    {
        public const double degToRad = Math.PI / 180.0;
        public const double radToDeg = 180.0 / Math.PI;

        public static double Sqr(double x)
        {
            return x * x;
        }

        public static double[] GenerateLinear(double start, double end, double spacing)
        {
            int i = 0;
            List<double> list = new List<double>();

            double val = start;
            while (val <= end)
            {
                list.Add(val);
                i++;
                val = start + i * spacing;
            }

            return list.ToArray();
        }
    }
}
