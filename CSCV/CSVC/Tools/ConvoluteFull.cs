using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class ConvoluteFull : ToolDetails
    {
        private List<List<NumericUpDown>> _List = new List<List<NumericUpDown>>();

        private Convolute _ToolConvolute;

        public ConvoluteFull(Convolute mama, Decimal[,] filt)
        {
            InitializeComponent();

            int c = filt.GetLength(0);
            int r = filt.GetLength(1);

            this.numericUpDownC.Value = c;
            this.numericUpDownR.Value = r;

            this.updateTableSize();

            for (int j = 0; j < c; j++)
                for (int k = 0; k < r; k++)
                {
                    this._List[j][k].Value = filt[j, k];
                }

            this.DontUpdate = false;
            this.tryToUpdate();

            this._ToolConvolute = mama;
        }

        public override Sect updateOverride(Sect indata)
        {
            if (indata == null) return null;
            Sect o = null;
            IA_Single.Convolute(indata, this.getFiltF(), ref o);
            return o;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.updateTableSize();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.updateTableSize();
        }

        public void updateTableSize()
        {
            int rows = (int)(this.numericUpDownR.Value);
            int cols = (int)(this.numericUpDownC.Value);

            while (this._List.Count < cols) this._List.Add(new List<NumericUpDown>());
            while (this._List.Count > cols)
            {
                var nudL = this._List[this._List.Count - 1];

                foreach (var nud in nudL)
                {
                    nud.ValueChanged -= new EventHandler(this.nudChanged);
                    nud.Dispose();
                }

                nudL.Clear();

                this._List.Remove(nudL);
            }

            foreach (var nudL in this._List)
            {
                while (nudL.Count < rows)
                {
                    var nud = this.CloneNud();
                    nud.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

                    int b = 8;
                    int c = this._List.IndexOf(nudL);
                    int r = nudL.Count;
                    nud.Name = "r" + r + "c" + c;
                    nud.Top += (nud.Height + b) * (r + 1);
                    nud.Left = b + (b + nud.Width) * c;
                    nud.ValueChanged += new EventHandler(this.nudChanged);
                    this.Controls.Add(nud);
                    nudL.Add(nud);
                }
                while (nudL.Count > rows)
                {
                    var nud = nudL[nudL.Count - 1];
                    nud.ValueChanged -= new EventHandler(this.nudChanged);
                    nud.Dispose();
                    nudL.Remove(nud);
                }
            }

            this.Refresh();
            this.tryToUpdate();
        }

        private void nudChanged(object sender, EventArgs e)
        {
            this.tryToUpdate();
        }

        private Decimal[,] getFilt()
        {
            int rows = (int)(this.numericUpDownR.Value);
            int cols = (int)(this.numericUpDownC.Value);

            Decimal[,] filt = new Decimal[cols, rows];

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++) 
                    filt[c,r] = this._List[c][r].Value;

            return filt;
        }

        private Single[,] getFiltF()
        {
            int rows = (int)(this.numericUpDownR.Value);
            int cols = (int)(this.numericUpDownC.Value);

            Single[,] filt = new Single[cols, rows];

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++) 
                    filt[c, r] = (Single)this._List[c][r].Value;

            return filt;
        }

        private bool DontUpdate = true;
        private void tryToUpdate()
        {
            if (this.DontUpdate) return;
            this.DontUpdate = true;
            this.updateImage();
            this.DontUpdate = false;
        }


        private NumericUpDown CloneNud()
        {
            NumericUpDown nud = new NumericUpDown();

            nud.Location = this.numericUpDownD.Location;
            nud.BackColor = this.numericUpDownD.BackColor;
            nud.ForeColor = this.numericUpDownD.ForeColor;
            nud.Font = this.numericUpDownD.Font;
            nud.Size = this.numericUpDownD.Size;
            nud.Maximum = this.numericUpDownD.Maximum;
            nud.Minimum = this.numericUpDownD.Minimum;
            nud.Value = this.numericUpDownD.Value;
            nud.DecimalPlaces = this.numericUpDownD.DecimalPlaces;
            nud.Increment = this.numericUpDownD.Increment;
            nud.TextAlign = this.numericUpDownD.TextAlign;

            return nud;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.tryToUpdate();
        }

        protected override void _Form_FormClosing(object sender, CancelEventArgs e)
        {
            this._ToolConvolute.setFilter(this.getFilt());
            base._Form_FormClosing(sender, e);
        }
    }
}
