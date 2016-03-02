﻿using System;
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

        public override Sect Transpose()
        {
            var s = new SectArray(this._Type, this._Height, this._Width);

            s.unsetMinMaxAvg = this.unsetMinMaxAvg;
            s.unsetStd = this.unsetStd;
            s._std = this._std;
            s._min = this._min;
            s._max = this._max;
            s._avg = this._avg;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    s._Data[x, y] = this._Data[y, x];
                }
            }

            return s;
        }

        public override Size getPrefferedSize()
        {
            return new Size(this._Width, this._Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y">row</param>
        /// <param name="x">column</param>
        /// <returns></returns>
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










        public void NormalizeMax()
        {
            float max = float.MinValue;

            for (int i = 0; i < this._Height; i++)
                for (int j = 0; j < this._Width; j++)
                    max = Math.Max(max, this.Data[i, j]);

            for (int i = 0; i < this._Height; i++)
                for (int j = 0; j < this._Width; j++)
                    this.Data[i, j] /= max;
        }

        public void NormalizeSum()
        {
            Single sum = 0;

            for (int i = 0; i < this._Height; i++)
                for (int j = 0; j < this._Width; j++)
                    sum += this.Data[i, j];

            for (int i = 0; i < this._Height; i++)
                for (int j = 0; j < this._Width; j++)
                    this.Data[i, j] /= sum;
        }




        public static class Build
        {
            public static class Gaussian
            {
                private static SectArray G(SectType t, Single sigma, int w, int h)
                {
                    SectArray sa = new SectArray(t, w, h);
                    Single sum = 0;

                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        { 
                            Single x = j - (w - 1.0f) / 2;
                            Single y = i - (h - 1.0f) / 2;
                            Single val = (Single)Math.Pow(Math.E, -(x * x + y * y) / (2 * sigma));
                            sum += val;
                            sa.Data[i, j] = val;
                        }
                    }

                    return sa;
                }

                public static SectArray NormalizedSum1D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, 1, span);
                    sa.NormalizeSum();
                    return sa;
                }

                public static SectArray NormalizedMax1D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, 1, span);
                    sa.NormalizeMax();
                    return sa;
                }

                public static SectArray NormalizedSum2D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, span, span);
                    sa.NormalizeSum();
                    return sa;
                }

                public static SectArray NormalizedMax2D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, span, span);
                    sa.NormalizeMax();
                    return sa;
                }
            }
        }
    }
}
