using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.CSCV;

namespace CSCV_IDE.Tools
{
    public partial class HistogramEqualizer : ToolDefault
    {
        private static string name = "Histogram Flattener";

        public static ToolButtonDefault getAddCellButton()
        {
            ToolButtonDefault button = new ToolButtonDefault();
            button.setClickHandler(new MouseEventHandler(HistogramEqualizer.addToWorkSpace));
            button.setText(name);
            button.setImage(Properties.Resources.AddCellHistogram);
            return button;
        }

        public static void addToWorkSpace(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormMain.addCell(new HistogramEqualizer());
        }
        
        public HistogramEqualizer()
            : base(true, true)
        {
            this.checkBoxName.Text = name;
        }

        public override Sect SpecialBitmapUpdateDefault(ref Sect d)
        {
            Sect o = null;
            IA_Single.HistogramEqualize(d, ref o);
            return o;
        }
    }
}
