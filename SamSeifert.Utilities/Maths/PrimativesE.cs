using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Maths
{
    /// <summary>
    /// Extension functions for groups of Primatives
    /// </summary>
    public static class PrimativesE
    {
        public static void Clamp(this int[] val, int min, int max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clamp(min, max);
        }

        public static void Clamp(this float[] val, float min, float max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clamp(min, max);
        }

        public static void Clamp(this double[] val, double min, double max)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = val[i].Clamp(min, max);
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
    }
}
