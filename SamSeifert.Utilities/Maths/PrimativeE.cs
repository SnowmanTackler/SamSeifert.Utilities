using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Maths
{
    /// <summary>
    /// Extensions for Primatives
    /// </summary>
    public static class PrimativeE
    {
        public static int ModGuaranteePositive(this int x, int mod)
        {
            return (x % mod + mod) % mod;
        }

        public static float ModGuaranteePositive(this float x, float mod)
        {
            return (x % mod + mod) % mod;
        }

        public static double ModGuaranteePositive(this double x, double mod)
        {
            return (x % mod + mod) % mod;
        }

        /// <summary>
        /// Returns mod X between min and max.  Helpful for convertering any angle to +- 180 degrees
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ModGuaranteeRange(this float x, float min, float max)
        {
            return min + ModGuaranteePositive(x - min, max - min);
        }
        public static double ModGuaranteeRange(this double x, double min, double max)
        {
            return min + ModGuaranteePositive(x - min, max - min);
        }

        /// <summary>
        /// Returns mod X between +/- lim.  Helpful for convertering any angle to +- 180 degrees
        /// </summary>
        /// <param name="x"></param>
        /// <param name="lim"></param>
        /// <returns></returns>
        public static float ModGuaranteeRangeAbs(this float x, float lim)
        {
            return ModGuaranteeRange(x, -lim, lim);
        }
        public static double ModGuaranteeRangeAbs(this double x, double lim)
        {
            return ModGuaranteeRange(x, -lim, lim);
        }



        public static int Clamp(this int val, int min, int max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clamp(this float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static double Clamp(this double val, double min, double max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static Decimal Clamp(this Decimal val, Decimal min, Decimal max)
        {
            return Math.Min(max, Math.Max(min, val));
        }




        public static byte ClampByte(this int val)
        {
            return (byte) Math.Min(255, Math.Max(0, val));
        }

        public static byte ClampByte(this float val)
        {
            return ClampByte((int)Math.Round(val));
        }

        public static byte ClampByte(this double val)
        {
            return ClampByte((int)Math.Round(val));
        }









        public static int ToThe(this int f, double power)
        {
            return (int)Math.Round(Math.Pow(f, power));
        }

        public static float ToThe(this float f, double power)
        {
            return (float) Math.Pow(f, power);
        }

        public static double ToThe(this double f, double power)
        {
            return Math.Pow(f, power);
        }



        public static int Squared(this int f)
        {
            return f.ToThe(2);
        }

        public static float Squared(this float f)
        {
            return f.ToThe(2);
        }

        public static double Squared(this double f)
        {
            return f.ToThe(2);
        }




        public static int SquareRoot(this int f)
        {
            return f.ToThe(0.5);
        }

        public static float SquareRoot(this float f)
        {
            return f.ToThe(0.5);
        }

        public static double SquareRoot(this double f)
        {
            return f.ToThe(0.5);
        }






        public static bool isInfinityOrNan(this float f)
        {
            return float.IsInfinity(f) || float.IsNaN(f);
        }

        public static bool isInfOrNan(this double f)
        {
            return double.IsInfinity(f) || double.IsNaN(f);
        }









        public static float Blend01(this float alpha, float value_at_0, float value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }

        public static double Blend01(this double alpha, double value_at_0, double value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }






        public static bool InRange(this int i, int low, int high)
        {
            return (i >= low) && (i < high);
        }

        public static bool InRange(this float i, float low, float high)
        {
            return (i >= low) && (i < high);
        }

        public static bool InRange(this double i, double low, double high)
        {
            return (i >= low) && (i < high);
        }





        public static int RoundToInt(this double d)
        {
            return (int)Math.Round(d);
        }

        public static int RoundToInt(this float f)
        {
            return (int)Math.Round(f);
        }






        public static void AssertEquals(this int has, int wants)
        {
            if (has != wants)
            {
                throw new Exception("Assertion failed. Want " + wants + " but have " + has);
            }
        }

    }
}

