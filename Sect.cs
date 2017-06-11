using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSeifert.Utilities;

namespace SamSeifert.CSCV
{
    public class Sect
    {
        public readonly SectType _Type;

        public Sect(SectType t)
        {
            this._Type = t;
        }

        public virtual Boolean isSquishy()
        {
            throw new NotImplementedException();
        }

        public virtual Size getPrefferedSize()
        {
            throw new NotImplementedException();
        }

        public virtual Single this[int y, int x]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// THIS DOESNT WORK YET
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="rt"></param>
        /// <returns></returns>
        public Single this[float y, float x, ResizeType rt]
        {
            get
            {
                var sz = this.getPrefferedSize();
                switch (rt)
                {
                    case ResizeType.Bilinear: // TODO Real Bilinear
                        int xh = ((int)Math.Ceiling(x)).Clampp(0, sz.Width - 1);
                        int xl = ((int)Math.Floor(x)).Clampp(0, sz.Width - 1);
                        int yh = ((int)Math.Ceiling(y)).Clampp(0, sz.Height - 1);
                        int yl = ((int)Math.Floor(y)).Clampp(0, sz.Height - 1);
                        return this[yh, xh] + this[yh, xl] + this[yl, xl] + this[yl, xh];
                    case ResizeType.NearestNeighbor:
                        return this[
                            ((int)Math.Round(y)).Clampp(0, sz.Height - 1),
                            ((int)Math.Round(x)).Clampp(0, sz.Width - 1)];
                    default:
                        return 0;

                }
            }
        }


        /// <summary>
        /// Returns a copy of this image.
        /// </summary>
        /// <returns></returns>
        public virtual Sect Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Should return a transposes copy of this image!
        /// </summary>
        /// <returns></returns>
        public virtual Sect Transpose()
        {
            throw new NotImplementedException();
        }

        public virtual Single getMinValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Single getMaxValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Single getAverageValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public unsafe static Sect FromImage(Image input, bool grayscale = false)
        {
            int w = input.Width;
            int h = input.Height;

            if (grayscale)
            {
                bool create = false;

                Bitmap b = input as Bitmap;

                if (b == null) create = true;
                else if (b.Size != new Size(w, h)) create = true;
                if (create) b = new Bitmap(input, w, h);

                var of_jaffar = new SectArray(SectType.Gray, w, h);

                BitmapData bmd = b.LockBits(
                    new Rectangle(0, 0, input.Width, input.Height),
                     System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                Byte* row;
                int xx = 0, x;

                for (int y = 0; y < h; y++)
                {
                    row = (Byte*)bmd.Scan0 + (y * bmd.Stride);

                    for (x = 0, xx = 0; x < w; x++, xx += 3)
                    {
                        of_jaffar[y, x] = row[xx + 0] / 255.0f;
                    }
                }

                b.UnlockBits(bmd);

                return of_jaffar;
            }
            else
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

                Sect comparer = null;
                foreach (var s in ls)
                {
                    if (comparer == null) comparer = s;
                    else
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                if (comparer[y, x] != s[y, x])
                                    return ret;
                }

                return new SectMask(SectType.Gray, ls[0]); // Gray Scale Anyway
            }
        }

























        public Bitmap getImage(ColorFiller anonFunc = null)
        {
            var sz = this.getPrefferedSize();

            Bitmap newB = new Bitmap(sz.Width, sz.Height, PixelFormat.Format24bppRgb);

            this.RefreshImage(ref newB, anonFunc);

            return newB;
        }

        public Bitmap getImageForSize(Size s, ColorFiller anonFunc = null)
        {
            return this.getImageForSize(s.Width, s.Height, anonFunc);
        }

        public Bitmap getImageForSize(int w, int h, ColorFiller anonFunc = null)
        {
            Bitmap newB = new Bitmap(w, h, PixelFormat.Format24bppRgb);

            this.RefreshImage(ref newB, anonFunc);

            return newB;
        }

        public Bitmap getImageForSizeShrinkEnlarge(Size s, ColorFiller anonFunc = null)
        {
            var ns = Sizing.fitAinB(this.getPrefferedSize(), new Size(s.Width, s.Height));

            Bitmap newB = new Bitmap(ns.Width, ns.Height, PixelFormat.Format24bppRgb);

            this.RefreshImage(ref newB, anonFunc);

            return newB;
        }

