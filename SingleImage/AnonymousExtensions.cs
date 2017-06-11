using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
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
            Func<Single, Single> func = (float f) => { return Helpers.Clamp(f, 0, 1); };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Power(Sect inpt, Single val, ref Sect outp)
        {
            Func<Single, Single> func = (float f) => { return (Single)Math.Pow(f, val); };
            return Anonymous(inpt, ref outp, func);
        }

        public static ToolboxReturn Threshold(Sect inpt, ref Sect outp, Single thresh)
        {
            Func<Single, Single> func = (float f) => { return f < thresh ? 0 : 1; };
            return Anonymous(inpt, ref outp, func);
        }
    }
}
