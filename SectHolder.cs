using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SamSeifert.Utilities;


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
            get { return this._Size.Width; }
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

        public override Single min
        {
            get
            {
                var mn = Single.MaxValue;
                foreach (var s in this._Sects.Values) mn = Math.Min(mn, s.min);
                return mn;
            }
        }

        public override Single max
        {
            get
            {
                var mx = Single.MinValue;
                foreach (var s in this._Sects.Values) mx = Math.Max(mx, s.max);
                return mx;
            }
        }

        public override Single avg
        {
            get
            {
                Single av = 0;
                foreach (var s in this._Sects.Values) av += s.avg;
                av /= this._Sects.Count;
                return av;
            }
        }












        public static SectHolder SectHoldeFromImage(Image input)
        {
            return SectHolder.SectHoldeFromImage(input, input.Width, input.Height);
        }

        public static SectHolder SectHoldeFromImage(Image input, int w, int h)
        {
            var ls = new List<Sect>();
            ls.Add(new SectArray(SectType.RGB_R, w, h));
            ls.Add(new SectArray(SectType.RGB_G, w, h));
            ls.Add(new SectArray(SectType.RGB_B, w, h));

            bool create = false;

            Bitmap b = input as Bitmap;

            if (b == null) create = true;
            else if (b.Size != new Size(w, h)) create = true;
            if (create) b = new Bitmap(input, w, h);

            var ret = new SectHolder(ls.ToArray());

            ret.setImage(b);

            return ret;
        }

        private unsafe void setImage(Bitmap input)
        {
            if (input == null) return;
            else if (this._Size != input.Size) Console.WriteLine("SectHolder: private unsafe void setImage(Bitmap input) - Size mismatch");
            else if (!(this.hasRGB())) Console.WriteLine("SectHolder: private unsafe void setImage(Bitmap input) - RGB mismatch");
            {
                var r = this._Sects[SectType.RGB_R] as SectArray;
                var g = this._Sects[SectType.RGB_G] as SectArray;
                var b = this._Sects[SectType.RGB_B] as SectArray;

                BitmapData bmd = input.LockBits(
                    new Rectangle(0, 0, input.Width, input.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                Byte * row;
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

                input.UnlockBits(bmd);

                this.reset();                
            }
        }

        public override void reset()
        {
            foreach (var s in this._Sects.Values) s.reset();
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










        internal override ColorFiller getColorFiller()
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
                    ColorMethods.hsv2rgb(H[y, x], S[y, x], V[y, x], out r, out g, out b);
                };
            }

            if (this.hasHSL())
            {
                var H = this._Sects[SectType.Hue];
                var S = this._Sects[SectType.HSL_S];
                var L = this._Sects[SectType.HSL_L];

                return delegate (int y, int x, out float r, out float g, out float b)
                {
                    ColorMethods.hsl2rgb(H[y, x], S[y, x], L[y, x], out r, out g, out b);
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

    }
}
