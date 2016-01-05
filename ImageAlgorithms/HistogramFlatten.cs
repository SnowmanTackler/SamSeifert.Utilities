using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class ImageAlgorithms
    {
        public static ToolboxReturn HistogramFlatten(Sect inpt, ref Sect outp)
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
                                HistogramFlatten(sh1.getSect(st), sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        HistogramFlatten(inpt, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void HistogramFlatten(Sect inpt, SectArray outp)
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
                    counts[255 + Helpers.Cast(inpt[y, x] * 255)]++;
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
                    outp[y, x] = _Bytes[255 + Helpers.Cast(inpt[y, x] * 255)] / 255.0f;
                }
            }
        }
    }
}
