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
    public class BitsPerPixel : ToolDefault
    {
        private volatile int inud = 1;
        private NumericUpDown nud;
        private static string name = "Bits Per Pixel";

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
                FormMain.addCell(new BitsPerPixel()); 
        }









        public BitsPerPixel()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.nud = ControlDeque.NumericUpDown(0);
            this.nud.Minimum = 1;
            this.nud.Maximum = 8;
            this.nud.Value = this.inud;
            this.nud.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            l.Add(this.nud);

            return l;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.inud = (int)this.nud.Value;
            this.StatusChanged();
        }

        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            ImageData o = null;
            ImageAlgorithms.BitPerPixel_O(ref d, this.inud, out o);
            return o;
        }
    }
}
