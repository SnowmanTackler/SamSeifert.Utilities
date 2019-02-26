using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Json;

namespace SamSeifert.Utilities.Extensions
{
    public static class CheckBoxE
    {
        public static void Unpack(this CheckBox cb, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                cb.Checked = (bool)outo;
        }

        public static void Pack(this CheckBox cb, JsonDict dict, string key)
        {
            dict[key] = cb.Checked;
        }
    }
}
