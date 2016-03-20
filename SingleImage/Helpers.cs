using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public static partial class SingleImage
    {
        private static void MatchOutputToInput(Sect inpt, ref Sect outp)
        {
            MatchOutputToInput(inpt, ref outp, inpt.getPrefferedSize());
        }

        private static void MatchOutputToInput(Sect inpt, ref Sect outp, Size sz_out)
        {
            Boolean remake = outp == null;
            if (!remake) remake = (inpt._Type != outp._Type) || (sz_out != outp.getPrefferedSize());
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
                        outp = new SectHolder(sz_out, (inpt as SectHolder).getSectTypes());
                        break;
                    default:
                        outp = new SectArray(inpt._Type, sz_out.Width, sz_out.Height);
                        break;
                }
            }
            else outp.Reset();
        }

        private static void MatchOutputToSizeAndSectTypes(ref Sect outp, Size sz, params SectType[] sts)
        {
            Boolean remake = outp == null;
            if (!remake) remake = sz != outp.getPrefferedSize();
            switch (sts.Length)
            {
                case 1:
                    {
                        if (!remake) remake = sts.First() != outp._Type;
                        if (remake) outp = new SectArray(SectType.Gray, sz.Width, sz.Height);
                        else outp.Reset();
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
                        if (remake) outp = new SectHolder(sz, sts);
                        else outp.Reset();
                    }
                    break;
            }
        }

        private static void DoAction1v1(ref Sect outp, Action<Sect, SectArray> act, Sect inpt)
        {
            if (inpt._Type == SectType.Holder)
            {
                var sh_inpt = inpt as SectHolder;
                var sh_outp = outp as SectHolder;

                foreach (var kvp in sh_inpt.Sects)
                {
                    var sout = sh_outp.getSect(kvp.Key);
                    act(kvp.Value, sout as SectArray);
                }
            }
            else
            {
                act(inpt, outp as SectArray);
            }
        }

        private static void DoAction1vN(ref Sect outp, Action<Sect[], SectArray> act, params Sect[] inpt)
        {
            if (outp._Type == SectType.Holder)
            {
                var sh_outp = outp as SectHolder;

                foreach (var kvp in sh_outp.Sects)
                {
                    var sin = new Sect[inpt.Length];

                    for (int i = 0; i < inpt.Length; i++)
                        sin[i] = (inpt[i] as SectHolder).getSect(kvp.Key);

                    act(sin, kvp.Value as SectArray);
                }
            }
            else
            {
                act(inpt, outp as SectArray);
            }
        }

    }
}
