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

        public float _StartingAngle { get; private set; }
        private float _Angle = 0;
        private int _TurnCount = 0;

        protected Continuous360(float inc, float starting_angle)
        {
            this._Increment = inc;
            this.SetAngle(starting_angle);
        }

        public static Continuous360 Radians(float starting_angle = 0)
        {
            return new Continuous360(UnitConverter.PIF * 2, starting_angle);
        }

        public static Continuous360 Degrees(float starting_angle = 0)
        {
            return new Continuous360(360, starting_angle);
        }

        /// <summary>
        /// Resets
        /// </summary>
        /// <param name="raw_angle"></param>
        public void SetAngle(float raw_angle)
        {
            this._StartingAngle = raw_angle;
            this._Angle = raw_angle;
            this._TurnCount = 0;
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
