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



        public static int Clamp(int min, int max, int val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clamp(float min, float max, float val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static double Clamp(double min, double max, double val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static Decimal Clamp(Decimal min, Decimal max, Decimal val)
        {
            return Math.Min(max, Math.Max(min, val));
        }




        public static byte ClampByte(int val)
        {
            return (byte) Math.Min(255, Math.Max(0, val));
        }

        public static byte ClampByte(float val)
        {
            return ClampByte((int)Math.Round(val));
        }







        public static void Clamp(float min, float max, float[] val)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = Clamp(min, max, val[i]);
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

