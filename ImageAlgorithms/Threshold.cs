using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class ImageAlgorithms
    {
        public static ToolboxReturn Threshold(Sect inpt, Single thresh, ref Sect outp)
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
                                Threshold_(sh1.getSect(st), thresh, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Threshold_(inpt, thresh, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Threshold_(Sect inpt, Single thresh, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    outp[y, x] = inpt[y, x] < thresh ? 0 : 1;

        }
    }
}
