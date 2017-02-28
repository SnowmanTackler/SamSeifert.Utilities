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

    public static partial class SingleImage
    {
        public static ToolboxReturn KMeans(Sect inpt, int k, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                MatchOutputToInput(inpt, ref outp);

                var sz = outp.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var sects = new List<Sect>();

                if (inpt._Type == SectType.Holder)
                {
                    var sh = inpt as SectHolder;
                    foreach (var inp in sh.Sects.Values)
                        sects.Add(inp);
                }
                else sects.Add(inpt);

                int pixels = w * h;
            }
        }
    }
}
