using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Resize : ToolDefault
    {
        private volatile float inud = 0.5f;
        private NumericUpDown nud;
        private volatile ImageAlgorithms.ResizeType t = ImageAlgorithms.ResizeType.NearestNeighbor;
        private ComboBox cb;

        public const string name = "Resize";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellResize);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Resize());
        }
        
        
        
        
        
      

        public Resize()
            : base (true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.AddRange(new object[] {
                ImageAlgorithms.ResizeType.NearestNeighbor,
                ImageAlgorithms.ResizeType.Bilinear
            });
            this.cb.SelectedItem = this.t;
            this.cb.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            l.Add(this.cb);

            this.nud = ControlDeque.NumericUpDown(2);
            this.nud.Minimum = 0.01m;
            this.nud.Maximum = 10.0m;
            this.nud.Value = new decimal(this.inud);
            this.nud.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            l.Add(this.nud);

            return l;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.inud = (float)this.nud.Value;
            this.StatusChanged();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.t = (ImageAlgorithms.ResizeType)this.cb.SelectedItem;
            this.StatusChanged();
        }

        public override ImageData SpecialBitmapUpdateDefault(ref ImageData d)
        {
            Size s = d.SizeOriginal;
            s.Width = (int)(s.Width * this.inud);
            s.Height = (int)(s.Height * this.inud);
            ImageData o = null;
            ImageAlgorithms.Resize_O(ref d, s, t, out o);
            return o;
        }
    }
}
