using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class RateTracker
    {
        private float _FPS = 0;
        private bool _First = true;
        private bool _Second = true;
        private DateTime _Last = DateTime.Now;

        int count = 0;

        public void Update(System.Windows.Forms.Label l)
        {
            l.Text = this.Update().ToString("00.00") + " Hz";
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

            if (elapsed < 0.0001f) return 100000; // Can't track more than 10,000 Hz

            float fps = 1 / elapsed;

            if (this._Second)
            {
                this._FPS = fps;
                this._Second = false;
            }
            else
            {
                const float alpha = 0.1f;
                this._FPS *= (1 - alpha);
                this._FPS += alpha * fps;
            }


            /// The problem with this is approach is, FPS won't change if the Update() method is never called.
            /// So, have the value always change slightly when it is updating to indicate a live feed.
            switch (count++ % 3)
            {
                case 1:
                    return this._FPS + 0.01f;
                case 2:
                    return this._FPS - 0.01f;
                default:
                    return this._FPS;
            }
        }
    }
}
