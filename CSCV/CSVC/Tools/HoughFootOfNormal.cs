using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    class HoughFootOfNormal : ToolDefault
    {
        private static string name = "Hough - Foot Of Normal";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellBPP);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new HoughFootOfNormal()); 
        }









        public HoughFootOfNormal()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
//            HoughTransform.FootOfNormal_O(ref d, out o);
            return o;
        }    
    }
}
