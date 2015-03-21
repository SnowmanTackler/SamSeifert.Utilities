﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class IA_Single
    {
        private static void match(Sect inpt, ref Sect outp)
        {
            Boolean remake = outp == null;
            if (!remake) remake = (inpt._Type != outp._Type) || (inpt.getPrefferedSize() != outp.getPrefferedSize());
            if (!remake)
            {
                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var k1 = (inpt as SectHolder).Sects.Keys;
                            var k2 = (outp as SectHolder).Sects.Keys;
                            remake = (k1.Count != k2.Count) || (k1.Except(k2).Any());
                            foreach (var val in (outp as SectHolder).Sects.Values)
                            {
                                if (remake) break;
                                else remake = !(val is SectArray);
                            }
                        }
                        break;
                    default:
                        remake = !(outp is SectArray);
                        break;
                }
            }
            if (remake)
            {
                Size sz = inpt.getPrefferedSize();
                switch (inpt._Type)
                {
                    case SectType.Holder:
                        outp = new SectHolder((inpt as SectHolder).getSectTypes(), sz);
                        break;
                    default:
                        outp = new SectArray(inpt._Type, sz.Width, sz.Height);
                        break;
                }
            }
            else outp.reset();
        }

        private static void match(ref Sect outp, Size sz, SectType[] sts)
        {
            Boolean remake = outp == null;
            if (!remake) remake = sz != outp.getPrefferedSize();
            switch (sts.Length)
            {
                case 1:
                    {
                        if (!remake) remake = sts.First() != outp._Type;
                        if (remake) outp = new SectArray(SectType.Gray, sz.Width, sz.Height);
                        else outp.reset();
                    }
                    break;
                default:
                    {
                        if (!remake) remake = SectType.Holder != outp._Type;
                        if (!remake)
                        {
                            SectHolder sh = outp as SectHolder;
                            if (sh.Sects.Count == sts.Length)
                            {
                                foreach (var st in sts)
                                {
                                    if (sh.Sects.ContainsKey(st))
                                    {
                                        var sa = sh.Sects[st];
                                        if (!(sa is SectArray))
                                        {
                                            remake = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        remake = true;
                                        break;
                                    }
                                }
                            }
                            else remake = true;
                        }
                        if (remake) outp = new SectHolder(sts, sz);
                        else outp.reset();
                    }
                    break;
            }
        }























        public static ToolboxReturn Convolute(Sect inpt, Single[,] vals, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);
                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Convolute_(sh1.getSect(st), vals, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Convolute_(inpt, vals, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Convolute_(Sect inpt, Single[,] vals, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            int rows = vals.GetLength(0);
            int cols = vals.GetLength(1);

            int c2 = cols / 2;
            int r2 = rows / 2;

            int tx, ty;
            float sum;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    sum = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        ty = y + i - r2;
                        if (ty >= 0 && ty < h)
                        {
                            for (int j = 0; j < cols; j++)
                            {
                                tx = x + j - c2;
                                if (tx >= 0 && tx < w)
                                {
                                    sum += inpt[ty, tx] * vals[i, j];
                                }
                            }
                        }
                    }
                    outp[y, x] = sum;
                }
            }
        }











        public static ToolboxReturn GrayScale(Sect inpt, GrayScaleType t, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(ref outp, inpt.getPrefferedSize(), new SectType[] { SectType.Gray });

                var sz = outp.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                if (inpt is SectHolder)
                {
                    var sh = inpt as SectHolder;
                    switch (t)
                    {
                        case GrayScaleType.Maximum:
                            {
                                bool first = true;

                                foreach (var inp in sh.Sects.Values)
                                {
                                    if (first)
                                    {
                                        first = false;
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] = inp[y, x];
                                    }
                                    else
                                    {
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] = Math.Max(outp[y, x], inp[y, x]);
                                    }
                                }

                                return ToolboxReturn.Good;
                            }
                        case GrayScaleType.Minimum:
                            {
                                bool first = true;

                                foreach (var inp in sh.Sects.Values)
                                {
                                    if (first)
                                    {
                                        first = false;
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] = inp[y, x];
                                    }
                                    else
                                    {
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] = Math.Min(outp[y, x], inp[y, x]);
                                    }
                                }

                                return ToolboxReturn.Good;
                            }
                        case GrayScaleType.Mean:
                            {
                                Single mult = 1.0f / sh.Sects.Count;
                                bool first = true;
                                foreach (var inp in sh.Sects.Values)
                                {
                                    if (first)
                                    {
                                        first = false;
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] = inp[y, x] * mult;
                                    }
                                    else
                                    {
                                        for (int y = 0; y < h; y++)
                                            for (int x = 0; x < w; x++)
                                                outp[y, x] += inp[y, x] * mult;
                                    }
                                }

                                return ToolboxReturn.Good;
                            }
                        default: return ToolboxReturn.SpecialError;
                    }
                }
                else
                {
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            outp[y, x] = inpt[y, x];
                    return ToolboxReturn.Good;
                }
            }
        }


















        public static ToolboxReturn Resize(Sect inpt, Size res, ResizeType t, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Boolean remake = outp == null;
                if (!remake) remake = (inpt._Type != outp._Type) || (res != outp.getPrefferedSize());
                if (!remake)
                {
                    switch (inpt._Type)
                    {
                        case SectType.Holder:
                            {
                                var k1 = (inpt as SectHolder).Sects.Keys;
                                var k2 = (outp as SectHolder).Sects.Keys;
                                remake = (k1.Count != k2.Count) || (k1.Except(k2).Any());
                                foreach (var val in (outp as SectHolder).Sects.Values)
                                {
                                    if (remake) break;
                                    else remake = !(val is SectArray);
                                }
                            }
                            break;
                        default:
                            remake = !(outp is SectArray);
                            break;
                    }
                }
                if (remake)
                {
                    switch (inpt._Type)
                    {
                        case SectType.Holder:
                            outp = new SectHolder((inpt as SectHolder).getSectTypes(), res);
                            break;
                        default:
                            outp = new SectArray(inpt._Type, res.Width, res.Height);
                            break;
                    }
                }
                else outp.reset();

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Resize_(sh1.getSect(st), res, t, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Resize_(inpt, res, t, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Resize_(Sect inpt, Size res, ResizeType t, SectArray outp)
        {
            int h = res.Height;
            int w = res.Width;

            int xUp, xDown, yUp, yDown;
            float xAdj, yAdj;

            Size sz = inpt.getPrefferedSize();
            int refH = sz.Height;
            int refW = sz.Width;

            switch (t)
            {
                case ResizeType.NearestNeighbor:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            yAdj = y * (refH - 1);
                            yAdj /= (h - 1);
                            yUp = (int)Math.Round(yAdj, 0);

                            for (int x = 0; x < w; x++)
                            {
                                xAdj = x * (refW - 1);
                                xAdj /= (w - 1);
                                xUp = (int)Math.Round(xAdj, 0);
                                outp[y, x] = inpt[yUp, xUp];
                            }
                        }
                        break;
                    }
                case ResizeType.Bilinear:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            yAdj = y * (refH - 1);
                            yAdj /= (h - 1);
                            yUp = (int)Math.Ceiling((double)yAdj);
                            yDown = (int)yAdj;

                            for (int x = 0; x < w; x++)
                            {
                                xAdj = x * (refW - 1);
                                xAdj /= Math.Max(1, (w - 1));
                                xUp = (int)Math.Ceiling((double)xAdj);
                                xDown = (int)xAdj;

                                if (xUp == xDown && yUp == yDown)
                                {
                                    outp[y, x] = inpt[yUp, xUp];
                                }
                                else if (xUp == xDown)
                                {
                                    outp[y, x] = SectHolder.getLinearEstimate(
                                        inpt[yDown, xUp],
                                        inpt[yUp, xUp],
                                        yAdj % 1);
                                }
                                else if (yUp == yDown)
                                {
                                    outp[y, x] = SectHolder.getLinearEstimate(
                                        inpt[yUp, xDown],
                                        inpt[yUp, xUp],
                                        xAdj % 1);
                                }
                                else
                                {
                                    outp[y, x] = SectHolder.getLinearEstimate(
                                        SectHolder.getLinearEstimate(
                                            inpt[yDown, xDown],
                                            inpt[yUp, xDown],
                                            yAdj % 1),
                                        SectHolder.getLinearEstimate(
                                            inpt[yDown, xUp],
                                            inpt[yUp, xUp],
                                            yAdj % 1),
                                        xAdj % 1);
                                }
                            }
                        }
                        break;
                    }
            }
        }















        public static ToolboxReturn HistogramEqualize(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                HistogramEqualize_(sh1.getSect(st), sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        HistogramEqualize_(inpt, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void HistogramEqualize_(Sect inpt, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            int[] counts = new int[511];
            Byte[] _Bytes = new Byte[511];

            for (int i = 0; i < counts.Length; i++) counts[i] = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    counts[255 + IA_Helpers.Cast(inpt[y, x] * 255)]++;
                }
            }

            int sum = 0, sumLast = 0, temp;
            for (int i = 0; i < counts.Length; i++)
            {
                temp = counts[i];
                if (temp != 0)
                {
                    sumLast = sum;
                    sum += temp;
                }
            }

            sum = 0;
            sumLast = Math.Max(1, sumLast);
            for (int i = 0; i < counts.Length; i++)
            {
                _Bytes[i] = (Byte)((255 * sum) / (sumLast));
                sum += counts[i];
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    outp[y, x] = _Bytes[255 + IA_Helpers.Cast(inpt[y, x] * 255)] / 255.0f;
                }
            }
        }







        public static ToolboxReturn Gradient(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Gradient_(sh1.getSect(st), sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Gradient_(inpt, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Gradient_(Sect inp, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            var valsX = new Single[,]
            {
                {-1,  0,  1},
                {-2,  0,  2},
                {-1,  0,  1},
            };

            var valsY = new Single[,]
            {
                {-1, -2, -1},
                { 0,  0,  0},
                { 1,  2,  1},
            };

            const int dim = 3;
            int ty;
            float val;
            float gx;
            float gy;

            for (int y = 1; y < h - 1; y++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    gx = 0;
                    gy = 0;
                    for (int i = 0; i < dim; i++)
                    {
                        ty = y + i - 1;
                        for (int j = 0; j < dim; j++)
                        {
                            val = inp[ty, x + j - 1];
                            gx += val * valsX[i, j];
                            gy += val * valsY[i, j];
                        }
                    }
                    outp[y, x] = (Single)Math.Sqrt(gx * gx + gy * gy);
                }
            }

            for (int y = 0; y < h; y++)
            {
                outp[y, 0] = 0;
                outp[y, w - 1] = 0;
            }

            for (int x = 0; x < w; x++)
            {
                outp[0, x] = 0;
                outp[h - 1, x] = 0;
            }
        }














        public static ToolboxReturn TwoTone(Sect inpt, Single thres, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                TwoTone_(sh1.getSect(st), thres, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        TwoTone_(inpt, thres, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void TwoTone_(Sect inpt, Single thresh, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = inpt[y, x] < thresh ? 0 : 1;

        }

        public static ToolboxReturn Multiply(Sect inpt, Single val, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Multiply_(sh1.getSect(st), val, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Multiply_(inpt, val, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Multiply_(Sect inpt, Single val, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = inpt[y, x] * val;

        }

        public static ToolboxReturn Power(Sect inpt, Single val, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Power_(sh1.getSect(st), val, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Power_(inpt, val, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Power_(Sect inpt, Single val, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = (Single) Math.Pow(inpt[y, x], val);

        }

        public static ToolboxReturn Abs(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Abs_(sh1.getSect(st), sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Abs_(inpt, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Abs_(Sect inpt, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = Math.Abs(inpt[y, x]);

        }




        public static ToolboxReturn SplitPositiveNegative(Sect inpt, ref Sect pos, ref Sect neg)
        {
            if (inpt == null)
            {
                pos = null;
                neg = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref pos);
                match(inpt, ref neg);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var shp = pos as SectHolder;
                            var shn = neg as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                SplitPositiveNegative_(
                                    sh1.getSect(st), 
                                    shp.getSect(st) as SectArray, 
                                    shn.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        SplitPositiveNegative_(inpt, pos as SectArray, neg as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void SplitPositiveNegative_(Sect inpt, SectArray pos, SectArray neg)
        {
            var sz = inpt.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Single val = inpt[y, x];
                    if (val > 0)
                    {
                        neg[y, x] = 0;
                        pos[y, x] = val;
                    }
                    else if (val < 0)
                    {
                        neg[y, x] = -val;
                        pos[y, x] = 0;
                    }
                    else
                    {
                        neg[y, x] = 0;
                        pos[y, x] = 0;
                    }
                }
            }
        }















        public static ToolboxReturn BitPerPixel(Sect inpt, int bpp, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if ((bpp < 1) || (bpp > 8))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                BitPerPixel_(sh1.getSect(st), bpp, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        BitPerPixel_(inpt, bpp, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void BitPerPixel_(Sect inpt, int bpp, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            float mult = (float)Math.Pow(2, bpp);
            float div = mult - 1;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    outp[y, x] = IA_Helpers.Clamp(((Single)Math.Round(inpt[y, x] * mult - 0.5f)) / div, 0, 1);
                }
            }
        }
















        public static ToolboxReturn Clamp(Sect inpt, Single min, Single max, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Clamp_(sh1.getSect(st), min, max, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Clamp_(inpt, min, max, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Clamp_(Sect inpt, Single min, Single max, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = IA_Helpers.Clamp(inpt[y, x], min, max);
        }




















        private static float nextGaussian(Single std, Random rand)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return std * (float)randStdNormal;
            //            double randNormal = mean + stdDev * randStdNormal;
        }

        private static float nextNormal(Single std, Random rand)
        {
            return std * (1 - 2 * ((float)(rand.NextDouble())));
        }


        public static ToolboxReturn Noise(Sect inpt, NoiseType t, Single p, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Random r = new Random();
                match(inpt, ref outp);

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Noise_(sh1.getSect(st), t, p, r, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Noise_(inpt,  t, p, r, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Noise_(Sect inpt, NoiseType t, Single p, Random r, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            switch (t)
            {
                case NoiseType.Gaussian:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                outp[y, x] = inpt[y, x] + nextGaussian(p, r);
                            }
                        }
                        break;
                    }
                case NoiseType.Uniform:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                outp[y, x] = inpt[y, x] + nextNormal(p, r);
                            }
                        }
                        break;
                    }
                case NoiseType.SaltAndPepper:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            for (int x = 0; x < w; x++)
                            {
                                float rand = (float)r.NextDouble();
                                outp[y, x] = (rand * 2 < p) ? 1 : (rand < p) ? 0 : inpt[y, x];
                            }
                        }
                        break;
                    }
            }
        }




















        public static ToolboxReturn Colormap(Sect inpt, ColorMapType c, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type == SectType.Holder)
            {
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Single min = inpt.min;
                Single range = inpt.max - min;

                if (range == 0)
                {
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }

                match(ref outp, inpt.getPrefferedSize(), new SectType[] { 
                    SectType.RGB_R,
                    SectType.RGB_G, 
                    SectType.RGB_B, 
                });

                Size sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                Single[,] r = ((outp as SectHolder).Sects[SectType.RGB_R] as SectArray).Data;
                Single[,] g = ((outp as SectHolder).Sects[SectType.RGB_G] as SectArray).Data;
                Single[,] b = ((outp as SectHolder).Sects[SectType.RGB_B] as SectArray).Data;

                var ch = getColdHot();
                var lens = ch.GetLength(0) - 1;
                int cur = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        // Shouldn't need this but what the hell                        
                        Single lum = Math.Max(0, Math.Min(1, (inpt[y, x] - min) / range));
                        
                        switch (c)
                        {
                            case ColorMapType.Hue:
                                ColorMethods.hsv2rgb(lum, 1, 1,
                                    out r[y, x],
                                    out g[y, x],
                                    out b[y, x]);
                                break;
                            case ColorMapType.Cold_Hot:
                                cur = (int)(lum * lens);
                                r[y, x] = ch[cur, 0];
                                g[y, x] = ch[cur, 1];
                                b[y, x] = ch[cur, 2];
                                break;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        private static Single[,] arrayColdHot = null;
        private static unsafe Single[,] getColdHot()
        {
            if (arrayColdHot == null)
            {
                var im = Properties.Resources.HeatmapHotCold;

                int w = im.Size.Width;

                var bits = im.LockBits(
                    new Rectangle(0, 0, w, im.Size.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                Byte* row;
                int xx = 0, x;
                const int y = 0;

                row = (Byte*)bits.Scan0 + (y * bits.Stride);

                var ia = new Single[w, 3];

                for (x = 0, xx = 0; x < w; x++, xx += 3)
                {
                    ia[x, 0] = row[xx + 2] / 255.0f;
                    ia[x, 1] = row[xx + 1] / 255.0f;
                    ia[x, 2] = row[xx + 0] / 255.0f;
                }

                im.UnlockBits(bits);

                arrayColdHot = ia;
            }
            return arrayColdHot;
        }


































        /// Modifies an existing ImageData class
        public static ToolboxReturn HSL(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type != SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (!(
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_R) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_G) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_B)
                ))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                match(ref outp, inpt.getPrefferedSize(), new SectType[] { 
                    SectType.HSL_H,
                    SectType.HSL_S, 
                    SectType.HSL_L, 
                });

                Size sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var r = (inpt as SectHolder).Sects[SectType.RGB_R];
                var g = (inpt as SectHolder).Sects[SectType.RGB_G];
                var b = (inpt as SectHolder).Sects[SectType.RGB_B];

                Single[,] hue = ((outp as SectHolder).Sects[SectType.HSL_H] as SectArray).Data;
                Single[,] sat = ((outp as SectHolder).Sects[SectType.HSL_S] as SectArray).Data;
                Single[,] lum = ((outp as SectHolder).Sects[SectType.HSL_L] as SectArray).Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.rgb2hsl(
                            Math.Max(0, Math.Min(1, r[y, x])),
                            Math.Max(0, Math.Min(1, g[y, x])),
                            Math.Max(0, Math.Min(1, b[y, x])),
                            out hue[y, x],
                            out sat[y, x],
                            out lum[y, x]);
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        /// Modifies an existing ImageData class
        public static ToolboxReturn HSV(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type != SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (!(
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_R) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_G) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_B)
                ))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                match(ref outp, inpt.getPrefferedSize(), new SectType[] { 
                    SectType.HSV_H,
                    SectType.HSV_S, 
                    SectType.HSV_V, 
                });

                Size sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var r = (inpt as SectHolder).Sects[SectType.RGB_R];
                var g = (inpt as SectHolder).Sects[SectType.RGB_G];
                var b = (inpt as SectHolder).Sects[SectType.RGB_B];

                Single[,] hue = ((outp as SectHolder).Sects[SectType.HSV_H] as SectArray).Data;
                Single[,] sat = ((outp as SectHolder).Sects[SectType.HSV_S] as SectArray).Data;
                Single[,] val = ((outp as SectHolder).Sects[SectType.HSV_V] as SectArray).Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.rgb2hsv(
                            Math.Max(0, Math.Min(1, r[y, x])),
                            Math.Max(0, Math.Min(1, g[y, x])),
                            Math.Max(0, Math.Min(1, b[y, x])),
                            out hue[y, x],
                            out sat[y, x],
                            out val[y, x]);
                    }
                }

                return ToolboxReturn.Good;
            }
        }








        const int mask_sub = 268697856;   // 00 01 00000000 01 00000000 01 00000000
        const int mask_roll = 537395712;  // 00 10 00000000 10 00000000 10 00000000
        const int mask_rolli = 536346111; // 00 01 11111111 01 11111111 01 11111111
        const int mask_eq = 267648255;    // 00 00 11111111 00 11111111 00 11111111
        const int mask_9 = 511 ;

        public struct point_data
        {
            public int bin, bin1, bin2, bin3;
            public int loc, loc1, loc2, loc3;
            public int count;

            public point_data(Single f1, Single f2, Single f3, int bin_count)
            {
                this.bin1 = Math.Max((int)(f1 * bin_count), bin_count - 1);
                this.bin2 = Math.Max((int)(f2 * bin_count), bin_count - 1);
                this.bin3 = Math.Max((int)(f3 * bin_count), bin_count - 1);

                this.loc1 = (int)(255 * f1);
                this.loc2 = (int)(255 * f2);
                this.loc3 = (int)(255 * f3);

                this.loc = point_data.getShift(loc1, loc2, loc3);
                this.bin = point_data.getShift(bin1, bin2, bin3);

                this.count = 1;
            }

            public out_rgb getOutput()
            {
                return new out_rgb(this.loc1 / 255.0f, this.loc2 / 255.0f, this.loc3 / 255.0f);
            }

            public static int getShift(int bin1, int bin2, int bin3)
            {
                return (bin1 << 20) | (bin2 << 10) | bin3;
            }
        }

        public struct out_rgb
        {
            public Single r, g, b;
            public out_rgb(Single r, Single g, Single b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }

        public static ToolboxReturn MeanShiftRGB(Sect inpt, Single radius, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type != SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if ((inpt.max > 1) || (inpt.min < 0))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (!(
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_R) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_G) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_B)
                ))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Console.WriteLine(" ");
                Console.WriteLine("Starting");
                var stp = new System.Diagnostics.Stopwatch();
                stp.Start();

                Single r2 = radius * radius;

                match(ref outp, inpt.getPrefferedSize(), new SectType[] { 
                    SectType.RGB_R,
                    SectType.RGB_G, 
                    SectType.RGB_B, 
                });

                Size sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var rin = (inpt as SectHolder).Sects[SectType.RGB_R];
                var gin = (inpt as SectHolder).Sects[SectType.RGB_G];
                var bin = (inpt as SectHolder).Sects[SectType.RGB_B];

                var rout = (outp as SectHolder).Sects[SectType.RGB_R];
                var gout = (outp as SectHolder).Sects[SectType.RGB_G];
                var bout = (outp as SectHolder).Sects[SectType.RGB_B];

                int bin_count = 15;
                var input_locs = new int[h, w];
                var location_data = new Dictionary<int, point_data>();

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        point_data pd = new point_data(rin[y, x], gin[y, x], bin[y, x], bin_count);
                        input_locs[y, x] = pd.loc;
                        point_data pdo;
                        if (location_data.TryGetValue(pd.loc, out pdo)) pdo.count++;
                        else pdo = pd;
                        location_data[pd.loc] = pdo;
                    }
                }

                var location_bins = new Dictionary<int, List<point_data>>();
                var location_bins_ = new Dictionary<int, point_data[]>();

                foreach (var pd in location_data.Values)
                {
                    List<point_data> ls;
                    if (!location_bins.TryGetValue(pd.bin, out ls))
                    {
                        ls = new List<point_data>();
                        location_bins[pd.bin] = ls;
                    }
                    ls.Add(pd);
                }

                foreach (var key in location_bins.Keys.ToArray())
                    location_bins_[key] = location_bins[key].ToArray();

                var point_map = new Dictionary<int, out_rgb>();

                int radius_cut2 = (int)(radius * radius * 255 * 255);
                int prop_count = (int)Math.Ceiling(radius * bin_count);


                Console.WriteLine("Prep: " + stp.Elapsed); stp.Restart();

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var re = IA_Single.getMappingFrom(
                            location_data[input_locs[y, x]],
                            prop_count,
                            radius_cut2,
                            bin_count,
                            location_bins_,
                            point_map);
                        rout[y, x] = re.r;
                        gout[y, x] = re.g;
                        bout[y, x] = re.b;
                    }
                }


                Console.WriteLine("Find: " + stp.Elapsed);
                return ToolboxReturn.Good;
            }
        }

        public static out_rgb getMappingFrom(
            point_data pd,
            int prop_count,
            int radius2_cut,
            int bin_count,
            Dictionary<int, point_data[]> location_bins,
            Dictionary<int, out_rgb> point_map)
        {
            out_rgb ret;
            if (!point_map.TryGetValue(pd.loc, out ret))
            {
                int d1, d2, d3;
                int n1 = 0, n2 = 0, n3 = 0;
                int c = 0;

                point_data[] bin;
                for (int i1 = -prop_count; i1 <= prop_count; i1++)
                    for (int i2 = -prop_count; i2 <= prop_count; i2++)
                        for (int i3 = -prop_count; i3 <= prop_count; i3++)
                            if (location_bins.TryGetValue((((pd.bin1 + i1) & mask_9) << 20) |
                                                          (((pd.bin2 + i2) & mask_9) << 10) |
                                                           ((pd.bin3 + i3) & mask_9), out bin))
                            {
                                foreach (point_data pdi in bin)
                                {
                                    d1 = pd.loc1 - pdi.loc1;
                                    d2 = pd.loc2 - pdi.loc2;
                                    d3 = pd.loc3 - pdi.loc3;
                                    if (d1 * d1 + d2 * d2 + d3 * d3 < radius2_cut)
                                    {
                                        n1 += pdi.loc1 * pdi.count;
                                        n2 += pdi.loc2 * pdi.count;
                                        n3 += pdi.loc3 * pdi.count;
                                        c += pdi.count;
                                    }
                                }
                            }

                Single cf = c * 255.0f;
                var pd_next = new point_data(n1 / cf, n2 / cf, n3 / cf, bin_count);
                if (pd.loc == pd_next.loc) ret = pd.getOutput();
                else ret = IA_Single.getMappingFrom(
                    pd_next, 
                    prop_count,
                    radius2_cut, 
                    bin_count, 
                    location_bins,
                    point_map);

                point_map[pd.loc] = ret;
            }
            return ret;
        }



        public static ToolboxReturn MeanShiftRGB_OLD(Sect inpt, Single radius, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type != SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (!(
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_R) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_G) &&
                (inpt as SectHolder).Sects.ContainsKey(SectType.RGB_B)
                ))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Console.WriteLine(" ");
                Console.WriteLine("Starting");
                var stp = new System.Diagnostics.Stopwatch();
                stp.Start();

                Single r2 = radius * radius;

                match(ref outp, inpt.getPrefferedSize(), new SectType[] { 
                    SectType.RGB_R,
                    SectType.RGB_G, 
                    SectType.RGB_B, 
                });

                Size sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var rin = (inpt as SectHolder).Sects[SectType.RGB_R];
                var gin = (inpt as SectHolder).Sects[SectType.RGB_G];
                var bin = (inpt as SectHolder).Sects[SectType.RGB_B];

                var rout = (outp as SectHolder).Sects[SectType.RGB_R];
                var gout = (outp as SectHolder).Sects[SectType.RGB_G];
                var bout = (outp as SectHolder).Sects[SectType.RGB_B];


                Single radius_255 = radius * 255;
                int radius_cutt = (int)(radius_255 * radius_255);
                var point_locations = new Dictionary<Tuple<int, int, int>, int>();
                var inp_tuples = new Tuple<int, int, int>[h, w];

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var tp = new Tuple<int, int, int>(
                            (int)Math.Round(255 * rin[y, x]),
                            (int)Math.Round(255 * gin[y, x]),
                            (int)Math.Round(255 * bin[y, x]));

                        inp_tuples[y, x] = tp;

                        int outi;
                        if (point_locations.TryGetValue(tp, out outi)) point_locations[tp] = outi + 1;
                        else point_locations[tp] = 1;
                    }
                }

                var point_bins_l = new Dictionary<Tuple<int, int, int>, List<Tuple<int, int, int, int>>>();
                
                foreach (var kvp in point_locations)
                {
                    var new_value = new Tuple<int, int, int, int>(
                        kvp.Key.Item1,
                        kvp.Key.Item2,
                        kvp.Key.Item3,
                        kvp.Value);

                    var new_key = new Tuple<int, int, int>(
                        (int)Math.Round(kvp.Key.Item1 / radius_255),
                        (int)Math.Round(kvp.Key.Item2 / radius_255),
                        (int)Math.Round(kvp.Key.Item3 / radius_255));

                    List<Tuple<int, int, int, int>> ls;

                    if (point_bins_l.TryGetValue(new_key, out ls))
                    {
                        ls.Add(new_value);
                    }
                    else
                    {
                        ls = new List<Tuple<int, int, int, int>>();
                        ls.Add(new_value);
                        point_bins_l[new_key] = ls;
                    }                       
                }

                var point_bins = new Dictionary<Tuple<int, int, int>, Tuple<int, int, int, int>[]>();
                foreach (var kvp in point_bins_l) point_bins[kvp.Key] = kvp.Value.ToArray();

                Console.WriteLine("Prep: " + stp.Elapsed);
                stp.Restart();

                var point_map = new Dictionary<Tuple<int, int, int>, Tuple<int, int, int>>();
                int res = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var ip = inp_tuples[y, x];
                        var re = IA_Single.getMappingFrom(point_bins, point_map, ip, radius_255, radius_cutt, ref res);
                        rout[y, x] = re.Item1 / 255.0f;
                        gout[y, x] = re.Item2 / 255.0f;
                        bout[y, x] = re.Item3 / 255.0f;
                    }
                }


                Console.WriteLine("Find: " + stp.Elapsed);
                Console.WriteLine("Point Compression: " + ((Single)(point_locations.Count)) / (w * h));
                Console.WriteLine("Points Per Bin: " + ((Single)(point_locations.Count)) / (point_bins_l.Count));
                Console.WriteLine("Bins: " + point_bins_l.Count);


                return ToolboxReturn.Good;
            }
        }


        public static Tuple<int, int, int> getMappingFrom(
            Dictionary<Tuple<int, int, int>, Tuple<int, int, int, int>[]> point_bins,
            Dictionary<Tuple<int, int, int>, Tuple<int, int, int>> point_map,
            Tuple<int, int, int> loc,
            Single radius_255,
            int radius_cuttoff,
            ref int reused)
        {
            Tuple<int, int, int> ret;
            if (point_map.TryGetValue(loc, out ret))
            {
                reused++;
                return ret;
            }
            else
            {
                var bin1 = (int)Math.Round(loc.Item1 / radius_255);
                var bin2 = (int)Math.Round(loc.Item2 / radius_255);
                var bin3 = (int)Math.Round(loc.Item3 / radius_255);

                int n1 = 0;
                int n2 = 0;
                int n3 = 0;
                int d1 = 0;
                int d2 = 0;
                int d3 = 0;
                int d_all_2 = 0;
                int count = 0;

                Tuple<int, int, int, int>[] cbin;

                for (int i1 = -1; i1 < 2; i1++)
                    for (int i2 = -1; i2 < 2; i2++)
                        for (int i3 = -1; i3 < 2; i3++)
                        {
                            if (point_bins.TryGetValue(
                                new Tuple<int, int, int>(i1 + bin1, i2 + bin2, i3 + bin3),
                                out cbin))
                            {
                                foreach (var pt in cbin)
                                {
                                    d1 = loc.Item1 - pt.Item1;
                                    d2 = loc.Item2 - pt.Item2;
                                    d3 = loc.Item3 - pt.Item3;
                                    d_all_2 = d1 * d1 + d2 * d2 + d3 * d3;
                                    if (d_all_2 < radius_cuttoff)
                                    {
                                        int cc = pt.Item4;
                                        n1 += cc * pt.Item1;
                                        n2 += cc * pt.Item2;
                                        n3 += cc * pt.Item3;
                                        count += cc;
                                    }
                                }
                            }
                        }

                var value_next = new Tuple<int, int, int>(n1 / count, n2 / count, n3 / count);
                if (value_next.Equals(loc))
                {
                    point_map[loc] = loc;
                    return loc;
                }
                else
                {
                    value_next = IA_Single.getMappingFrom(point_bins, point_map, value_next, radius_255, radius_cuttoff, ref reused);
                    point_map[loc] = value_next;
                    return value_next;
                }                
            }
        }













    }
}
