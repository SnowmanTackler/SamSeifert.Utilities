using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SamSeifert.Utilities.Files.Json;

namespace SamSeifert.Utilities.Extensions
{
    public static class ComboBoxE
    {
        public static void Unpack(this ComboBox cb, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                cb.SelectedIndex = Math.Max(0, Math.Min(cb.Items.Count - 1, (int)Math.Round((double)outo)));
        }

        public static void Pack(this ComboBox cb, JsonDict dict, string key)
        {
            dict[key] = (double)cb.SelectedIndex;
        }
    }
}
