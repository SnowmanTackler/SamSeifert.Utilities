using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public virtual Sect Clone()
        {
            throw new NotImplementedException();
        }

        public virtual Single min
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Single max
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual Single avg
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual void reset()
        {
            throw new NotImplementedException();
        }



























        public unsafe Bitmap getImage(bool forceZero = true)
        {
            var sz = this.getPrefferedSize();

            Bitmap newB = new Bitmap(sz.Width, sz.Height, PixelFormat.Format24bppRgb);

            this.refreshImage(ref newB, forceZero);

            return newB;
        }

        public unsafe Bitmap getImageForSize(Size s, bool forceZero = true)
        {
            return this.getImageForSize(s.Width, s.Height, forceZero);
        }

        public unsafe Bitmap getImageForSize(int w, int h, bool forceZero = true)
        {
            Bitmap newB = new Bitmap(w, h, PixelFormat.Format24bppRgb);

            this.refreshImage(ref newB, forceZero);

            return newB;
        }

        /// <summary>
        /// Shrinks or Enlarges
        /// </summary>
        /// <param name="s"></param>
        /// <param name="forceZero"></param>
        /// <returns></returns>
        public unsafe Bitmap getImageForSizeShrinkEnlarge(Size s, bool forceZero = true)
        {
            var ns = Sizing.fitAinB(this.getPrefferedSize(), new Size(s.Width, s.Height));

            Bitmap newB = new Bitmap(ns.Width, ns.Height, PixelFormat.Format24bppRgb);

            this.refreshImage(ref newB, forceZero);

            return newB;
        }

        /// <summary>
        /// Just Shrink
        /// </summary>
        /// <param name="s"></param>
        /// <param name="forceZero"></param>
        /// <returns></returns>
        public unsafe Bitmap getImageForSizeShrinkOnly(Size s, bool forceZero = true)
        {
            var p = this.getPrefferedSize();

            var ns = Sizing.fitAinB(p, s);
            if ((ns.Width < p.Width) || (ns.Height < p.Height)) p = ns.Size;

            Bitmap newB = new Bitmap(p.Width, p.Height, PixelFormat.Format24bppRgb);

            this.refreshImage(ref newB, forceZero);

            return newB;
        }

        public virtual void getRGB(int y, int x, out float r, out float g, out float b)
        {
            var val = this[y, x];

            switch (this._Type)
            {
                case SectType.RGB_R:
                    r = val;
                    g = 0;
                    b = 0;
                    break;
                case SectType.RGB_G:
                    r = 0;
                    g = val;
                    b = 0;
                    break;
                case SectType.RGB_B:
                    r = 0;
                    g = 0;
                    b = val;
                    break;
                default:
                    r = val;
                    g = val;
                    b = val;
                    break;
            }
        }

        public unsafe void refreshImage(ref Bitmap bmp, bool forceZero = true)
        {
            if (bmp == null) bmp = this.getImage(forceZero);
            else
            {
                var sz = this.getPrefferedSize();
                
                Single mult, offset;

                if (forceZero)
                {
                    mult = 255.0f;
                    offset = 0;
                }
                else
                {
                    Boolean p = this.min >= 0;
                    Boolean n = this.max <= 0;
                    mult = n ? -255.0f : 255.0f;
                    offset = (p || n) ? 0 : 128;
                }

                if (bmp.Size != sz)
                {
                    Rectangle rect = Sizing.fitAinB(new Size(sz.Width, sz.Height), bmp.Size);

                    BitmapData bmdNew = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                    byte* row;
                    int xx = 0, x;

                    float yAdj;
                    float xAdj;
                    
                    // Nearest Neighbor [Expansion]
                    if (rect.Width > sz.Width || rect.Height > sz.Height)
                    {
                        int yA, xA;
                        Single r, g, b;

                        for (int y = 0; y < bmp.Height; y++)
                        {
                            yAdj = y * (sz.Height - 1);
                            yAdj /= (bmp.Height - 1);
                            yA = (int)Math.Round(yAdj, 0);

                            row = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                            for (x = 0, xx = 0; x < bmp.Width; x++, xx += 3)
                            {
                                xAdj = x * (sz.Width - 1);
                                xAdj /= (bmp.Width - 1);
                                xA = (int)Math.Round(xAdj, 0);

                                this.getRGB(yA, xA, out r, out g, out b);
                                row[xx + 2] = IA_Helpers.castByte(r * mult + offset);
                                row[xx + 1] = IA_Helpers.castByte(g * mult + offset);
                                row[xx + 0] = IA_Helpers.castByte(b * mult + offset);
                            }
                        }
                    }
                    // Bilinear [Compression]
                    else
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
                                    this.getRGB(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    fR = rYuXu;
                                    fG = gYuXu;
                                    fB = bYuXu;
                                }
                                else if (xUp == xDown)
                                {
                                    this.getRGB(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    this.getRGB(yDown, xUp, out rYdXu, out gYdXu, out bYdXu);
                                    fR = SectHolder.getLinearEstimate(rYdXu, rYuXu, yAdj2);
                                    fG = SectHolder.getLinearEstimate(gYdXu, gYuXu, yAdj2);
                                    fB = SectHolder.getLinearEstimate(bYdXu, bYuXu, yAdj2);
                                }
                                else if (yUp == yDown)
                                {
                                    this.getRGB(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    this.getRGB(yUp, xDown, out rYuXd, out gYuXd, out bYuXd);
                                    fR = SectHolder.getLinearEstimate(rYuXd, rYuXu, xAdj2);
                                    fG = SectHolder.getLinearEstimate(gYuXd, gYuXu, xAdj2);
                                    fB = SectHolder.getLinearEstimate(bYuXd, bYuXu, xAdj2);
                                }
                                else
                                {
                                    this.getRGB(yUp, xUp, out rYuXu, out gYuXu, out bYuXu);
                                    this.getRGB(yUp, xDown, out rYuXd, out gYuXd, out bYuXd);
                                    this.getRGB(yDown, xDown, out rYdXd, out gYdXd, out bYdXd);
                                    this.getRGB(yDown, xUp, out rYdXu, out gYdXu, out bYdXu);

                                    fR = SectHolder.getLinearEstimate(
                                         SectHolder.getLinearEstimate(rYdXd, rYuXd, yAdj2),
                                         SectHolder.getLinearEstimate(rYdXu, rYuXu, yAdj2),
                                         xAdj2);
                                    fG = SectHolder.getLinearEstimate(
                                         SectHolder.getLinearEstimate(gYdXd, gYuXd, yAdj2),
                                         SectHolder.getLinearEstimate(gYdXu, gYuXu, yAdj2),
                                         xAdj2);
                                    fB = SectHolder.getLinearEstimate(
                                         SectHolder.getLinearEstimate(bYdXd, bYuXd, yAdj2),
                                         SectHolder.getLinearEstimate(bYdXu, bYuXu, yAdj2),
                                         xAdj2);
                                }

                                row[xx + 2] = IA_Helpers.castByte(fR * mult + offset);
                                row[xx + 1] = IA_Helpers.castByte(fG * mult + offset);
                                row[xx + 0] = IA_Helpers.castByte(fB * mult + offset);
                            }
                        }
                    }

                    bmp.UnlockBits(bmdNew);
                }
                else
                {

                    BitmapData bmd = bmp.LockBits(
                    new Rectangle(0, 0, sz.Width, sz.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    Single r, g, b;

                    Byte* rowNew;
                    for (int y = 0; y < sz.Height; y++)
                    {
                        rowNew = (byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (int x = 0, xx = 0; x < sz.Width; x++, xx += 3)
                        {
                            this.getRGB(y, x, out r, out g, out b);
                            rowNew[xx + 2] = IA_Helpers.castByte(r * mult + offset);
                            rowNew[xx + 1] = IA_Helpers.castByte(g * mult + offset);
                            rowNew[xx + 0] = IA_Helpers.castByte(b * mult + offset);
                        }
                    }

                    bmp.UnlockBits(bmd);
                }
            }
        }
    }
}
