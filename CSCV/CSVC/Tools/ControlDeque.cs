using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace ImageToolbox.Tools
{
    class ControlDeque
    {
        public static NumericUpDown NumericUpDown(int decimal_places)
        {
            var nud = new NumericUpDown();
            nud.BackColor = System.Drawing.Color.Black;
            nud.ForeColor = System.Drawing.Color.White;
            nud.Margin = new System.Windows.Forms.Padding(0);
            nud.Maximum = 1;
            nud.Minimum = 0;
            nud.Increment = (Decimal)Math.Pow(0.1, decimal_places);
            nud.Value = 0;
            nud.DecimalPlaces = decimal_places;
            nud.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nud.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            return nud;
        }

        public static ComboBox ComboBox()
        {
            var cb = new ComboBox();
            cb.BackColor = System.Drawing.Color.Black;
            cb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            cb.ForeColor = System.Drawing.Color.White;
            cb.FormattingEnabled = true;
            cb.Margin = new System.Windows.Forms.Padding(0);
            cb.MaxDropDownItems = 10;
            return cb;
        }
    }
}
