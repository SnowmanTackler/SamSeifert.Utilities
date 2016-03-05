using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class MathUtil
    {
        public static int ModGuaranteePositive(int x, int mod)
        {
            return (x % mod + mod) % mod;
        }

        public static float ModGuaranteePositive(float x, float mod)
        {
            return (x % mod + mod) % mod;
        }

        public static double ModGuaranteePositive(double x, double mod)
        {
            return (x % mod + mod) % mod;
        }


        public static float Clamp(int min, int max, int val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clamp(float min, float max, float val)
        {
            return Math.Min(max, Math.Max(min, val));
        }





    }
}

