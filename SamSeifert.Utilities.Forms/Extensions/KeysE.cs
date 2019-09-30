using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Extensions
{
    public static class KeysE
    {
        public static bool Contains(this Keys k, Keys modifier)
        {
            return (k & modifier) == modifier;
        }
    }
}
