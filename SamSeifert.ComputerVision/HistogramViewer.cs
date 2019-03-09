using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using SamSeifert.Utilities.Extensions;

namespace SamSeifert.ComputerVision
{
    public partial class HistogramViewer
    {
        public static unsafe void histogram(Sect inpt, int imh, Boolean autoscale, ref Bitmap bmp)
        {        
            if (imh < 1) return;

            int[,] histogram;

            HistogramViewer.fillHistogram(inpt, out histogram);

            int x;
            const int picWidth = 511;

            Boolean remake = bmp == null;;
            if (!remake) remake = bmp.Height != imh;
            if (remake)
            {
                if (bmp != null) bmp.Dispose();
                bmp = new Bitmap(picWidth, imh, PixelFormat.Format24bppRgb);
            }

            int max = 1;

            if (autoscale)
            {
                for (int c = 0; c < 3; c++)
                    for (x = 0; x < picWidth; x++)
                        max = Math.Max(max, histogram[c, x]);
            }
            else
            {
                var sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;
                max = Math.Max(max, w * h * 10 / 255);
            }
            
            for (int c = 0; c < 3; c++)
                for (x = 0; x < picWidth; x++)
                    histogram[c, x] = ((histogram[c, x]) * imh) / max;

            using (var bmd = bmp.Locked(
                ImageLockMode.ReadWrite,
                bmp.PixelFormat))
            {

                byte* row;
                int xx = 0;

                for (int c = 0; c < 3; c++)
                {
                    for (int y = 0; y < bmd.Height; y++)
                    {
                        row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (x = 0, xx = 2 - c; x < bmd.Width; x++, xx += 3)
                        {
                            if (y < histogram[c, x]) row[xx] = 255;
                            else row[xx] = 0;
                        }
                    }
                }
            }

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        private static void fillHistogram(Sect inpt, out int[,] counts)
        {
            int x, y;

            var sz = inpt.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            counts = new int[3, 511];

            Single r, g, b;

            int total_count = 0;

            var anonFunc = inpt.getColorFiller();

            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x++)
                {
                    anonFunc(y, x, out r, out g, out b);
                    counts[0, 255 + Helpers.Cast(r * 255.0f)]++;
                    counts[1, 255 + Helpers.Cast(g * 255.0f)]++;
                    counts[2, 255 + Helpers.Cast(b * 255.0f)]++;
                    total_count++;
                }
            }

            for (int i1 = 0; i1 < 3; i1++)
                if (counts[i1, 255] == total_count) counts[i1, 255] = 0;
        }
    }
}
