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
    public partial class Gain : ToolDefault
    {
        private volatile float inud = 0.5f;
        private NumericUpDown nud;
        private static string name = "Gain";

        public Gain()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.nud = ControlDeque.NumericUpDown(2);
            this.nud.Minimum = -100;
            this.nud.Maximum = 100;
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

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.Multiply(d, this.inud, ref o);
            return o;
        }
    }
}
