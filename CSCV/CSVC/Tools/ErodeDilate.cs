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
    public partial class ErodeDilate : ToolDefault
    {
        public static string name = "Erode + Dilate";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellErodeDialate);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new ErodeDilate());
        }






        private Boolean[][] grid = new Boolean[][]
        {
            new Boolean[]{false, true, false},
            new Boolean[]{true,  true, true },
            new Boolean[]{false, true, false}
        };

        private int cycles = 1;
        private int functionType = 2;

        public ErodeDilate()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
            this.ToolStrip_Edit.Enabled = true;
        }

        public override void MenuEdit()
        {
            new ErodeDilateFull(this, this.grid, this.cycles, this.functionType);
        }

        public void update(Boolean[][] grid, int cycles, int functionType)
        {
            this.cycles = cycles;
            this.functionType = functionType;
            this.grid = grid;
            this.StatusChanged();
        }

/*        public override void SpecialBitmapUpdate()
        {
            this._SpecialBitmap = this.nhiTD.getSpecialBitmap();

            if (this._SpecialBitmap == null) return;

            if (this.checkBoxName.Checked)
            {
                if (this.functionType == 0)
                    this._SpecialBitmap = ImageToolbox.erode(this._SpecialBitmap, this.grid, this.cycles);
                else if (this.functionType == 1)
                    this._SpecialBitmap = ImageToolbox.dilate(this._SpecialBitmap, this.grid, this.cycles);
                else if (this.functionType == 2)
                    this._SpecialBitmap = ImageToolbox.open(this._SpecialBitmap, this.grid, this.cycles);
                else
                    this._SpecialBitmap = ImageToolbox.close(this._SpecialBitmap, this.grid, this.cycles);
            }

            this.UpdateThumb();
        }*/
    }
}
