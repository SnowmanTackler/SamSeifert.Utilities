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
    public partial class Grayscale : ToolDefault
    {
        private volatile ImageAlgorithms.GrayScaleType t = ImageAlgorithms.GrayScaleType.Mean;
        private ComboBox cb;
        private static string name = "Grayscale";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(Grayscale.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellGray);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Grayscale());
        }

        public Grayscale()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.AddRange(new object[] {
                ImageAlgorithms.GrayScaleType.Mean,
                ImageAlgorithms.GrayScaleType.Maximum,
                ImageAlgorithms.GrayScaleType.Minimum
            });
            this.cb.SelectedItem = this.t;
            this.cb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            l.Add(this.cb);

            return l;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.t = (ImageAlgorithms.GrayScaleType)this.cb.SelectedItem;
            this.StatusChanged();
        }

        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            ImageData o = null;
            ImageAlgorithms.GrayScale_O(ref d, this.t, out o);
            return o;
        }

    }
}
