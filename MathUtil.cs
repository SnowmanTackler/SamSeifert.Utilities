using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class MathUtil
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




        public static int Clampp(this int val, int min, int max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clampp(this float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static double Clampp(this double val, double min, double max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static Decimal Clampp(this Decimal val, Decimal min, Decimal max)
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




        public static void Clamp(this int[] val, int min, int max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clampp(min, max);
        }

        public static void Clamp(this float[] val, float min, float max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clampp(min, max);
        }

        public static void Clamp(this double[] val, double min, double max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clampp(min, max);
        }





        public static bool isInfinityOrNan(this float f)
        {
            return float.IsInfinity(f) || float.IsNaN(f);
        }

        public static bool isInfOrNan(this double f)
        {
            return double.IsInfinity(f) || double.IsNaN(f);
        }





        public static int Min(params int[] values)
        {
            return values.Min();
        }

        public static float Min(params float[] values)
        {
            return values.Min();
        }

        public static double Min(params double[] values)
        {
            return values.Min();
        }

        public static int Max(params int[] values)
        {
            return values.Max();
        }

        public static float Max(params float[] values)
        {
            return values.Max();
        }

        public static double Max(params double[] values)
        {
            return values.Max();
        }







        public static float Blend01(this float alpha, float value_at_0, float value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }

        public static double Blend01(this double alpha, double value_at_0, double value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }









        public static int NumberOfNonZeros(this IEnumerable<int> nums)
        {
            int ret = 0;
            foreach (var num in nums)
                if (num != 0)
                    ret++;
            return ret;
        }

        public static int NumberOfNonZeros(this IEnumerable<float> nums)
        {
            int ret = 0;
            foreach (var num in nums)
                if (num != 0)
                    ret++;
            return ret;
        }
        public static int NumberOfNonZeros(this IEnumerable<double> nums)
        {
            int ret = 0;
            foreach (var num in nums)
                if (num != 0)
                    ret++;
            return ret;
        }



        /// <summary>
        /// Samples will be normalized to sum to 1, so do whatever you want!
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Entropy(this IEnumerable<int> a)
        {
            double sum = a.Sum();

            if (sum == 0) throw new Exception("No Input!");

            double entropy = 0;

            foreach (var i in a)
            {
                if (i == 0) continue;
                double p = i / sum;
                entropy -= p * Math.Log(p, 2);
            }

            return entropy;
        }


        /// <summary>
        /// Samples will be normalized to sum to 1, so do whatever you want!
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Entropy(this IEnumerable<float> a)
        {
            double sum = a.Sum();

            if (sum == 0) throw new Exception("No Input!");

            double entropy = 0;

            foreach (var i in a)
            {
                if (i == 0) continue;
                double p = i / sum;
                entropy -= p * Math.Log(p, 2);
            }

            return entropy;
        }
    }
}

