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

                var ls = new List<SectType>();
                if (inpt._Type == SectType.Holder) ls.AddRange((inpt as SectHolder).getSectTypes());
                else ls.Add(inpt._Type);

                SingleImage.MatchOutputToSizeAndSectTypes(ref outp, outp_sz, ls.ToArray());


                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    switch (pt)
                    {
                        case PaddingType.Zero:
                        case PaddingType.Unity:
                        case PaddingType.Extend:
                            {
                                for (int y = 0; y < inh; y++)
                                    for (int x = 0; x < inw; x++)
                                        anon_outp[y + pad, x + pad] = anon_inpt[y, x]; // CENTER
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

                                // 1 - Math.Abs() gives right angle we're looking for (input pixel to output pixel)
                                for (int y = 0; y < outp_sz.Height; y++)
                                {
                                    for (int x = 0; x < outp_sz.Width; x++)
                                    {
                                        anon_outp[y, x] = anon_inpt[
                                            inhm1 - Math.Abs(inhm1 - (((y - pad) % repy + repy) % repy)),
                                            inwm1 - Math.Abs(inwm1 - (((x - pad) % repx + repx) % repx))];
                                    }
                                }
                                break;
                            }
                    }
                };

                SingleImage.DoAction1v1(ref outp, act, inpt);

                switch (pt)
                {
                    case PaddingType.Zero:
                        PaddingUpdateUniform(pad, 0, outp);
                        break;
                    case PaddingType.Unity:
                        PaddingUpdateUniform(pad, 1, outp);
                        break;
                    case PaddingType.Extend:
                        PaddingUpdateExtend(pad, outp);
                        break;
                    case PaddingType.Repeat:
                    case PaddingType.Mirror:
                        break;
                    default: throw new NotImplementedException();
                }

                return ToolboxReturn.Good;
            }
        }

        public static void PaddingUpdateUniform(int pad, float value, Sect inpt)
        {
            var size_now = inpt.getPrefferedSize();
            var inh = size_now.Height - pad * 2;
            var inw = size_now.Width - pad * 2;

            Action<SectArray> act = (SectArray anon) =>
            {
                for (int y = 0; y < pad; y++)
                    for (int x = 0; x < size_now.Width; x++)
                        anon[y, x] = value; // TOP  

                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                    for (int x = 0; x < size_now.Width; x++)
                        anon[y, x] = value; // BOT

                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                    for (int x = 0; x < pad; x++)
                        anon[y, x] = value; // LEFT (No Corners)

                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                        anon[y, x] = value; // RIGHT (No Corners)
            };

            if (inpt._Type == SectType.Holder)
            {
                foreach (var sect in (inpt as SectHolder).Sects.Values)
                {
                    act(sect as SectArray);
                }
            }
            else
            {
                act(inpt as SectArray);
            }
        }

        public static void PaddingUpdateExtend(int pad, Sect inpt)
        {
            var size_now = inpt.getPrefferedSize();
            var inh = size_now.Height - pad * 2;
            var inw = size_now.Width - pad * 2;

            Action<SectArray> act = (SectArray anon) =>
            {
                for (int y = 0; y < pad; y++)
                    for (int x = pad, iw = 0; iw < inw; iw++, x++)
                        anon[y, x] = anon[pad, x]; // TOP (No Corners)

                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                    for (int x = pad, iw = 0; iw < inw; iw++, x++)
                        anon[y, x] = anon[inh + pad - 1, x]; // BOT (No Corners)

                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                    for (int x = 0; x < pad; x++)
                        anon[y, x] = anon[y, pad]; // LEFT (No Corners)

                for (int y = pad, ih = 0; ih < inh; ih++, y++)
                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                        anon[y, x] = anon[y, inw + pad - 1]; // RIGHT (No Corners)

                float val;

                val = anon[pad, pad];
                for (int y = 0; y < pad; y++)
                    for (int x = 0; x < pad; x++)
                        anon[y, x] = val; // TOP LEFT CORNER

                val = anon[pad, inw + pad - 1];
                for (int y = 0; y < pad; y++)
                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                        anon[y, x] = val; // TOP RIGHT CORNER

                val = anon[inh + pad - 1, pad];
                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                    for (int x = 0; x < pad; x++)
                        anon[y, x] = val; // BOT LEFT CORNER

                val = anon[inh + pad - 1, inw + pad - 1];
                for (int y = inh + pad, py = 0; py < pad; py++, y++)
                    for (int x = inw + pad, px = 0; px < pad; px++, x++)
                        anon[y, x] = val; // BOT RIGHT CORNER
            };

            if (inpt._Type == SectType.Holder)
            {
                foreach (var sect in (inpt as SectHolder).Sects.Values)
                {
                    act(sect as SectArray);
                }
            }
            else
            {
                act(inpt as SectArray);
            }

        }










        public static ToolboxReturn PaddingOff(Sect inpt, int pad, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var outp_sz = inpt.getPrefferedSize(); ;
                outp_sz.Width -= pad * 2;
                outp_sz.Height -= pad * 2;

                if (outp_sz.Width <= 0) return ToolboxReturn.SpecialError;
                if (outp_sz.Height <= 0) return ToolboxReturn.SpecialError;

                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    for (int y = 0; y < outp_sz.Height; y++)
                        for (int x = 0; x < outp_sz.Width; x++)
                            anon_outp[y, x] = anon_inpt[y + pad, x + pad]; // CENTER
                };


                var ls = new List<SectType>();
                if (inpt._Type == SectType.Holder) ls.AddRange((inpt as SectHolder).getSectTypes());
                else ls.Add(inpt._Type);

                SingleImage.MatchOutputToSizeAndSectTypes(ref outp, outp_sz, ls.ToArray());
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
