using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public enum PaddingType
    {
        [Description("Zero")]
        Zero,

        [Description("Unity")]
        Unity,

        [Description("Extend")]
        Extend,

        [Description("Mirror")]
        Mirror,

        [Description("Repeat")]
        Repeat,
    };

    public static partial class SingleImage
    {
        public static ToolboxReturn PaddingAdd(Sect inpt, PaddingType pt, int pad, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var inpt_sz = inpt.getPrefferedSize();
                var inh = inpt_sz.Height;
                var inw = inpt_sz.Width;

                var outp_sz = inpt_sz;
                outp_sz.Width += pad * 2;
                outp_sz.Height += pad * 2;

                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    switch (pt)
                    {
                        case PaddingType.Zero:
                            {
                                for (int y = 0; y < inh; y++)
                                    for (int x = 0; x < inw; x++)
                                        anon_outp[y + pad, x + pad] = anon_inpt[y, x]; // CENTER

                                for (int y = 0; y < pad; y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                        anon_outp[y, x] = 0; // TOP  

                                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                        anon_outp[y, x] = 0; // BOT

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = 0; x < pad; x++)
                                        anon_outp[y, x] = 0; // LEFT (No Corners)

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                                        anon_outp[y, x] = 0; // RIGHT (No Corners)
                                break;
                            }
                        case PaddingType.Unity:
                            {
                                for (int y = 0; y < inh; y++)
                                    for (int x = 0; x < inw; x++)
                                        anon_outp[y + pad, x + pad] = anon_inpt[y, x]; // CENTER

                                for (int y = 0; y < pad; y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                        anon_outp[y, x] = 1; // TOP  

                                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                        anon_outp[y, x] = 1; // BOT

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = 0; x < pad; x++)
                                        anon_outp[y, x] = 1; // LEFT (No Corners)

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                                        anon_outp[y, x] = 1; // RIGHT (No Corners)
                                break;
                            }
                        case PaddingType.Extend:
                            {
                                for (int y = 0; y < inh; y++)
                                    for (int x = 0; x < inw; x++)
                                        anon_outp[y + pad, x + pad] = anon_inpt[y, x]; // CENTER

                                for (int y = 0; y < pad; y++)
                                    for (int x = pad, iw = 0; iw < inw; iw++, x++)
                                        anon_outp[y, x] = anon_outp[pad, x]; // TOP (No Corners)

                                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                                    for (int x = pad, iw = 0; iw < inw; iw++, x++)
                                        anon_outp[y, x] = anon_outp[inh + pad - 1, x]; // BOT (No Corners)

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = 0; x < pad; x++)
                                        anon_outp[y, x] = anon_outp[y, pad]; // LEFT (No Corners)

                                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                                        anon_outp[y, x] = anon_outp[y, inw + pad - 1]; // RIGHT (No Corners)

                                float val;

                                val = anon_outp[pad, pad];
                                for (int y = 0; y < pad; y++)
                                    for (int x = 0; x < pad; x++)
                                        anon_outp[y, x] = val; // TOP LEFT CORNER

                                val = anon_outp[pad, inw + pad - 1];
                                for (int y = 0; y < pad; y++)
                                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                                        anon_outp[y, x] = val; // TOP RIGHT CORNER

                                val = anon_outp[inh + pad - 1, pad];
                                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                                    for (int x = 0; x < pad; x++)
                                        anon_outp[y, x] = val; // BOT LEFT CORNER

                                val = anon_outp[inh + pad - 1, inw + pad - 1];
                                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                                        anon_outp[y, x] = val; // BOT RIGHT CORNER

                                break;
                            }
                        case PaddingType.Repeat:
                            {
                                for (int y = 0; y < outp_sz.Height; y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                        anon_outp[y, x] = anon_inpt[
                                            ((y - pad) % inh + inh) % inh, // DOUBLE MODULO SOLVES NEGATIVES
                                            ((x - pad) % inw + inw) % inw // DOUBLE MODULO SOLVES NEGATIVES
                                            ];

                                break;
                            }
                        case PaddingType.Mirror:
                            {
                                int repx = inw * 2 - 2; // How Often X Repeats
                                int repy = inh * 2 - 2; // How Often Y Repeats

                                int inhm1 = inh - 1;
                                int inwm1 = inw - 1;

                                // 1 - Math.Abs() gives right angle we're looking for

                                for (int y = 0; y < outp_sz.Height; y++)
                                    for (int x = 0; x < outp_sz.Width; x++)
                                    {
                                        anon_outp[y, x] = anon_inpt[
                                            inhm1 - Math.Abs(inhm1 - (((y - pad) % repy + repy) % repy)),
                                            inwm1 - Math.Abs(inwm1 - (((x - pad) % repx + repx) % repx))
                                            ];
                                    }
                                break;

                            }
                        default: throw new NotImplementedException();
                    }

                };


                var ls = new List<SectType>();
                if (inpt._Type == SectType.Holder) ls.AddRange((inpt as SectHolder).getSectTypes());
                else ls.Add(inpt._Type);

                SingleImage.MatchOutputToSizeAndSectTypes(ref outp, outp_sz, ls.ToArray());
                SingleImage.Do1v1Action(inpt, ref outp, act);

                return ToolboxReturn.Good;
            }
        }
    }
}
