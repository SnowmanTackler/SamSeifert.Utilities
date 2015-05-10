using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.CSCV;

namespace CSCV_IDE.Tools
{
    public partial class BlackWhite : ToolDefault
    {
        private volatile float inud = 0.5f;
        private NumericUpDown nud;
        private static string name = "Black and White";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(BlackWhite.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellTwoTone);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new BlackWhite());
        }

        public BlackWhite()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        protected override List<Control> getStartControls()
        {
            var l = base.getStartControls();

            this.nud = ControlDeque.NumericUpDown(2);
            this.nud.Minimum = 0;
            this.nud.Maximum = 1;
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
            IA_Single.TwoTone(d, this.inud, ref o);
            return o;
        }
    }
}
