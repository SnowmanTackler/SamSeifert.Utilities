using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra;

namespace SamSeifert.CSCV
{
    public class SectArray : Sect
    {
        public Matrix<float> Data { get { return this._Data; } }
        private readonly Matrix<float> _Data = null;
        private readonly int _Width;
        private readonly int _Height;

        public int Width
        {
            get
            {
                return this._Width;
            }
        }

        public int Height
        {
            get
            {
                return this._Height;
            }
        }

        public override bool isSquishy()
        {
            return false;
        }

        public override Sect Clone()
        {
            return new SectArray(this._Data.Clone(), this._Type);
        }

        public void SetValue(float val)
        {
            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data.At(y, x, val);
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
#if DEBUG
                return this._Data[y, x];
#else
                return this._Data.At(y, x);                  
#endif
            }
            set
            {
#if DEBUG
                this._Data[y, x] = value;
#else
                this._Data.At(y, x, value);
#endif
            }
        }

        public SectArray(SectType t, int w, int h)
            : this(new Single[h, w], t)
        {
        }

        public SectArray(SectType t, Size sz)
        : this(t, sz.Width, sz.Height)
        {
        }

        public SectArray(Single[,] data, SectType t)
            : base(t)
        {
            this._Data = Matrix<float>.Build.DenseOfArray(data);
            this._Height = this._Data.RowCount;
            this._Width = this._Data.ColumnCount;
        }

        public SectArray(Matrix<float> data, SectType t)
            : base(t)
        {
            this._Data = data;
            this._Height = this._Data.RowCount;
            this._Width = this._Data.ColumnCount;
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




        public override void Normalize(float min_value, float max_value)
        {
            if (max_value <= min_value) throw new Exception();

            var stats = this.getStats();

            float scalar = (stats._Max == stats._Min) ? 0 : (max_value - min_value) / (stats._Max - stats._Min);
                               
            for (int i = 0; i < this._Height; i++)
                for (int j = 0; j < this._Width; j++)
                    this.Data[i, j] = (this.Data[i, j] - stats._Min) * scalar + min_value;
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

            public static SectArray FromSquishy(Sect squish, Size sz)
            {
                if (!squish.isSquishy()) throw new NotImplementedException();
                if (squish._Type == SectType.Holder) throw new NotImplementedException();
                SectArray sa = new SectArray(squish._Type, sz);
                for (int y = 0; y < sz.Height; y++)
                {
                    for (int x = 0; x < sz.Width; x++)
                    {
                        sa.Data[y, x] = squish[y, x];
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
                    sa.Normalize();
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
                    sa.Normalize();
                    return sa;
                }
            }
        }

    }
}
