using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ComputerVision
{
    public static partial class SingleImage
    {
        public static ToolboxReturn Abs(Sect inpt, Single min, Single max, ref Sect outp)
        {
            Func<Single, Single> func = (float f) => { return Math.Abs(f); };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Clamp(Sect inpt, Single min, Single max, ref Sect outp)
        {
            Func<Single, Single> func = (float f) => { return Helpers.Clamp(f, min, max); };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Clamp(Sect inpt, ref Sect outp)
        {
            return Clamp(inpt, 0, 1, ref outp);
        }

        public static ToolboxReturn Power(Sect inpt, Single val, ref Sect outp)
        {
            Func<Single, Single> func = (float f) => { return (Single)Math.Pow(f, val); };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Threshold(Sect inpt, Single thresh, ref Sect outp)
        {
            Func<Single, Single> func = (float f) => { return f < thresh ? 0 : 1; };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Map(Sect inpt, float from_min, float from_max, float to_min, float to_max, ref Sect outp)
        {
            float rangeTo = to_max - to_min;
            float rangeFrom = from_max - from_min;
            if (rangeTo * rangeFrom != 0)
            {
                float gain = rangeTo / rangeFrom;
                float offset = to_min - from_min * gain;
                return SingleImage.Anonymous(inpt, ref outp, (f) => f * gain + offset);
            }
            else return ToolboxReturn.SpecialError;
        }
    }
}
