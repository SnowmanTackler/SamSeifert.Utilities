using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class TimerE
    {
        /// <summary>
        /// Also starts timer by enabling it.
        /// </summary>
        /// <param name="t"></param>
        public static void Zero(this Timer t)
        {
            t.Enabled = false;
            t.Enabled = true;
        }
    }
}
