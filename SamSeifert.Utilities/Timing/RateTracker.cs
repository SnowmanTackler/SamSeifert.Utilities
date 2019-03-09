using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Timing
{
    public class RateTracker
    {
        private float _AverageElapsed = 0;
        private bool _First = true;
        private bool _Second = true;
        private DateTime _Last = DateTime.Now;
        private readonly float _Alpha;

        int _VisbibleChangeCount = 0;

        public RateTracker(float alpha = 0.1f)
        {
            this._Alpha = alpha;
        }

        public float Update()
        {
            var now = DateTime.Now;
            var elapsed = (float)((now - this._Last).TotalSeconds);
            this._Last = now;

            if (this._First)
            {
                this._First = false;
                return 0;
            }

            if (this._Second)
            {
                this._AverageElapsed = elapsed;
                this._Second = false;
            }
            else
            {
                this._AverageElapsed *= (1 - this._Alpha);
                this._AverageElapsed += this._Alpha * elapsed;
            }

            if (this._AverageElapsed < 0.0001f) return -1; // Can't track more than 10,000 Hz

            float fps = 1 / this._AverageElapsed;


            /// The problem with this is approach is, FPS won't change if the Update() method is never called.
            /// So, have the value always change slightly when it is updating to indicate a live feed.
            switch (_VisbibleChangeCount++ % 3)
            {
                case 1:
                    return fps + 0.01f;
                case 2:
                    return fps - 0.01f;
                default:
                    return fps;
            }
        }
    }
}
