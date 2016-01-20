using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class MultipleImages
    {
        /// <summary>
        /// Doesn't matter which input is what size, but one should be conclusively smaller than the other.
        /// </summary>
        /// <param name="in1"></param>
        /// <param name="in2"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Dilate(Sect in1, Sect in2, ref Sect outp)
        {
            return ErodeDilate(in1, in2, ref outp, false);
        }

        /// <summary>
        /// Doesn't matter which input is what size, but one should be conclusively smaller than the other.
        /// </summary>
        /// <param name="in1"></param>
        /// <param name="in2"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Erode(Sect in1, Sect in2, ref Sect outp)
        {
            return ErodeDilate(in1, in2, ref outp, true);
        }

        private static ToolboxReturn ErodeDilate(Sect in1, Sect in2, ref Sect outp, bool erode)
        {
            if (in1 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (in2 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Size s1 = in1.getPrefferedSize();
                Size s2 = in2.getPrefferedSize();

                if ((s1.Width >= s2.Width) && (s1.Height >= s2.Height))
                {
                    // Submitted in correct order
                }
                else if ((s1.Width <= s2.Width) && (s1.Height <= s2.Height))
                {
                    // Submitted in incorrect order, so just swap them
                    SamSeifert.Utilities.Helpers.Swap(ref s1, ref s2);
                    SamSeifert.Utilities.Helpers.Swap(ref in1, ref in2);
                }
                else
                {
                    // Neither is distinctly smaller.
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }

                var ret = MatchSectTypes(new Sect[] { in1, in2 }, ref outp, s1);
                if (ret != ToolboxReturn.Good) return ret;

                // In an ideal world the smaller input would have an odd (height) and (width)
                int x1_start = (s2.Width - 1) / 2;
                int x1_end = s1.Width - (s2.Width - x1_start - 1);
                int y1_start = (s2.Height - 1) / 2;
                int y1_end = s1.Height - (s2.Height - y1_start - 1);

                Action<Sect, Sect, Sect> act;
                
                if (erode) act = (Sect anon_in1, Sect anon_in2, Sect anon_outp) =>
                {
                    int tx, ty;
                    float sum;

                    for (int y1 = y1_start; y1 < y1_end; y1++)
                    {
                        for (int x1 = x1_start; x1 < x1_end; x1++)
                        {
                            sum = 0;
                            for (int y2 = 0; y2 < s2.Height; y2++)
                            {
                                ty = y1 + y2 - y1_start;
                                for (int x2 = 0; x2 < s2.Width; x2++)
                                {
                                    tx = x1 + x2 - x1_start;
                                    sum += anon_in1[ty, tx] * anon_in2[y2, x2];
                                }
                            }
                            anon_outp[y1, x1] = sum == 0 ? 0 : 1;
                        }
                    }
                };
                else act = (Sect anon_in1, Sect anon_in2, Sect anon_outp) =>
                {
                    int tx, ty;
                    float sum;

                    float sum_in2 = 0;

                    for (int y = 0; y < s2.Height; y++)
                        for (int x = 0; x < s2.Width; x++)
                            sum_in2 += anon_in2[y, x];

                    for (int y1 = y1_start; y1 < y1_end; y1++)
                    {
                        for (int x1 = x1_start; x1 < x1_end; x1++)
                        {
                            sum = 0;
                            for (int y2 = 0; y2 < s2.Height; y2++)
                            {
                                ty = y1 + y2 - y1_start;
                                for (int x2 = 0; x2 < s2.Width; x2++)
                                {
                                    tx = x1 + x2 - x1_start;
                                    sum += anon_in1[ty, tx] * anon_in2[y2, x2];
                                }
                            }
                            anon_outp[y1, x1] = (sum > sum_in2 - 0.1f) ? 1 : 0;
                        }
                    }
                };


                if (outp._Type == SectType.Holder)
                {
                    foreach (var sect_out in (outp as SectHolder).Sects.Values)
                    {
                        Sect sect_in1 = in1;
                        if (sect_in1._Type == SectType.Holder) sect_in1 = (sect_in1 as SectHolder).Sects[sect_out._Type];
                        Sect sect_in2 = in2;
                        if (sect_in2._Type == SectType.Holder) sect_in2 = (sect_in2 as SectHolder).Sects[sect_out._Type];
                        act(sect_in1, sect_in2, sect_out);
                    }
                }
                else
                {
                    act(in1, in2, outp);
                }

                return ToolboxReturn.Good;
            }
        }
    }
}
