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





        public static int Squared(this int f)
        {
            return f * f;
        }

        public static float Squared(this float f)
        {
            return f * f;
        }

        public static double Squared(this double f)
        {
            return f * f;
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







        public class Continuous360_Radians
        {
            protected readonly float _Increment;

            private bool _First = true;
            private float _Angle = 0;
            private int _TurnCount = 0;

            protected Continuous360_Radians(float inc)
            {
                this._Increment = inc;
            }

            public Continuous360_Radians() : this(UnitConverter.PIF * 2)
            {
            }

            public float UpdateAngle(float raw_angle)
            {
                float angle;

                while (true)
                {
                    angle = raw_angle + this._TurnCount * this._Increment;
                    float yawP1 = angle + this._Increment;
                    float yawM1 = angle - this._Increment;
                    if (Math.Abs(yawP1 - this._Angle) < Math.Abs(angle - this._Angle)) this._TurnCount++;
                    else if (Math.Abs(yawM1 - this._Angle) < Math.Abs(angle - this._Angle)) this._TurnCount--;
                    else break;
                }

                this._Angle = angle;
                return angle;
            }
        }

        public class Continuous360 : Continuous360_Radians
        {
            public Continuous360() : base(360)
            {
            }
        }
    }
}

