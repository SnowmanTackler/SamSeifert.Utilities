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
