using SamSeifert.Utilities; using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ComputerVision
{
    public static partial class MultipleImages
    {
        /// <summary>
        /// Doesn't matter which input is what size, but one should be conclusively smaller than the other.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filter"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn HeatEquation(Sect image, Sect mask, int padding, int iterations, ref Sect outp)
        {
            if (image == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (mask == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (mask._Type == SectType.Holder)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                Size sz_image = image.getPrefferedSize();
                Size sz_mask = mask.getPrefferedSize();

                int filter_width = padding * 2 + 1;

                if (padding < 0)
                {
                    // Filter needs to be square, and odd dimension > 0
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }

                var sigma = filter_width / 4.0f;
                sigma *= sigma;

                Sect filter = SectArray.Build.Gaussian.NormalizedSum1D(SectType.Gray, sigma, filter_width);
                Sect filterT = filter.Transpose();

                if (sz_image != sz_mask)
                {
                    // Image and mask need to be same size.
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }
                else if ((sz_image.Width < filter_width) || (sz_image.Height < filter_width))
                {
                    // Filter Needs to be smaller than image
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }

                var ret = MatchSectTypes(new Sect[] { image, filter }, ref outp, sz_image);
                if (ret != ToolboxReturn.Good) return ret;


                Sect padded_mask = null;
                Sect padded_image_1 = null;

                SingleImage.PaddingAdd(mask, PaddingType.Extend, padding, ref padded_mask);
                SingleImage.PaddingAdd(image, PaddingType.Extend, padding, ref padded_image_1);

                Sect padded_image_2 = padded_image_1.Clone();
                Sect padded_image_3 = padded_image_1.Clone();

                Action< Sect, Sect, Sect> act_middle = (Sect anon_image, Sect anon_filter, Sect anon_outp) =>
                {
                    // In an ideal world the smaller input would have an odd (height) and (width)

                    Size anon_filter_size = anon_filter.getPrefferedSize();
                    Size anon_image_size = anon_image.getPrefferedSize();
                    int x1_start = (anon_filter_size.Width - 1) / 2;
                    int x1_end = anon_image_size.Width - x1_start;
                    int y1_start = (anon_filter_size.Height - 1) / 2;
                    int y1_end = anon_image_size.Height - y1_start;

                    int tx, ty;
                    float sum;

                    // Set middle:
                    for (int y1 = y1_start; y1 < y1_end; y1++)
                    {
                        for (int x1 = x1_start; x1 < x1_end; x1++)
                        {
                            if (padded_mask[y1, x1] == 0)
                            {
                                sum = 0;
                                for (int y2 = 0; y2 < anon_filter_size.Height; y2++)
                                {
                                    ty = y1 + y2 - y1_start;
                                    for (int x2 = 0; x2 < anon_filter_size.Width; x2++)
                                    {
                                        tx = x1 + x2 - x1_start;
                                        sum += anon_image[ty, tx] * anon_filter[y2, x2];
                                    }
                                }
                                anon_outp[y1, x1] = sum;
                            }
                        }
                    }
                };

                if (image._Type == SectType.Holder)
                {                                                       
                    for (int i = 0; i < iterations; i++)
                    {
                        foreach (var st in (image as SectHolder).getSectTypes())
                        {
                            var s1 = (padded_image_1 as SectHolder).Sects[st];
                            var s2 = (padded_image_2 as SectHolder).Sects[st];
                            var s3 = (padded_image_3 as SectHolder).Sects[st];
                            act_middle(s1, filter, s2);
                            act_middle(s2, filterT, s3);
                        }
                        Utilities.Misc.Util.Swap(ref padded_image_1, ref padded_image_3);
                    }
                }
                else
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        act_middle(padded_image_1, filter, padded_image_2);
                        act_middle(padded_image_2, filterT, padded_image_3);
                        Utilities.Misc.Util.Swap(ref padded_image_1, ref padded_image_3);
                    }
                }

                SingleImage.PaddingOff(padded_image_1, padding, ref outp);
                return ToolboxReturn.Good;
            }
        }
    }
}