        public Bitmap getImageForSizeShrinkOnly(Size s, ColorFiller anonFunc = null)
        {
            var p = this.getPrefferedSize();
            var ns = Sizing.fitAinB(p, s);
            if ((ns.Width < p.Width) || (ns.Height < p.Height)) p = ns.Size;

            Bitmap newB = new Bitmap(p.Width, p.Height, PixelFormat.Format24bppRgb);

            this.RefreshImage(ref newB, anonFunc);

            return newB;
        }


        public delegate void ColorFiller(int y, int x, out float r, out float g, out float b);
        public virtual ColorFiller getColorFiller()
        {
            switch (this._Type)
            {
                case SectType.Holder: // OVERRIDDEN IN SECT HOLDER
                    throw new NotImplementedException();
                case SectType.RGB_R:
                    return delegate (int y, int x, out float r, out float g, out float b)
                    {
                        r = this[y, x];
                        g = 0;
                        b = 0;
                    };
                case SectType.RGB_G:
                    return delegate (int y, int x, out float r, out float g, out float b)
                    {
                        r = 0;
                        g = this[y, x];
                        b = 0;
                    };
                case SectType.RGB_B:
                    return delegate (int y, int x, out float r, out float g, out float b)
                    {
                        r = 0;
                        g = 0;
                        b = this[y, x];
                    };
                default:
                    return delegate (int y, int x, out float r, out float g, out float b)
                    {
                        r = this[y, x];
                        g = this[y, x];
                        b = this[y, x];
                    };
            }
        }

