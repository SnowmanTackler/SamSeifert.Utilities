using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SamSeifert.CSCV
{
    public static partial class SingleImage
    {
        public static ToolboxReturn HueFilter(Sect inpt, float BandCenter, float BandWidth, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt._Type == SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                MatchOutputToSizeAndSectTypes(ref outp, inpt.getPrefferedSize(), new SectType[] { SectType.Gray });

                var sz = outp.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        outp[y, x] = SamSeifert.Utilities.ColorUtil.CheckHue(inpt[y, x], BandCenter, BandWidth) ? 1 : 0;

                return ToolboxReturn.Good;
            }
        }
    }
}
