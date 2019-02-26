using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ComputerVision
{
    public static partial class SingleImage
    {

        public static ToolboxReturn Gradient(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
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

                    var valsX = new Single[,]
                    {
                        {-1,  0,  1},
                        {-2,  0,  2},
                        {-1,  0,  1},
                    };

                    var valsY = new Single[,]
                    {
                        {-1, -2, -1},
                        { 0,  0,  0},
                        { 1,  2,  1},
                    };

                    const int dim = 3;
                    int ty;
                    float val;
                    float gx;
                    float gy;

                    for (int y = 1; y < h - 1; y++)
                    {
                        for (int x = 1; x < w - 1; x++)
                        {
                            gx = 0;
                            gy = 0;
                            for (int i = 0; i < dim; i++)
                            {
                                ty = y + i - 1;
                                for (int j = 0; j < dim; j++)
                                {
                                    val = anon_inpt[ty, x + j - 1];
                                    gx += val * valsX[i, j];
                                    gy += val * valsY[i, j];
                                }
                            }
                            anon_outp[y, x] = (Single)Math.Sqrt(gx * gx + gy * gy);
                        }
                    }

                    for (int y = 0; y < h; y++) // Edges
                    {
                        anon_outp[y, 0] = 0;
                        anon_outp[y, w - 1] = 0;
                    }

                    for (int x = 0; x < w; x++) // Edges
                    {
                        anon_outp[0, x] = 0;
                        anon_outp[h - 1, x] = 0;
                    }
                };

                SingleImage.MatchOutputToInput(inpt, ref outp);
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
