using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class IA_Multiple
    {
        public static ToolboxReturn match(Sect[] inpt, ref Sect outp)
        {
            if (inpt.Length == 0)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                SectType st = SectType.NaN;
                Size sz = Size.Empty;
                Boolean first = true;
                Boolean first_size = true;
                foreach (var inp in inpt)
                {
                    // Make Sure matching sect type
                    if (first) st = inp._Type;
                    else if (st != inp._Type)
                    {
                        outp = null;
                        return ToolboxReturn.SpecialError;
                    }
                    first = false;

                    // Make Sure matching size
                    Size tsz = inp.getPrefferedSize();
                    if (!inp.isSquishy())
                    {
                        if (first_size)
                        {
                            first_size = false;
                            sz = inp.getPrefferedSize();
                        }
                        else if (sz != tsz)
                        {
                            outp = null;
                            return ToolboxReturn.ImageSizeMismatch;
                        }
                    }
                    else if (first_size)
                    {
                        sz.Height = Math.Max(sz.Height, tsz.Height);
                        sz.Width = Math.Max(sz.Width, tsz.Width);
                    }
                }

                Boolean remake = outp == null;
                if (!remake) remake = (st != outp._Type) || (sz != outp.getPrefferedSize());

                if (st == SectType.Holder)
                {
                    SectHolder sh = null;

                    first = true;
                    foreach (Sect s in inpt)
                    {
                        if (first) sh = s as SectHolder;
                        else
                        {
                            var k1 = sh.Sects.Keys;
                            var k2 = (s as SectHolder).Sects.Keys;
                            if ((k1.Count != k2.Count) || (k1.Except(k2).Any())) return ToolboxReturn.SpecialError;
                        }
                        first = false;
                    }

                    if (!remake)
                    {
                        var k11 = sh.Sects.Keys;
                        var k22 = (outp as SectHolder).Sects.Keys;
                        remake = ((k11.Count != k22.Count) || (k11.Except(k22).Any()));
                        foreach (var val in (outp as SectHolder).Sects.Values)
                        {
                            if (remake) break;
                            else remake = !(val is SectArray);
                        }
                    }

                    if (remake) outp = new SectHolder(sh.getSectTypes(), sz);
                    return ToolboxReturn.Good;
                }
                else
                {
                    if (!remake) remake = !(outp is SectArray);
                    if (remake) outp = new SectArray(st, sz.Width, sz.Height);
                    else outp.reset();
                    return ToolboxReturn.Good;
                }
            }
        }











        public static ToolboxReturn Add(Sect[] inpt, Boolean[] signs, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inpt.Length != signs.Length)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {             
                var ret = match(inpt, ref outp);
                if (ret != ToolboxReturn.Good) return ret;

                if (outp._Type == SectType.Holder)
                {
                    SectHolder sh = null;

                    foreach (var st2 in sh.getSectTypes())
                    {
                        var ls = new List<Sect>();
                        foreach (var inp in inpt) ls.Add((inp as SectHolder).getSect(st2));
                        Add_(ls.ToArray(), signs, (outp as SectHolder).getSect(st2) as SectArray);
                    }

                    return ToolboxReturn.Good;
                }
                else
                {
                    Add_(inpt, signs, outp as SectArray);
                    return ToolboxReturn.Good;
                }
            }
        }

        private static void Add_(Sect[] inpt, Boolean[] signs, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            for (int i = 0; i < inpt.Length; i++)
            {
                var inp = inpt[i];

                if (i == 0)
                {
                    if (signs[i])
                    {
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                outp[y, x] = inp[y, x];
                    }
                    else
                    {
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                outp[y, x] =- inp[y, x];
                    }
                }
                else
                {
                    if (signs[i])
                    {
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                outp[y, x] += inp[y, x];
                    }
                    else
                    {
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                                outp[y, x] -= inp[y, x];
                    }
                }
            }
        }












        public static ToolboxReturn Mult(Sect[] inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var ret = match(inpt, ref outp);
                if (ret != ToolboxReturn.Good) return ret;

                if (outp._Type == SectType.Holder)
                {
                    SectHolder sh = null;

                    foreach (var st2 in sh.getSectTypes())
                    {
                        var ls = new List<Sect>();
                        foreach (var inp in inpt) ls.Add((inp as SectHolder).getSect(st2));
                        Mult_(ls.ToArray(), (outp as SectHolder).getSect(st2) as SectArray);
                    }

                    return ToolboxReturn.Good;
                }
                else
                {
                    Mult_(inpt, outp as SectArray);
                    return ToolboxReturn.Good;
                }
            }
        }

        private static void Mult_(Sect[] inpt, SectArray outp)
        {
            var sz = outp.getPrefferedSize();
            int w = sz.Width;
            int h = sz.Height;

            for (int i = 0; i < inpt.Length; i++)
            {
                var inp = inpt[i];

                if (i == 0)
                {
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            outp[y, x] = inp[y, x];
                }
                else
                {
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            outp[y, x] *= inp[y, x];
                }
            }
        }
    }
}
