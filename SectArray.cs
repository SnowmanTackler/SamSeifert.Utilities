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

        public void SetValue(float val)
        {
            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] = val;
                }
            }
        }

        public override Sect Transpose()
        {
            var s = new SectArray(this._Type, this._Height, this._Width);

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
            Array.Copy(this._Data, s._Data, this._Width * this._Height);
        }

        public override Single getMinValue
        {
            get
            {
                float tracker = Single.MaxValue;

                for (int y = 0; y < this._Height; y++)
                    for (int x = 0; x < this._Width; x++)
                        tracker = Math.Min(this._Data[y, x], tracker);

                return tracker;
            }
        }

        public override Single getMaxValue
        {
            get
            {
                float tracker = Single.MinValue;

                for (int y = 0; y < this._Height; y++)
                    for (int x = 0; x < this._Width; x++)
                        tracker = Math.Max(this._Data[y, x], tracker);

                return tracker;
            }
        }
        public override Single getAverageValue
        {
            get
            {
                float tracker = 0;

                for (int y = 0; y < this._Height; y++)
                    for (int x = 0; x < this._Width; x++)
                        tracker += this._Data[y, x];

                return tracker / (this._Width * this._Height);
            }
        }

        public Single getStandardDeviation
        {
            get
            {
                float average = this.getAverageValue;

                Single f = 0, st = 0;

                for (int y = 0; y < this._Height; y++)
                {
                    for (int x = 0; x < this._Width; x++)
                    {
                        f = this._Data[y, x] - average;
                        st += (f * f);
                    }
                }

                st /= this._Height * this._Width;
                return (Single)Math.Sqrt(st);
            }
        }







        /// <summary>
        /// ONLY USE FOR +=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectArray operator + (SectArray left, float right)
        {
            for (int i = 0; i < left._Height; i++)
                for (int j = 0; j < left._Width; j++)
                    left.Data[i, j] += right;

            return left;
        }

        /// <summary>
        /// ONLY USE FOR -=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectArray operator -(SectArray left, float right)
        {
            for (int i = 0; i < left._Height; i++)
                for (int j = 0; j < left._Width; j++)
                    left.Data[i, j] -= right;

            return left;
        }

        /// <summary>
        /// ONLY USE FOR /=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectArray operator /(SectArray left, float right)
        {
            for (int i = 0; i < left._Height; i++)
                for (int j = 0; j < left._Width; j++)
                    left.Data[i, j] /= right;

            return left;
        }

        /// <summary>
        /// ONLY USE FOR *=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectArray operator *(SectArray left, float right)
        {
            for (int i = 0; i < left._Height; i++)
                for (int j = 0; j < left._Width; j++)
                    left.Data[i, j] *= right;

            return left;
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
            public static SectArray FromArray(SectType t, int w, int h, IEnumerable<float> f)
            {
                SectArray sa = new SectArray(t, w, h);

                using (var en = f.GetEnumerator())
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            if (en.MoveNext()) sa.Data[y, x] = en.Current;
                            else sa.Data[y, x] = 0;
                        }
                    }
                }

                return sa;
            }


            public static class Gaussian
            {
                private static SectArray G(SectType t, Single sigma, int w, int h)
                {
                    SectArray sa = new SectArray(t, w, h);
                    Single sum = 0;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        { 
                            Single dx = x - (w - 1.0f) / 2;
                            Single dy = y - (h - 1.0f) / 2;
                            Single val = (Single)Math.Pow(Math.E, -(dx * dx + dy * dy) / (2 * sigma));
                            sum += val;
                            sa.Data[y, x] = val;
                        }
                    }

                    return sa;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="t"></param>
                /// <param name="sigma"></param>
                /// <param name="span">Should be Odd!</param>
                /// <returns></returns>
                public static SectArray NormalizedSum1D(SectType t, Single sigma, int span)
                {
                    
                    SectArray sa = G(t, sigma, 1, span);
                    sa.NormalizeSum();
                    return sa;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="t"></param>
                /// <param name="sigma"></param>
                /// <param name="span">Should be Odd!</param>
                /// <returns></returns>
                public static SectArray NormalizedMax1D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, 1, span);
                    sa.NormalizeMax();
                    return sa;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="t"></param>
                /// <param name="sigma"></param>
                /// <param name="span">Should be Odd!</param>
                /// <returns></returns>
                public static SectArray NormalizedSum2D(SectType t, Single sigma, int span)
                {
                    SectArray sa = G(t, sigma, span, span);
                    sa.NormalizeSum();
                    return sa;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="t"></param>
                /// <param name="sigma"></param>
                /// <param name="span">Should be Odd!</param>
                /// <returns></returns>
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
