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

        public static Action<String> Writer
        {
            set; private get;
        } = (String s) => {
            Trace.WriteLine(s);
        };
    }
}
