using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public class HarrisCornersMemory
    {
        internal Sect PadI, PadIx, PadIy, PadIxy;

        internal HarrisCornersMemory() { }
    }

    public static partial class SingleImage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inpt">Input</param>
        /// <param name="alpha">If M is weighted sum of [Ixx Ixy; Ixy Iy], harris value = det(M) - alpha * trace(M)^2;</param>
        /// <param name="span">Should be Odd!  Filter size for ix, iy, ixy summation</param>
        /// <param name="memory">Store the goods</param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn HarrisCorners(
            Sect inpt,
            float alpha,
            int span,
            ref HarrisCornersMemory memory, 
            ref Sect outp)
        {
            if (span % 2 == 0)
            {
                return ToolboxReturn.SpecialError;
            }
            else if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                if (memory == null) memory = new HarrisCornersMemory();

                int pad_add = Math.Max(1, // Need at least one for Ix and Iy
                    (span - 1) / 2);

                SingleImage.PaddingAdd(inpt, PaddingType.Extend, pad_add, ref memory.PadI);

                var gx = SectArray.Build.FromArray(SectType.Gray, 3, 1, new float[] { -0.5f, 0, 0.5f });  //0.5 because dx = 2
                var gy = gx.Transpose();

                MultipleImages.Convolute(memory.PadI, gx, ref memory.PadIx);
                MultipleImages.Convolute(memory.PadI, gy, ref memory.PadIy);

                var sz = inpt.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                SingleImage.MatchOutputToInput(memory.PadI, ref memory.PadIxy);
                SingleImage.DoAction1vN(ref memory.PadIxy, (Sect[] anon_inpt, SectArray anon_outp) =>
                {
                    var aix = anon_inpt[0] as SectArray;
                    var aiy = anon_inpt[1] as SectArray;

                    for (int _y = 0, y = pad_add; _y < h; _y++, y++)
                    {
                        for (int _x = 0, x = pad_add; _x < w; _x++, x++)
                        {
                            float vx = aix[y, x];
                            float vy = aiy[y, x];
                            anon_outp[y, x] = vx * vy;
                            aix[y, x] = vx * vx;
                            aiy[y, x] = vy * vy;
                        }
                    }
                }, memory.PadIx, memory.PadIy);

                SingleImage.PaddingUpdateExtend(pad_add, memory.PadIx);
                SingleImage.PaddingUpdateExtend(pad_add, memory.PadIy);
                SingleImage.PaddingUpdateExtend(pad_add, memory.PadIxy);

                var sigma = span / 4.0f;
                sigma *= sigma;

                var weights = SectArray.Build.Gaussian.NormalizedMax2D(SectType.Gray, sigma, span);


                SingleImage.MatchOutputToInput(inpt, ref outp);
                SingleImage.DoAction1vN(ref outp, (Sect[] anon_inpt, SectArray anon_outp) =>
                {
                    var aixx = anon_inpt[0] as SectArray;
                    var aiyy = anon_inpt[1] as SectArray;
                    var aixy = anon_inpt[2] as SectArray;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            float sum_ixx = 0;
                            float sum_ixy = 0;
                            float sum_iyy = 0;

                            for (int dy = 0; dy < span; dy++)
                            {
                                for (int dx = 0; dx < span; dx++)
                                {
                                    float weight = weights[dy, dx];
                                    sum_ixx += aixx[y + dy, x + dx] * weight;
                                    sum_iyy += aiyy[y + dy, x + dx] * weight;
                                    sum_ixy += aixy[y + dy, x + dx] * weight;
                                }
                            }

                            anon_outp[y, x] = sum_ixx * sum_iyy - sum_ixy * sum_ixy - alpha * (sum_ixx + sum_iyy);
                        }
                    }
                }, memory.PadIx, memory.PadIy, memory.PadIxy);

                return ToolboxReturn.Good;
            }
        }
    }
}
