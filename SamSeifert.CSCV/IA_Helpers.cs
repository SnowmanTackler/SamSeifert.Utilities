using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.CSCV
{
    public class IA_Helpers
    {
        public static float Clamp(float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static int Cast(float f)
        {
            return Math.Max(-255, Math.Min(255, (int)Math.Round(f, 0)));
        }

        public static Byte castByte(float f)
        {
            return (Byte)Math.Max(0, Math.Min(255, f));
        }


    }
}
