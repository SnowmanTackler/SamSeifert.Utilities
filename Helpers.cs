using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    internal static class Helpers
    {

        /// <summary>
        /// Gets the linear estimate of y for an x value between 0 and 1
        /// </summary>
        /// <param name="y0">Value of function at x = 0</param>
        /// <param name="y1">Value of function at x = 1</param>
        /// <param name="x">X value between 0 and 1</param>
        /// <returns></returns>
        internal static float getLinearEstimate(float y0, float y1, float x)
        {
            return y0 + x * (y1 - y0);
        }

        internal static float Clamp(float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        internal static int Cast(float f)
        {
            return Math.Max(-255, Math.Min(255, (int)Math.Round(f, 0)));
        }

        internal static Byte castByte(float f)
        {
            return (Byte)Math.Max(0, Math.Min(255, f));
        }

        internal static float nextGaussian(Single std, Random rand)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return std * (float)randStdNormal;
            //            double randNormal = mean + stdDev * randStdNormal;
        }

        internal static float nextNormal(Single std, Random rand)
        {
            return std * (1 - 2 * ((float)(rand.NextDouble())));
        }


    }
}
