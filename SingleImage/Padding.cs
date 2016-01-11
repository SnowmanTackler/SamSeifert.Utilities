using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.CSCV
{
    public enum PaddingType
    {
        [Description("Black")]
        Black,

        [Description("White")]
        White,

        [Description("Extend")]
        Extend,

        [Description("Mirror")]
        Mirror,
    };

    public static partial class SingleImage
    {
        public ToolboxReturn Padding(Sect inpt, PaddingType p, int pad, ref Sect outp)
        {

        }
    }
}
