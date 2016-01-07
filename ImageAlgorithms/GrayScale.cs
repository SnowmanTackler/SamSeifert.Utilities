using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public enum GrayscaleType
    {
        Mean,
        Maximum,
        Minimum
    };

    public static partial class ImageAlgorithms
    {
        public static ToolboxReturn Grayscale(Sect inpt, GrayscaleType t, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                MatchOutputToSizeAndSectTypes(ref outp, inpt.getPrefferedSize(), new SectType[] { SectType.Gray });

                var sz = outp.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                if (inpt._Type == SectType.Holder)
                {
                    var sh = inpt as SectHolder;
                    switch (t)
                    {
                        case GrayscaleType.Maximum:
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
                        case GrayscaleType.Minimum:
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
                        case GrayscaleType.Mean:
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
    }
}
