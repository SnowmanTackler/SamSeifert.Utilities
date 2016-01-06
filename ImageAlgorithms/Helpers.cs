using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class ImageAlgorithms
    {
        private static void match(Sect inpt, ref Sect outp)
        {
            Boolean remake = outp == null;
            if (!remake) remake = (inpt._Type != outp._Type) || (inpt.getPrefferedSize() != outp.getPrefferedSize());
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
                Size sz = inpt.getPrefferedSize();
                switch (inpt._Type)
                {
                    case SectType.Holder:
                        outp = new SectHolder((inpt as SectHolder).getSectTypes(), sz);
                        break;
                    default:
                        outp = new SectArray(inpt._Type, sz.Width, sz.Height);
                        break;
                }
            }
            else outp.reset();
        }

        private static void match(ref Sect outp, Size sz, params SectType[] sts)
        {
            Boolean remake = outp == null;
            if (!remake) remake = sz != outp.getPrefferedSize();
            switch (sts.Length)
            {
                case 1:
                    {
                        if (!remake) remake = sts.First() != outp._Type;
                        if (remake) outp = new SectArray(SectType.Gray, sz.Width, sz.Height);
                        else outp.reset();
                    }
                    break;
                default:
                    {
                        if (!remake) remake = SectType.Holder != outp._Type;
                        if (!remake)
                        {
                            SectHolder sh = outp as SectHolder;
                            if (sh.Sects.Count == sts.Length)
                            {
                                foreach (var st in sts)
                                {
                                    if (sh.Sects.ContainsKey(st))
                                    {
                                        var sa = sh.Sects[st];
                                        if (!(sa is SectArray))
                                        {
                                            remake = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        remake = true;
                                        break;
                                    }
                                }
                            }
                            else remake = true;
                        }
                        if (remake) outp = new SectHolder(sts, sz);
                        else outp.reset();
                    }
                    break;
            }
        }
    }
}
