using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public class Gradient : ToolDefault
    {
        private static string name = "Gradient";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(Gradient.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellGradient);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Gradient());
        }

        public Gradient()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            ImageData o = null;
            ImageAlgorithms.Gradient_O(ref d, out o);
            return o;
        }
    }
}
