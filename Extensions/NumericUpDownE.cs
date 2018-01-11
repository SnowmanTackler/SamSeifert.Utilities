using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class NumericUpDownE
    {
        public static double ValueD(this NumericUpDown nud)
        {
            return (double)nud.Value;
        }

        public static float ValueF(this NumericUpDown nud)
        {
            return (float)nud.Value;
        }

        public static int ValueI(this NumericUpDown nud)
        {
            return (int)Math.Round(nud.Value);
        }
    }
}
