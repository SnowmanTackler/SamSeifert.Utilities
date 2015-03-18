using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.HueChooser;
using SamSeifert.ImageProcessing;

namespace ImageToolbox.Tools
{
    public partial class ColorFilterFull : ToolDetails
    {
        private HueChooser.ColorFilterOptions _ColorFilterOptions;

        public ColorFilterFull(HueChooser.ColorFilterOptions colorFilterOptions)
        {
            this.InitializeComponent();
            this._ColorFilterOptions = colorFilterOptions;
            this.hueChooser1.setInitialValues(colorFilterOptions);
        }

        public override Sect updateOverride(Sect indata)
        {
            if (indata == null) return null;
            return this.modifySpecialBitmap(indata); ;
        }

        public unsafe Sect modifySpecialBitmap(Sect indata)
        {
            return indata.Clone();
/*            Sect o = indata.Clone();

            Sect r, g, b;

            o.getRGB(out r, out g, out b, DataType.ReadWrite);

            for (int y = 0; y < o.Height; y++)
                for (int x = 0; x < o.Width; x++)
                    HueChooser.filterColor(
                        ref r[y, x],
                        ref g[y, x],
                        ref b[y, x],
                        this._ColorFilterOptions);

            return o;*/
        }

        public void hueChooser1_ValueChanged(object sender, EventArgs e)
        {
            this._ColorFilterOptions.HueBandCenter = this.hueChooser1.HueBandCenter;
            this._ColorFilterOptions.HueBandWidth = this.hueChooser1.HueBandWidth;
            this._ColorFilterOptions.GrayBack = this.hueChooser1.GrayBack;
            this.updateImage();
        }

    }
}
