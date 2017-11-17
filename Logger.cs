using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class Logger
    {
        public static void WriteLine()
        {
            Logger.Writer("");
        }

        public static void WriteLine(String s)
        {
            Logger.Writer(s);
        }

        public static void WriteError(Object sender, String hint)
        {
            Type t = (sender is Type) ? (sender as Type) : sender.GetType();
            Logger.Writer("Error -" + t.FullName + ": " + hint);
        }

        public static void WriteException(Object sender, String hint, Exception exc)
        {
            Type t = (sender is Type) ? (sender as Type) : sender.GetType();
            Logger.Writer("Exception -" + t.FullName + ": " + hint + ", " + exc.ToString());
        }


        private static volatile Action<String> _Writer = (String s) => {
            Trace.WriteLine(s);
        };

        public static Action<String> Writer
        {            
            set
            {
                Logger._Writer = value;
            }
            private get
            {
                return Logger._Writer;
            }
        }

    }
}
