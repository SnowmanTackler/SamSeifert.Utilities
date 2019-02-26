using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class SingleImage
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
                float mult = (float)Math.Pow(2, bpp);
                float div = mult - 1;

                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    var sz = anon_outp.getPrefferedSize();
                    int w = sz.Width;
                    int h = sz.Height;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            anon_outp[y, x] = Helpers.Clamp(((Single)Math.Round(anon_inpt[y, x] * mult - 0.5f)) / div, 0, 1);
                        }
                    }
                };

                SingleImage.MatchOutputToInput(inpt, ref outp);
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
