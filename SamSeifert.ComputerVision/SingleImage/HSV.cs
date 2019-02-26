using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSeifert.Utilities;
using SamSeifert.Utilities.Extensions;

namespace SamSeifert.CSCV
{
    public partial class SingleImage
    {
        public static ToolboxReturn RGB2HSV(SectHolder inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (!inpt.hasRGB())
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz = inpt.getPrefferedSize();

                var R = inpt.Sects[SectType.RGB_R];
                var G = inpt.Sects[SectType.RGB_G];
                var B = inpt.Sects[SectType.RGB_B];

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.Hue, SectType.HSV_S, SectType.HSV_V);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var hue = (sh_out.Sects[SectType.Hue] as SectArray).Data;
                var sat = (sh_out.Sects[SectType.HSV_S] as SectArray).Data;
                var val = (sh_out.Sects[SectType.HSV_V] as SectArray).Data;

                float hh, ss, vv;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorE.rgb2hsv(
                            Math.Max(0, Math.Min(1, R[y, x])),
                            Math.Max(0, Math.Min(1, G[y, x])),
                            Math.Max(0, Math.Min(1, B[y, x])),
                            out hh,
                            out ss,
                            out vv);

                        hue[y, x] = hh;
                        sat[y, x] = ss;
                        val[y, x] = vv;

                    }
                }

                return ToolboxReturn.Good;
            }
        }

        public static ToolboxReturn HSV2RGB(SectHolder inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (!inpt.hasHSV())
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz = inpt.getPrefferedSize();

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.RGB_R, SectType.RGB_G, SectType.RGB_B);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var H = inpt.Sects[SectType.Hue];
                var S = inpt.Sects[SectType.HSV_S];
                var V = inpt.Sects[SectType.HSV_V];

                var R = (sh_out.Sects[SectType.RGB_R] as SectArray).Data;
                var G = (sh_out.Sects[SectType.RGB_G] as SectArray).Data;
                var B = (sh_out.Sects[SectType.RGB_B] as SectArray).Data;

                float rr, gg, bb;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorE.hsv2rgb(
                            H[y, x],
                            Math.Max(0, Math.Min(1, S[y, x])),
                            Math.Max(0, Math.Min(1, V[y, x])),
                            out rr,
                            out gg,
                            out bb);

                        R[y, x] = rr;
                        G[y, x] = gg;
                        B[y, x] = bb;
                    }
                }

                return ToolboxReturn.Good;
            }
        }
    }
}
