using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.CSCV
{
    public class SectGaussian : SectArray
    {
        public SectGaussian(SectType t, Single sigma, int span)
            : base(t, span, span)
        {
            Single sum = 0;
            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    int x = j - span / 2;
                    int y = i - span / 2;

                    Single val = (Single) Math.Pow(Math.E, -(x * x + y * y) / (2 * sigma));
                    sum += val;
                    this.Data[i, j] = val;
                }
            }
            for (int i = 0; i < span; i++)
            {
                for (int j = 0; j < span; j++)
                {
                    this.Data[i, j] /= sum;
                }
            }
        }
    }
}
