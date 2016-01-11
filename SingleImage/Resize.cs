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
    public enum ResizeType
    {
        [Description("Nearest Neighbor")]
        NearestNeighbor,

        [Description("Bilinear")]
        Bilinear
    };

    public static partial class SingleImage
    {
        public static ToolboxReturn Resize(Sect inpt, Size sz_out, ResizeType t, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if ((sz_out.Width <= 0) || (sz_out.Height <= 0))
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    int h = sz_out.Height;
                    int w = sz_out.Width;


                    Size sz_in = anon_inpt.getPrefferedSize();
                    int refH = sz_in.Height;
                    int refW = sz_in.Width;

                    switch (t)
                    {
                        case ResizeType.NearestNeighbor:
                            {
                                float temp;
                                int yA, xA;

                                for (int y = 0; y < h; y++)
                                {
                                    temp = y;
                                    temp /= h;
                                    temp += 1.0f / (2 * h);
                                    // temp is now scaled 0 to 1 on large image 
                                    temp -= 1.0f / (2 * refH);
                                    temp *= refH;
                                    yA = Helpers.Clamp((int)Math.Round(temp), 0, refH - 1);

                                    for (int x = 0; x < w; x++)
                                    {
                                        temp = x;
                                        temp /= w;
                                        temp += 1.0f / (2 * w);
                                        // temp is now scaled 0 to 1 on large image 
                                        temp -= 1.0f / (2 * refW);
                                        temp *= refW;
                                        xA = Helpers.Clamp((int)Math.Round(temp), 0, refW - 1);

                                        anon_outp[y, x] = anon_inpt[yA, xA];
                                    }
                                }
                                break;
                            }
                        case ResizeType.Bilinear: // TODO: FIX BILINEAR (PIXELS ON BORDER)
                            {
                                int xUp, xDown, yUp, yDown;
                                float xAdj, yAdj;

                                for (int y = 0; y < h; y++)
                                {
                                    yAdj = y * (refH - 1);
                                    yAdj /= (h - 1);
                                    yUp = (int)Math.Ceiling((double)yAdj);
                                    yDown = (int)yAdj;

                                    for (int x = 0; x < w; x++)
                                    {
                                        xAdj = x * (refW - 1);
                                        xAdj /= Math.Max(1, (w - 1));
                                        xUp = (int)Math.Ceiling((double)xAdj);
                                        xDown = (int)xAdj;

                                        if (xUp == xDown && yUp == yDown)
                                        {
                                            anon_outp[y, x] = anon_inpt[yUp, xUp];
                                        }
                                        else if (xUp == xDown)
                                        {
                                            anon_outp[y, x] = Helpers.getLinearEstimate(
                                                anon_inpt[yDown, xUp],
                                                anon_inpt[yUp, xUp],
                                                yAdj % 1);
                                        }
                                        else if (yUp == yDown)
                                        {
                                            anon_outp[y, x] = Helpers.getLinearEstimate(
                                                anon_inpt[yUp, xDown],
                                                anon_inpt[yUp, xUp],
                                                xAdj % 1);
                                        }
                                        else
                                        {
                                            anon_outp[y, x] = Helpers.getLinearEstimate(
                                                Helpers.getLinearEstimate(
                                                    anon_inpt[yDown, xDown],
                                                    anon_inpt[yUp, xDown],
                                                    yAdj % 1),
                                                Helpers.getLinearEstimate(
                                                    anon_inpt[yDown, xUp],
                                                    anon_inpt[yUp, xUp],
                                                    yAdj % 1),
                                                xAdj % 1);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                };

                SingleImage.MatchOutputToInput(inpt, ref outp, sz_out);
                SingleImage.Do1v1Action(inpt, ref outp, act);

                return ToolboxReturn.Good;
            }
        }
    }
}
