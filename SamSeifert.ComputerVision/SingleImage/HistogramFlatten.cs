﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ComputerVision
{
    public static partial class SingleImage
    {
        public static ToolboxReturn HistogramFlatten(Sect inpt, ref Sect outp)
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                Action<Sect, SectArray> act = (Sect anon_inpt, SectArray anon_outp) =>
                {
                    var sz = anon_outp.getPrefferedSize();
                    int w = sz.Width;
                    int h = sz.Height;

                    int[] counts = new int[511];
                    Byte[] _Bytes = new Byte[511];

                    for (int i = 0; i < counts.Length; i++) counts[i] = 0;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            counts[255 + Helpers.Cast(anon_inpt[y, x] * 255)]++;
                        }
                    }

                    int sum = 0, sumLast = 0, temp;
                    for (int i = 0; i < counts.Length; i++)
                    {
                        temp = counts[i];
                        if (temp != 0)
                        {
                            sumLast = sum;
                            sum += temp;
                        }
                    }

                    sum = 0;
                    sumLast = Math.Max(1, sumLast);
                    for (int i = 0; i < counts.Length; i++)
                    {
                        _Bytes[i] = (Byte)((255 * sum) / (sumLast));
                        sum += counts[i];
                    }

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            anon_outp[y, x] = _Bytes[255 + Helpers.Cast(anon_inpt[y, x] * 255)] / 255.0f;
                        }
                    }
                };

                SingleImage.MatchOutputToInput(inpt, ref outp);
                SingleImage.DoAction1v1(ref outp, act, inpt);

                return ToolboxReturn.Good;
            }
        }
    }
}
