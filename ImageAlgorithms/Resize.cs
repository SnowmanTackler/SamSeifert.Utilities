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

    public static partial class ImageAlgorithms
    {
        public static ToolboxReturn Resize(Sect inpt, Size res, ResizeType t, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Boolean remake = outp == null;
                if (!remake) remake = (inpt._Type != outp._Type) || (res != outp.getPrefferedSize());
                if (!remake)
                {
                    switch (inpt._Type)
                    {
                        case SectType.Holder:
                            {
                                var k1 = (inpt as SectHolder).Sects.Keys;
                                var k2 = (outp as SectHolder).Sects.Keys;
                                remake = (k1.Count != k2.Count) || (k1.Except(k2).Any());
                                foreach (var val in (outp as SectHolder).Sects.Values)
                                {
                                    if (remake) break;
                                    else remake = !(val is SectArray);
                                }
                            }
                            break;
                        default:
                            remake = !(outp is SectArray);
                            break;
                    }
                }
                if (remake)
                {
                    switch (inpt._Type)
                    {
                        case SectType.Holder:
                            outp = new SectHolder(res, (inpt as SectHolder).getSectTypes());
                            break;
                        default:
                            outp = new SectArray(inpt._Type, res.Width, res.Height);
                            break;
                    }
                }
                else outp.reset();

                switch (inpt._Type)
                {
                    case SectType.Holder:
                        {
                            var sh1 = inpt as SectHolder;
                            var sh2 = outp as SectHolder;
                            foreach (var st in sh1.getSectTypes())
                                Resize_(sh1.getSect(st), res, t, sh2.getSect(st) as SectArray);
                        }
                        return ToolboxReturn.Good;
                    default:
                        Resize_(inpt, res, t, outp as SectArray);
                        return ToolboxReturn.Good;
                }
            }
        }

        private static void Resize_(Sect inpt, Size res, ResizeType t, SectArray outp)
        {
            int h = res.Height;
            int w = res.Width;

            int xUp, xDown, yUp, yDown;
            float xAdj, yAdj;

            Size sz = inpt.getPrefferedSize();
            int refH = sz.Height;
            int refW = sz.Width;

            switch (t)
            {
                case ResizeType.NearestNeighbor:
                    {
                        for (int y = 0; y < h; y++)
                        {
                            yAdj = y * (refH - 1);
                            yAdj /= (h - 1);
                            yUp = (int)Math.Round(yAdj, 0);

                            for (int x = 0; x < w; x++)
                            {
                                xAdj = x * (refW - 1);
                                xAdj /= (w - 1);
                                xUp = (int)Math.Round(xAdj, 0);
                                outp[y, x] = inpt[yUp, xUp];
                            }
                        }
                        break;
                    }
                case ResizeType.Bilinear:
                    {
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
                                    outp[y, x] = inpt[yUp, xUp];
                                }
                                else if (xUp == xDown)
                                {
                                    outp[y, x] = Helpers.getLinearEstimate(
                                        inpt[yDown, xUp],
                                        inpt[yUp, xUp],
                                        yAdj % 1);
                                }
                                else if (yUp == yDown)
                                {
                                    outp[y, x] = Helpers.getLinearEstimate(
                                        inpt[yUp, xDown],
                                        inpt[yUp, xUp],
                                        xAdj % 1);
                                }
                                else
                                {
                                    outp[y, x] = Helpers.getLinearEstimate(
                                        Helpers.getLinearEstimate(
                                            inpt[yDown, xDown],
                                            inpt[yUp, xDown],
                                            yAdj % 1),
                                        Helpers.getLinearEstimate(
                                            inpt[yDown, xUp],
                                            inpt[yUp, xUp],
                                            yAdj % 1),
                                        xAdj % 1);
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
