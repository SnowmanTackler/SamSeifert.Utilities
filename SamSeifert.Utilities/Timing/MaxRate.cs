using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Timing
{
    public class MaxRate
    {
        private DateTime _LastTime;
        public readonly float _Interval;

        public MaxRate(float interval = 1)
        {
            this._Interval = interval;
            this._LastTime = DateTime.Now;
        }

        public bool Update()
        {
            var new_time = DateTime.Now;

            if ((new_time - this._LastTime).TotalMilliseconds >= this._Interval)
            {
                this._LastTime = new_time;
                return true;
            }
            else return false;
        }
    }
}
