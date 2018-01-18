using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SamSeifert.Utilities;
using SamSeifert.Utilities.Extensions;

namespace SamSeifert.CSCV
{
    public class SectHolder : Sect
    {
        private readonly Size _Size = new Size(0, 0);
        private readonly Dictionary<SectType, Sect> _Sects;
        public Dictionary<SectType, Sect> Sects { get { return this._Sects; } }

        public int Width
        {
            get { return this._Size.Width; }
        }
        public int Height
        {
            get { return this._Size.Height; }
        }

        public SectHolder(Size sz, params SectType[] sects)
            : base(SectType.Holder)
        {
            this._Size = sz;
            this._Sects = new Dictionary<SectType, Sect>();
            foreach (var st in sects) 
                this._Sects[st] = new SectArray(st, sz.Width, sz.Height);

        }

        public SectHolder(params Sect[] sects)
            : base(SectType.Holder)
        {
            this._Sects = new Dictionary<SectType, Sect>();
            bool first = true;
            foreach (var s in sects)
            {
                if (s == null)
                    throw new Exception("SectHolder: public SectHolder(Sect[] sects) : base(SectType.Holder) - Null Input");

                if (s._Type == SectType.Holder)
                    throw new Exception("SectHolder: public SectHolder(Sect[] sects) : base(SectType.Holder) - Sectype mismatch");

                this._Sects[s._Type] = s;
                var ss = s.getPrefferedSize();

                if (s.isSquishy())
                {
                    if (!first)
                    {
                        this._Size.Width = Math.Max(this._Size.Width, ss.Width);
                        this._Size.Height = Math.Max(this._Size.Height, ss.Height);
                    }
                }
                else if (first)
                {
                    this._Size = ss;
                    first = false;
                }
                else if (this._Size != s.getPrefferedSize())
                {
                    throw new Exception("SectHolder: public SectHolder(Sect[] sects) : base(SectType.Holder) - Size mistmatch");
                }

            }
        }

        public override void Normalize(float min_value, float max_value)
        {
            if (max_value <= min_value) throw new Exception();

            var stats = this.getStats();

            float scalar = (stats._Max == stats._Min) ? 0 : (max_value - min_value) / (stats._Max - stats._Min);

            int height = this.Height;
            int width = this.Width;

            foreach (var s in _Sects.Values)
            {
                if (!(s is SectArray))
                    throw new Exception();
                var sa = s as SectArray;
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        sa.Data[i, j] = (sa.Data[i, j] - stats._Min) * scalar + min_value;

            }
        }

        public override Boolean isSquishy()
        {
            return false;
        }

        public override Size getPrefferedSize()
        {
            return this._Size;
        }

        public override Sect Clone()
        {
            var ls = new List<Sect>();

            foreach (var s in this._Sects.Values) ls.Add(s.Clone());

            return new SectHolder(ls.ToArray());
        }

        public override Sect Transpose()
        {
            var ls = new List<Sect>();

            foreach (var s in this._Sects.Values) ls.Add(s.Transpose());

            return new SectHolder(ls.ToArray());

        }

