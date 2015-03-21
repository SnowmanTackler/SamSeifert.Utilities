using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public class MeanShift : ToolDefault
    {
        private static string name = "Mean Shift";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(MeanShift.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellHistogram);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new MeanShift());
        }

        public MeanShift()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.MeanShiftRGB(d, 0.25f, ref o);
            return o;
        }

    }
}
