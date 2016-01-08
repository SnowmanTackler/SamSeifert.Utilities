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
        private static ToolboxReturn MatchSectTypes(Sect[] inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            if (inpt.Length == 0)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var holders = new List<SectHolder>();
                var nonholders = new List<Sect>();


                Size sz_out = Size.Empty;
                Boolean first_size = true;
                Boolean first_single = true;
                SectType single_type = SectType.NaN;

                foreach (var inp in inpt)
                {
                    if (inp._Type == SectType.Holder) holders.Add(inp as SectHolder);
                    else
                    {
                        if (first_single)
                        {
                            single_type = inp._Type;
                            first_single = false;
                        }
                        else if (inp._Type != single_type) single_type = SectType.NaN;
                        nonholders.Add(inp);
                    }

                    // Make Sure Inputs All Same Size
                    Size tsz = inp.getPrefferedSize();
                    if (!inp.isSquishy())
                    {
                        if (first_size)
                        {
                            first_size = false;
                            sz_out = inp.getPrefferedSize();
                        }
                        else if (sz_out != tsz)
                        {
                            outp = null;
                            return ToolboxReturn.ImageSizeMismatch;
                        }
                    }
                    else if (first_size)
                    {
                        sz_out.Height = Math.Max(sz_out.Height, tsz.Height);
                        sz_out.Width = Math.Max(sz_out.Width, tsz.Width);
                    }
                }

                SectHolder first_sh = null;

                foreach (var sh in holders)
                {
                    if (first_sh == null) first_sh = sh as SectHolder;
                    else
                    {
                        var k1 = first_sh.Sects.Keys;
                        var k2 = sh.Sects.Keys;
                        if ((k1.Count != k2.Count) || (k1.Except(k2).Any()))
                        {
                            outp = null;
                            return ToolboxReturn.SpecialError;
                        }
                    }
                }

                if (holders.Count == 0) // Just Non Holders
                {
                    if (single_type == SectType.NaN) single_type = SectType.Gray;
                    Boolean remake = outp == null;
                    if (!remake) remake = !(outp is SectArray);
                    if (!remake) remake = (single_type != outp._Type) || (sz_out != outp.getPrefferedSize());
                    if (remake) outp = new SectArray(single_type, sz_out.Width, sz_out.Height);
                    else outp.reset();
                }
                else
                {
                    Boolean remake = outp == null;
                    if (!remake) remake = (SectType.Holder != outp._Type) || (sz_out != outp.getPrefferedSize());
                    if (!remake)
                    {
                        var k1 = first_sh.Sects.Keys;
                        var k2 = (outp as SectHolder).Sects.Keys;
                        remake = ((k1.Count != k2.Count) || (k1.Except(k2).Any()));
                    }

                    if (remake) outp = new SectHolder(sz_out, first_sh.getSectTypes());
                    else outp.reset();
                }

                return ToolboxReturn.Good;
            }
        }
    }
}
