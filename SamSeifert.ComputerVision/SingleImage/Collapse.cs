using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SamSeifert.ComputerVision
{
    public static partial class SingleImage
    {
        public static ToolboxReturn Collapse(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Size sz_in = inpt.getPrefferedSize();

                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    Size sz_anon_in = anon_inpt.getPrefferedSize();                    
                    Size sz_anon_out = anon_outp.getPrefferedSize();

                    int w = sz_anon_in.Width;
                    int h = sz_anon_in.Height;

                    for (int x = 0; x < w; x++)
                    {
                        float sum = 0;
                        for (int y = 0; y < h; y++)
                        {
                            sum += anon_inpt[y, x];
                        }
                        anon_outp[0, x] = sum / h;
                    }
                };

                SingleImage.MatchOutputToInput(inpt, ref outp, new Size(sz_in.Width, 1));
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
