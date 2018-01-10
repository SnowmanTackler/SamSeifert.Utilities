using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SamSeifert.Utilities;

namespace SamSeifert.CSCV
{
    public enum NoiseType
    {
        Gaussian,
        Uniform,

        [Description("Salt and Pepper")]
        SaltAndPepper
    };

    public enum RandomSeed
    {
        TimeVarying,
        Constant
    }

    public static partial class SingleImage
    {
        public static ToolboxReturn Noise(Sect inpt, NoiseType nt, RandomSeed rs, Single p, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                int seed = Environment.TickCount;

                if (rs == RandomSeed.Constant)
                    seed = 0;

                Random r = new Random(seed);               

                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {

                    var sz = anon_outp.getPrefferedSize();
                    int w = sz.Width;
                    int h = sz.Height;

                    switch (nt)
                    {
                        case NoiseType.Gaussian:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        anon_outp[y, x] = anon_inpt[y, x] + Utilities.Statistics.NextGaussian(r, p);
                                    }
                                }
                                break;
                            }
                        case NoiseType.Uniform:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        anon_outp[y, x] = anon_inpt[y, x] + p * (1 - 2 *  (float)r.NextDouble());
                                    }
                                }
                                break;
                            }
                        case NoiseType.SaltAndPepper:
                            {
                                r = new Random(0); // So each image has same pixels firing
                                p = p.Clampp(0, 1);
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        float rand = (float)r.NextDouble();
                                        anon_outp[y, x] = (rand * 2 < p) ? 1 : (rand < p) ? 0 : anon_inpt[y, x];
                                    }
                                }
                                break;
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
