using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class ImageAlgorithms
    {
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
                    outp[y, x] = Helpers.Clamp(((Single)Math.Round(inpt[y, x] * mult - 0.5f)) / div, 0, 1);
                }
            }
        }
    }
}
