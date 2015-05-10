using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.HueChooser;
using SamSeifert.CSCV;

namespace CSCV_IDE.Tools
{
    public class ColorFilter : ToolDefault
    {
        private static string name = "Color Filter";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(ColorFilter.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellColorFilter);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new ColorFilter());
        }






        private HueChooser.ColorFilterOptions _ColorFilterOptions = new HueChooser.ColorFilterOptions(0.5f, 0.25f, true);

        public ColorFilter()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
            this.ToolStrip_Edit.Enabled = true;
        }

        public override void MenuEdit()
        {
            new ColorFilterFull(this._ColorFilterOptions).ShowDialog(this._ImageDataLast);
            this.StatusChanged();
        }


        private Sect _ImageDataLast = null;
        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            this._ImageDataLast = d;

            Sect o = d.Clone();

/*            float[,] r, g, b;
            o.getRGB(out r, out g, out b, DataType.ReadWrite);

            for (int y = 0; y < o.Height; y++)
                for (int x = 0; x < o.Width; x++)
                    HueChooser.filterColor(
                        ref r[y,x],
                        ref g[y,x],
                        ref b[y,x],
                        this._ColorFilterOptions);*/

            return o;
        }
    }
}
