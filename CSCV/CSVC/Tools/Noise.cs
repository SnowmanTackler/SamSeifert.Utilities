using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class Noise : ToolDefault
    {
        private volatile float inud = 0.5f;
        private NumericUpDown nud;
        private volatile NoiseType t = NoiseType.Gaussian;
        private ComboBox cb;

        private static string name = "Noise";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(Noise.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellGaussian);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new Noise());
        }






        public Noise()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.cb = ControlDeque.ComboBox();
            this.cb.Items.AddRange(new object[] {
                NoiseType.Gaussian,
                NoiseType.Uniform,
                NoiseType.SaltAndPepper
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
            this.t = (NoiseType)this.cb.SelectedItem;
            this.StatusChanged();
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.Noise(d, this.t, inud, ref o);
            return o;
        }
    }
}