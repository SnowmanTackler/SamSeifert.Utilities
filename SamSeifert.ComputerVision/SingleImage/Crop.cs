using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SamSeifert.CSCV
{
    public static partial class SingleImage
    {
        public static ToolboxReturn Crop(Sect inpt, Rectangle rect, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if ((rect.Width <= 0) || (rect.Height <= 0))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    int h = rect.Height;
                    int w = rect.Width;

                    Size sz_in = anon_inpt.getPrefferedSize();
                    int refH = sz_in.Height;
                    int refW = sz_in.Width;

                    anon_outp.SetValue(0);

                    int min_x = Math.Max(0, rect.X);
                    int max_x = Math.Min(refW, rect.X + rect.Width);

                    int min_y = Math.Max(0, rect.Y);
                    int max_y = Math.Min(refH, rect.Y + rect.Height);

                    for (int y = min_y; y < max_y; y++)
                        for (int x = min_x; x < max_x; x++)
                            anon_outp[y - rect.Y, x - rect.X] = anon_inpt[y, x];
                };

                SingleImage.MatchOutputToInput(inpt, ref outp, rect.Size);
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
