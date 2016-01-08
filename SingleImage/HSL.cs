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
        public static ToolboxReturn RGB2HSL(Sect inpt, ref Sect outp)
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

            var sh_in = inpt as SectHolder;

            if (!sh_in.hasRGB())
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz = inpt.getPrefferedSize();

                var R = sh_in.Sects[SectType.RGB_R];
                var G = sh_in.Sects[SectType.RGB_G];
                var B = sh_in.Sects[SectType.RGB_B];

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.Hue, SectType.HSL_S, SectType.HSL_L);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var hue = (sh_out.Sects[SectType.Hue] as SectArray).Data;
                var sat = (sh_out.Sects[SectType.HSL_S] as SectArray).Data;
                var lum = (sh_out.Sects[SectType.HSL_L] as SectArray).Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.rgb2hsl(
                            Math.Max(0, Math.Min(1, R[y, x])),
                            Math.Max(0, Math.Min(1, G[y, x])),
                            Math.Max(0, Math.Min(1, B[y, x])),
                            out hue[y, x],
                            out sat[y, x],
                            out lum[y, x]);
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        public static ToolboxReturn HSL2RGB(Sect inpt, ref Sect outp)
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

            var sh_in = inpt as SectHolder;

            if (!sh_in.hasHSL())
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz = inpt.getPrefferedSize();

                var H = sh_in.Sects[SectType.Hue];
                var S = sh_in.Sects[SectType.HSL_S];
                var L = sh_in.Sects[SectType.HSL_L];

                MatchOutputToSizeAndSectTypes(ref outp, sz, SectType.RGB_R, SectType.RGB_G, SectType.RGB_B);

                int w = sz.Width;
                int h = sz.Height;

                var sh_out = outp as SectHolder;

                var R = (sh_out.Sects[SectType.RGB_R] as SectArray).Data;
                var G = (sh_out.Sects[SectType.RGB_G] as SectArray).Data;
                var B = (sh_out.Sects[SectType.RGB_B] as SectArray).Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.hsl2rgb(
                            Math.Max(0, Math.Min(1, H[y, x])),
                            Math.Max(0, Math.Min(1, S[y, x])),
                            Math.Max(0, Math.Min(1, L[y, x])),
                            out R[y, x],
                            out G[y, x],
                            out B[y, x]);
                    }
                }

                return ToolboxReturn.Good;
            }
        }
    }
}
