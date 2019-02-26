using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSeifert.Utilities;

namespace SamSeifert.CSCV
{
    public static partial class SingleImage
    {
        public static ToolboxReturn RGB2HSL(SectHolder inpt, ref Sect outp)
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

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.Hue, SectType.HSL_S, SectType.HSL_L);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var hue = (sh_out.Sects[SectType.Hue] as SectArray).Data;
                var sat = (sh_out.Sects[SectType.HSL_S] as SectArray).Data;
                var lum = (sh_out.Sects[SectType.HSL_L] as SectArray).Data;

                float hh, ss, ll;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorUtil.rgb2hsl(
                            Math.Max(0, Math.Min(1, R[y, x])),
                            Math.Max(0, Math.Min(1, G[y, x])),
                            Math.Max(0, Math.Min(1, B[y, x])),
                            out hh,
                            out ss,
                            out ll);

                        hue[y, x] = hh;
                        sat[y, x] = ss;
                        lum[y, x] = ll;
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        public static ToolboxReturn HSL2RGB(SectHolder inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (!inpt.hasHSL())
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz = inpt.getPrefferedSize();

                var H = inpt.Sects[SectType.Hue];
                var S = inpt.Sects[SectType.HSL_S];
                var L = inpt.Sects[SectType.HSL_L];

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.RGB_R, SectType.RGB_G, SectType.RGB_B);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var R = (sh_out.Sects[SectType.RGB_R] as SectArray).Data;
                var G = (sh_out.Sects[SectType.RGB_G] as SectArray).Data;
                var B = (sh_out.Sects[SectType.RGB_B] as SectArray).Data;

                float rr, gg, bb;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorUtil.hsl2rgb(
                            H[y, x],
                            Math.Max(0, Math.Min(1, S[y, x])),
                            Math.Max(0, Math.Min(1, L[y, x])),
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
