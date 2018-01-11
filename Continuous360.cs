using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class Continuous360
    {
        protected readonly float _Increment;

        private float _Angle = 0;
        private int _TurnCount = 0;

        protected Continuous360(float inc)
        {
            this._Increment = inc;
        }

        public static Continuous360 Radians()
        {
            return new Continuous360(UnitConverter.PIF * 2);
        }

        public static Continuous360 Degrees()
        {
            return new Continuous360(360);
        }

        public void SetAngle(float raw_angle)
        {
            this._Angle = raw_angle;
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
}
