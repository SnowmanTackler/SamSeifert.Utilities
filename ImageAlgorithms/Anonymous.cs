using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class ImageAlgorithms
    {
        public static ToolboxReturn Anonymous(Sect inpt, Func<float, float> f, ref Sect outp)
        {
            if ((inpt == null) || (f == null))
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    var sz = anon_outp.getPrefferedSize();
                    int w = sz.Width;
                    int h = sz.Height;

                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            anon_outp[y, x] = f(anon_inpt[y, x]);
                };

                ImageAlgorithms.MatchOutputToInput(inpt, ref outp);
                ImageAlgorithms.Do1v1Action(inpt, ref outp, act);

                return ToolboxReturn.Good;
            }
        }
    }
}
