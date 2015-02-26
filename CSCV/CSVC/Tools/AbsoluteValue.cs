using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSeifert.ImageProcessing;
using System.Windows.Forms;

namespace ImageToolbox.Tools
{
    public class AbsoluteValue : ToolDefault
    {
        public AbsoluteValue()
            : base(true, true)
        {
            this.checkBoxName.Text = "Absolute Value";
        }

        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            ImageData o = null;
            ImageAlgorithms.Abs_O(ref d, out o);
            return o;
        }
    }
}
