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
    public partial class ErodeDilateFull : ToolDetails
    {
        private List<List<CheckBox>> _List = new List<List<CheckBox>>();

        public ErodeDilateFull(ErodeDilate parent, Boolean[][] grid, int cycles, int type)
        {
            InitializeComponent();

            this.setGrid(grid, cycles);
            this.setFunctionType(type);

            this.DontUpdate = false;
            this.tryToUpdate();

//            this.ShowDialog(parent.nhiTD.getSpecialBitmap());

            parent.update(this.getGrid(), (int)(this.numericUpDownCycles.Value), this.getFunctionType());
        }

        public void setFunctionType(int type)
        {
            if (type == 0) this.radioButtonE.Checked = true;
            else if (type == 1) this.radioButtonD.Checked = true;
            else if (type == 2) this.radioButtonO.Checked = true;
            else this.radioButtonC.Checked = true;
        }

        public int getFunctionType()
        {
            if (this.radioButtonE.Checked) return 0;
            else if (this.radioButtonD.Checked) return 1;
            else if (this.radioButtonO.Checked) return 2;
            else return 3;
        }

        public void setGrid(Boolean[][] grid, int cycles)
        {
            int c = grid.Length;
            int r = grid[0].Length;
            this.numericUpDownC.Value = c;
            this.numericUpDownR.Value = r;

            this.updateTableSize();

            for (int j = 0; j < c; j++)
                for (int k = 0; k < r; k++)
                {
                    this._List[j][k].Checked = grid[j][k];
                }

            this.numericUpDownCycles.Value = cycles;
        }

        public Boolean[][] getGrid()
        {
            int rows = (int)(this.numericUpDownR.Value);
            int cols = (int)(this.numericUpDownC.Value);

            Boolean[][] grid = new Boolean[cols][];

            for (int c = 0; c < cols; c++)
            {
                grid[c] = new Boolean[rows];
                for (int r = 0; r < rows; r++)
                    grid[c][r] = this._List[c][r].Checked;
            }

            return grid;
        }

        public override Sect updateOverride(Sect indata)
        {
            if (indata == null) return null;

            int index = this.getFunctionType();
            int cycles = (int)(this.numericUpDownCycles.Value);

            return null;

/*            if (index == 0) return ImageToolbox.erode(indata, this.getGrid(), cycles);
            else if (index == 1) return ImageToolbox.dilate(indata, this.getGrid(), cycles);
            else if (index == 2) return ImageToolbox.open(indata, this.getGrid(), cycles);
            else return ImageToolbox.close(indata, this.getGrid(), cycles);*/
        }

        private void numericUpDownR_ValueChanged(object sender, EventArgs e)
        {
            this.updateTableSize();
        }

        private void numericUpDownC_ValueChanged(object sender, EventArgs e)
        {
            this.updateTableSize();
        }

        public void updateTableSize()
        {
            int rows = (int)(this.numericUpDownR.Value);
            int cols = (int)(this.numericUpDownC.Value);

            while (this._List.Count < cols) this._List.Add(new List<CheckBox>());
            while (this._List.Count > cols)
            {
                var nudL = this._List[this._List.Count - 1];

                foreach (var nud in nudL)
                {
                    nud.CheckedChanged -= new EventHandler(this.nudChanged);
                    nud.Dispose();
                }

                nudL.Clear();

                this._List.Remove(nudL);
            }

            foreach (var nudL in this._List)
            {
                while (nudL.Count < rows)
                {
                    var nud = new CheckBox();
                    nud.Text = "";
                    nud.Width = 14;
                    nud.Height = 14;
                    nud.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                    nud.Checked = true;

                    int b = 4;
                    int c = this._List.IndexOf(nudL);
                    int r = nudL.Count;
                    nud.Name = "r" + r + "c" + c;
                    nud.Top = 330 + (nud.Height + b) * r;
                    nud.Left = b + (b + nud.Width) * c;
                    nud.CheckedChanged += new EventHandler(this.nudChanged);
                    this.Controls.Add(nud);
                    nudL.Add(nud);
                }
                while (nudL.Count > rows)
                {
                    var nud = nudL[nudL.Count - 1];
                    nud.CheckedChanged -= new EventHandler(this.nudChanged);
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

        private bool DontUpdate = true;
        private void tryToUpdate()
        {
            if (this.DontUpdate) return;
            this.DontUpdate = true;
            this.updateImage();
            this.DontUpdate = false;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;

            if (r != null)
                if (r.Checked)
                    this.updateImage();
        }

        private void numericUpDownCycles_ValueChanged(object sender, EventArgs e)
        {
            this.tryToUpdate();
        }
    }
}
