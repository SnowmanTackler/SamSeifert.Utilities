using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSeifert.CSCV;
using System.Windows.Forms;

namespace CSCV_IDE.Tools
{
    public class AbsoluteValue : ToolDefault
    {
        public AbsoluteValue()
            : base(true, true)
        {
            this.checkBoxName.Text = "Absolute Value";
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.Abs(d, ref o);
            return o;
        }
    }
}
