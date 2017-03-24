using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace SamSeifert.Utilities.Json
{
    public interface JsonPackable
    {
        JsonDict Pack();
        void Unpack(JsonDict dict);
    }

    public static class Extensions
    {
        public static void Unpack(this NumericUpDown nud, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                nud.Value = MathUtil.Clamp(nud.Minimum, nud.Maximum, (decimal)(double)outo);
        }

        public static void Pack(this NumericUpDown nud, JsonDict dict, string key)
        {
            dict[key] = (double)nud.Value;
        }

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

        public static void Unpack(this ComboBox cb, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                cb.SelectedIndex = Math.Max(0, Math.Min(cb.Items.Count - 1, (int)Math.Round((double)outo)));
        }

        public static void Pack(this ComboBox cb, JsonDict dict, string key)
        {
            dict[key] = cb.SelectedIndex;
        }


        public static T asGeneric<T>(this JsonDict d, String key)
        {
            return (T)d[key];
        }

        public static T asGeneric<T>(this JsonDict d, String key, T empty_or_error_value)
        {
            object outo;
            if (d.TryGetValue(key, out outo))
                if (outo is T)
                    return (T)outo;

            return empty_or_error_value;
        }

    }
}
