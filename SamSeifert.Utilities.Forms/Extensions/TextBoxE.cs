using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Files.Json;

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


        /// <summary>
        /// For Console Logging
        /// </summary>
        public static void UseAsLogger(this TextBox tb, Func<String> get_prefix = null)
        {
            Logger.WriterSupportTabs = (String message, String indent) =>
            {
                if (message.Length != 0)
                    if (get_prefix != null)
                        message = get_prefix() + "  " + indent + message;

                Console.WriteLine(message);
                //Trace.WriteLine(message);

                if (tb == null) return;
                if (tb.IsDisposed) return;

                var act = new Action(() => {
                    tb.AppendText(message + Environment.NewLine);
                });

                if (tb.InvokeRequired) tb.BeginInvoke(act); // Don't Wait
                else act();
            };
        }

    }
}
