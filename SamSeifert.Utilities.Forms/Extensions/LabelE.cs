using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class LabelE
    {
        public static void UpdateAndSetText(this System.Windows.Forms.Label l, Timing.RateTracker rt)
        {
            l.Text = rt.Update().ToString("00.00") + " Hz";
        }
    }
}
