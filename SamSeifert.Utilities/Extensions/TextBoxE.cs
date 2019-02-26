using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Json;

namespace SamSeifert.Utilities.Extensions
{
    public static class TextBoxE
    {
        public static void Unpack(this TextBox cb, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                cb.Text = outo as String;
        }

        public static void Pack(this TextBox cb, JsonDict dict, string key)
        {
            dict[key] = cb.Text;
        }
    }
}
