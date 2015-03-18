using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
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

        public SectHolder(SectType[] sects, Size sz)
            : base(SectType.Holder)
        {
            this._Size = sz;
            this._Sects = new Dictionary<SectType, Sect>();
            foreach (var st in sects) 
                this._Sects[st] = new SectArray(st, sz.Width, sz.Height);

            this._Sects.TryGetValue(SectType.RGB_R, out this._SectR);
            this._Sects.TryGetValue(SectType.RGB_G, out this._SectG);
            this._Sects.TryGetValue(SectType.RGB_B, out this._SectB);
        }

        public SectHolder(Sect[] sects)
            : base(SectType.Holder)
        {
            this._Sects = new Dictionary<SectType, Sect>();
            bool first = true;
            foreach (var s in sects)
            {
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

                if (s._Type == SectType.Holder)
                {
                    throw new Exception("SectHolder: public SectHolder(Sect[] sects) : base(SectType.Holder) - Sectype mismatch");
                }
            }

            this._Sects.TryGetValue(SectType.RGB_R, out this._SectR);
            this._Sects.TryGetValue(SectType.RGB_G, out this._SectG);
            this._Sects.TryGetValue(SectType.RGB_B, out this._SectB);
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

        private bool hasRGB()
        {
            return
                this.has(SectType.RGB_R) &&
                this.has(SectType.RGB_G) &&
                this.has(SectType.RGB_B);
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

        public Boolean has(SectType st) 
        {
            return this._Sects.ContainsKey(st);
        }











        private readonly Sect _SectR;
        private readonly Sect _SectG;
        private readonly Sect _SectB;


        public override void getRGB(int y, int x, out float r, out float g, out float b)
        {
            if (this._SectR == null) r = 0;
            else r = this._SectR[y, x];

            if (this._SectG == null) g = 0;
            else g = this._SectG[y, x];

            if (this._SectB == null) b = 0;
            else b = this._SectB[y, x];

        }






        



        /// <summary>
        /// Gets the linear estimate of y for an x value between 0 and 1
        /// </summary>
        /// <param name="y0">Value of function at x = 0</param>
        /// <param name="y1">Value of function at x = 1</param>
        /// <param name="x">X value between 0 and 1</param>
        /// <returns></returns>
        internal static float getLinearEstimate(float y0, float y1, float x)
        {
            return y0 + x * (y1 - y0);
        }

    }
}