        public unsafe void RefreshImage(ref Bitmap bmp, ColorFiller anonFunc = null)
        {
            if (bmp == null) bmp = this.getImage();
            else
            {
                var sz = this.getPrefferedSize();

                if (anonFunc == null) anonFunc = this.getColorFiller();

                if (bmp.Size == sz)
                {
                    BitmapData bmd = bmp.LockBits(
                    new Rectangle(0, 0, sz.Width, sz.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    Single r, g, b;
                    Byte* row;
                    for (int y = 0; y < sz.Height; y++)
                    {
                        row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (int x = 0, xx = 0; x < sz.Width; x++, xx += 3)
                        {
                            anonFunc(y, x, out r, out g, out b);
                            row[xx + 2] = Helpers.castByte(r * 255);
                            row[xx + 1] = Helpers.castByte(g * 255);
                            row[xx + 0] = Helpers.castByte(b * 255);
                        }
                    }

                    bmp.UnlockBits(bmd);
                }
                else
                {
                    Rectangle rect = Sizing.fitAinB(new Size(sz.Width, sz.Height), bmp.Size);

                    BitmapData bmdNew = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                    byte* row;
                    int xx = 0, x;

                    // Nearest Neighbor [Expansion]
                    if (rect.Width > sz.Width || rect.Height > sz.Height)
                    {
                        int yA, xA;
                        Single r, g, b;
                        float temp;

                        for (int y = 0; y < bmp.Height; y++)
                        {
                            temp = y;
                            temp /= bmp.Height;
                            temp += 1.0f / (2 * bmp.Height);
                            // temp is now scaled 0 to 1 on large image 
                            temp -= 1.0f / (2 * sz.Height);
                            temp *= sz.Height;
                            yA = Helpers.Clamp((int)Math.Round(temp), 0, sz.Height - 1);

                            row = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                            for (x = 0, xx = 0; x < bmp.Width; x++, xx += 3)
                            {
                                temp = x;
                                temp /= bmp.Width;
                                temp += 1.0f / (2 * bmp.Width);
                                // temp is now scaled 0 to 1 on large image 
                                temp -= 1.0f / (2 * sz.Width);
                                temp *= sz.Width;
                                xA = Helpers.Clamp((int)Math.Round(temp), 0, sz.Width - 1);

                                anonFunc(yA, xA, out r, out g, out b);
                                row[xx + 2] = Helpers.castByte(r * 255);
                                row[xx + 1] = Helpers.castByte(g * 255);
                                row[xx + 0] = Helpers.castByte(b * 255);
                            }
                        }
                    }
                    // Bilinear [Compression]
                    else  // TODO: FIX BILINEAR (PIXELS ON BORDER)
                    {
                        Single rYuXu, gYuXu, bYuXu;
                        Single rYuXd, gYuXd, bYuXd;
                        Single rYdXd, gYdXd, bYdXd;
                        Single rYdXu, gYdXu, bYdXu;

                        int yUp, yDown;
                        int xUp, xDown;

                        Single fR = 0;
                        Single fG = 0;
                        Single fB = 0;
                        Single yAdj2 = 0;
                        Single xAdj2 = 0;

                        Single xAdj, yAdj;

                        for (int y = 0; y < bmp.Height; y++)
                        {
                            yAdj = y * (sz.Height - 1);
                            yAdj /= (bmp.Height - 1);
                            yAdj2 = yAdj % 1;

                            yUp = (int)Math.Ceiling((double)yAdj);
                            yDown = (int)yAdj;

                            row = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                            for (x = 0, xx = 0; x < bmp.Width; x++, xx += 3)
                            {
                                xAdj = x * (sz.Width - 1);
                                xAdj /= (bmp.Width - 1);
                                xAdj2 = xAdj % 1;
                                xUp = (int)Math.Ceiling((double)xAdj);
                                xDown = (int)xAdj;

                                if (xUp == xDown && yUp == yDown)
                                {
                                    anonFunc(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    fR = rYuXu;
                                    fG = gYuXu;
                                    fB = bYuXu;
                                }
                                else if (xUp == xDown)
                                {
                                    anonFunc(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    anonFunc(yDown, xUp, out rYdXu, out gYdXu, out bYdXu);
                                    fR = Helpers.getLinearEstimate(rYdXu, rYuXu, yAdj2);
                                    fG = Helpers.getLinearEstimate(gYdXu, gYuXu, yAdj2);
                                    fB = Helpers.getLinearEstimate(bYdXu, bYuXu, yAdj2);
                                }
                                else if (yUp == yDown)
                                {
                                    anonFunc(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    anonFunc(yUp, xDown, out rYuXd, out gYuXd, out bYuXd);
                                    fR = Helpers.getLinearEstimate(rYuXd, rYuXu, xAdj2);
                                    fG = Helpers.getLinearEstimate(gYuXd, gYuXu, xAdj2);
                                    fB = Helpers.getLinearEstimate(bYuXd, bYuXu, xAdj2);
                                }
                                else
                                {
                                    anonFunc(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    anonFunc(yUp, xDown, out rYuXd, out gYuXd, out bYuXd);
                                    anonFunc(yDown, xDown, out rYdXd, out gYdXd, out bYdXd);
                                    anonFunc(yDown, xUp, out rYdXu, out gYdXu, out bYdXu);

                                    fR = Helpers.getLinearEstimate(
                                         Helpers.getLinearEstimate(rYdXd, rYuXd, yAdj2),
                                         Helpers.getLinearEstimate(rYdXu, rYuXu, yAdj2),
                                         xAdj2);
                                    fG = Helpers.getLinearEstimate(
                                         Helpers.getLinearEstimate(gYdXd, gYuXd, yAdj2),
                                         Helpers.getLinearEstimate(gYdXu, gYuXu, yAdj2),
                                         xAdj2);
                                    fB = Helpers.getLinearEstimate(
                                         Helpers.getLinearEstimate(bYdXd, bYuXd, yAdj2),
                                         Helpers.getLinearEstimate(bYdXu, bYuXu, yAdj2),
                                         xAdj2);
                                }

                                row[xx + 2] = Helpers.castByte(fR * 255);
                                row[xx + 1] = Helpers.castByte(fG * 255);
                                row[xx + 0] = Helpers.castByte(fB * 255);
                            }
                        }
                    }

                    bmp.UnlockBits(bmdNew);
                }
            }
        }

        public void ShowDialog()
        {
            using (var bp = this.getImage())
            {
                var frm = new System.Windows.Forms.Form();

                var pb = new System.Windows.Forms.PictureBox();
                pb.Image = bp;
                frm.Controls.Add(pb);
                pb.Dock = System.Windows.Forms.DockStyle.Fill;
                frm.ShowDialog();
            }
        }


        public void SaveToFile(string path)
        {
            using (var bp = this.getImage())
            {
                bp.Save(path);
            }
        }
    }
}
