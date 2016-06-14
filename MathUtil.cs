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

        public static void Clamp(float min, float max, float[] val)
        {
            for (int i = 0; i < val.Length; i++)
                val[i] = Clamp(min, max, val[i]);
        }


        public class Continuous360
        {
            private bool _First = true;
            private float _Angle = 0;
            private int _TurnCount = 0;

            public float UpdateAngle(float raw_angle)
            {
                float angle;
                do // 360 Continuos!
                {
                    angle = raw_angle + this._TurnCount * UnitConverter.PIF * 2;
                    float yawP1 = angle + UnitConverter.PIF * 2;
                    float yawM1 = angle - UnitConverter.PIF * 2;
                    if (Math.Abs(yawP1 - this._Angle) < Math.Abs(angle - this._Angle)) this._TurnCount++;
                    else if (Math.Abs(yawM1 - this._Angle) < Math.Abs(angle - this._Angle)) this._TurnCount--;
                    else break;
                } while (true);

                this._Angle = angle;
                return angle;
            }

        }


    }
}