        public unsafe void setImage(Bitmap input)
        {
            if (input == null) return;
            else if (this._Size != input.Size) Console.WriteLine("SectHolder: private unsafe void setImage(Bitmap input) - Size mismatch");
            else if (!(this.hasRGB())) Console.WriteLine("SectHolder: private unsafe void setImage(Bitmap input) - RGB mismatch");
            {
                var r = this._Sects[SectType.RGB_R] as SectArray;
                var g = this._Sects[SectType.RGB_G] as SectArray;
                var b = this._Sects[SectType.RGB_B] as SectArray;

                using (var bmd = input.Locked(
                    ImageLockMode.ReadOnly, 
                    PixelFormat.Format24bppRgb))
                {

                    Byte* row;
                    int xx = 0, x;

                    for (int y = 0; y < this._Size.Height; y++)
                    {
                        row = (Byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (x = 0, xx = 0; x < this._Size.Width; x++, xx += 3)
                        {
                            r[y, x] = row[xx + 2] / 255.0f;
                            g[y, x] = row[xx + 1] / 255.0f;
                            b[y, x] = row[xx + 0] / 255.0f;
                        }
                    }
                }
            }
        }

        ~SectHolder()
        {
        }

        public bool hasRGB()
        {
            return this.has(SectType.RGB_R, SectType.RGB_G, SectType.RGB_B);
        }

        public bool hasHSV()
        {
            return this.has(SectType.Hue, SectType.HSV_S, SectType.HSV_V);
        }

        public bool hasHSL()
        {
            return this.has(SectType.Hue, SectType.HSL_S, SectType.HSL_L);
        }

        public bool has(params SectType[] ss)
        {
            foreach (var s in ss)
                if (!this._Sects.ContainsKey(s)) 
                    return false;

            return true;
        }










        public override ColorFiller getColorFiller()
        {
            if (this.hasRGB())
            {
                var R = this._Sects[SectType.RGB_R];
                var G = this._Sects[SectType.RGB_G];
                var B = this._Sects[SectType.RGB_B];

                return delegate (int y, int x, out float r, out float g, out float b)
                {
                    r = R[y, x];
                    g = G[y, x];
                    b = B[y, x];
                };
            }

            if (this.hasHSV())
            {
                var H = this._Sects[SectType.Hue];
                var S = this._Sects[SectType.HSV_S];
                var V = this._Sects[SectType.HSV_V];

                return delegate (int y, int x, out float r, out float g, out float b)
                {
                    ColorUtil.hsv2rgb(H[y, x], S[y, x], V[y, x], out r, out g, out b);
                };
            }

            if (this.hasHSL())
            {
                var H = this._Sects[SectType.Hue];
                var S = this._Sects[SectType.HSL_S];
                var L = this._Sects[SectType.HSL_L];

                return delegate (int y, int x, out float r, out float g, out float b)
                {
                    ColorUtil.hsl2rgb(H[y, x], S[y, x], L[y, x], out r, out g, out b);
                };
            }

            return delegate (int y, int x, out float r, out float g, out float b)
            {
                r = 0;
                g = 0;
                b = 0;
            };
        }









        public Sect getSect(SectType t)
        {
            Sect s;
            if (this._Sects.TryGetValue(t, out s)) return s;
            else return null;
        }

        public SectType[] getSectTypes()
        {
            return this._Sects.Keys.ToArray();
        }












        /// <summary>
        /// ONLY USE FOR +=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectHolder operator +(SectHolder left, float right)
        {
            foreach (var sect in left._Sects.Values)
            {
                var sa = sect as SectArray;
                if (sa != null) sa += right;
            }
            return left;
        }

        /// <summary>
        /// ONLY USE FOR -=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectHolder operator -(SectHolder left, float right)
        {
            foreach (var sect in left._Sects.Values)
            {
                var sa = sect as SectArray;
                if (sa != null) sa -= right;
            }
            return left;
        }

        /// <summary>
        /// ONLY USE FOR *=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectHolder operator *(SectHolder left, float right)
        {
            foreach (var sect in left._Sects.Values)
            {
                var sa = sect as SectArray;
                if (sa != null) sa *= right;
            }
            return left;
        }

        /// <summary>
        /// ONLY USE FOR /=
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SectHolder operator /(SectHolder left, float right)
        {
            foreach (var sect in left._Sects.Values)
            {
                var sa = sect as SectArray;
                if (sa != null) sa /= right;
            }
            return left;
        }

        public void RefreshImage(byte[] data, params SectType[] types)
        {
            int start = 0;
            int stride = types.Length;

            foreach (var type in types)
            {
                var sect = this._Sects[type] as SectArray;

                int index = start;
                start++;

                for (int y = 0; y < this._Size.Height; y++)
                {

                    for (int x = 0; x < this._Size.Width; x++)
                    {
                        sect[y, x] = data[index] / 255.0f;
                        index += stride;
                    }
                }
            }
        }

        public Byte[] AsBytes(params SectType[] types)
        {
            var bytes = new Byte[this._Size.Height * this._Size.Width * types.Length];
            int start = 0;
            int stride = types.Length;

            foreach (var type in types)
            {
                var sect = this._Sects[type] as SectArray;

                int index = start;
                start++;

                for (int y = 0; y < this._Size.Height; y++)
                {
                    for (int x = 0; x < this._Size.Width; x++)
                    {
                        bytes[index] = MathUtil.ClampByte(255 * sect[y, x]);
                        index += stride;
                    }
                }
            }

            return bytes;
        }
    }
}
