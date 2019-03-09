using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ComputerVision
{
    public static partial class SingleImage
    {
        public static ToolboxReturn Anonymous(
            Sect inpt,
            ref Sect outp,
            Func<float, float> f
            )
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

                SingleImage.MatchOutputToInput(inpt, ref outp);
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }

        public static ToolboxReturn Anonymous(
            Sect inpt,
            Func<float, float> f
            )
        {
            if ((inpt == null) || (f == null))
            {
                return ToolboxReturn.NullInput;
            }
            else
            {
                Action<Sect> act = (Sect anon_inpt) =>
                {
                    var sz = anon_inpt.getPrefferedSize();
                    int w = sz.Width;
                    int h = sz.Height;

                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            anon_inpt[y, x] = f(anon_inpt[y, x]);
                };

                if (inpt._Type == SectType.Holder)
                {
                    foreach (var sect in (inpt as SectHolder).Sects.Values)
                    {
                        if (sect.Isnt<SectArray>()) return ToolboxReturn.SpecialError;
                        act(sect);
                    }
                }
                else
                {
                    if (inpt.Isnt<SectArray>()) return ToolboxReturn.SpecialError;
                    act(inpt);
                }

                return ToolboxReturn.Good;
            }
        }
    }
}
