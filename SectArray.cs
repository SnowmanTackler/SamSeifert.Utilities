using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.CSCV
{
    public class SectArray : Sect
    {
        public Single[,] Data { get { return this._Data; } }
        private readonly Single[,] _Data = null;
        private readonly int _Width;
        private readonly int _Height;

        private Boolean unsetMinMaxAvg = true;
        private Single _min, _max, _avg;
        private Boolean unsetStd = true;
        private Single _std;

        public override bool isSquishy()
        {
            return false;
        }

        public override Sect Clone()
        {
            var s = new SectArray(this._Type, this._Width, this._Height);
            this.CopyTo(s);
            return s;
        }

        public override Size getPrefferedSize()
        {
            return new Size(this._Width, this._Height);
        }

        public override Single this[int y, int x]
        {
            get
            {
                return this._Data[y, x];
            }
            set
            {
                this._Data[y, x] = value;
            }
        }

        public SectArray(SectType t, int w, int h)
            : this(new Single[h, w], t)
        {
        }

        public SectArray(Single[,] data, SectType t)
            : base(t)
        {
            this._Data = data;
            this._Height = data.GetLength(0);
            this._Width = data.GetLength(1);
        }

        internal void CopyTo(SectArray s)
        {
            s.unsetMinMaxAvg = this.unsetMinMaxAvg;
            s.unsetStd = this.unsetStd;
            s._std = this._std;
            s._min = this._min;
            s._max = this._max;
            s._avg = this._avg;
            Array.Copy(this._Data, s._Data, this._Width * this._Height);
        }

        public override Single getMinValue
        {
            get
            {
                if (this.unsetMinMaxAvg) this.statsSet(); 
                return this._min;
            }
        }

        public override Single getMaxValue
        {
            get
            {
                if (this.unsetMinMaxAvg) this.statsSet();
                return this._max;
            }
        }
        public override Single getAverageValue
        {
            get
            {
                if (this.unsetMinMaxAvg) this.statsSet();
                return this._avg;
            }
        }

        public override void Reset()
        {
            this.unsetMinMaxAvg = true;
            this.unsetStd = true;
        }

        private void statsSet()
        {
            this._min = Single.MaxValue;
            this._max = Single.MinValue;
            this._avg = 0;

            Single val = 0;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    val = this._Data[y, x];
                    this._min = Math.Min(val, this._min);
                    this._max = Math.Max(val, this._max);
                    this._avg += val;
                }
            }

            this._avg /= (this._Width * this._Height);

            this.unsetMinMaxAvg = false;
        }

        public Single getStandardDeviation
        {
            get
            {
                if (this.unsetStd)
                {
                    Single f = 0, st = 0;

                    for (int y = 0; y < this._Height; y++)
                    {
                        for (int x = 0; x < this._Width; x++)
                        {
                            f = this._Data[y, x] - this.getAverageValue;
                            st += (f * f);
                        }
                    }

                    st /= this._Height * this._Width;
                    this._std = (Single)Math.Sqrt(st);
                    this.unsetStd = false;
                }

                return this._std;
            }
        }
















        public static SectArray GaussianNormalizedSum(SectType t, Single sigma, int span)
        {
            SectArray sa = new SectArray(t, span, span);
            Single sum = 0;

            float center = (span - 1.0f) / 2;
            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    Single x = j - center;
                    Single y = i - center;
                    Single val = (Single)Math.Pow(Math.E, -(x * x + y * y) / (2 * sigma));
                    sum += val;
                    sa.Data[i, j] = val;
                }
            }

            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    sa.Data[i, j] /= sum;
                }
            }
            return sa;
        }

        public static SectArray GaussianNormalizedMax(SectType t, Single sigma, int span)
        {
            SectArray sa = new SectArray(t, span, span);
            Single max = 0;

            float center = (span - 1.0f) / 2;
            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    Single x = j - center;
                    Single y = i - center;
                    Single val = (Single)Math.Pow(Math.E, -(x * x + y * y) / (2 * sigma));
                    max = Math.Max(max, val);
                    sa.Data[i, j] = val;
                }
            }

            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    sa.Data[i, j] /= max;
                }
            }
            return sa;
        }
    }
}
