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
        public static Action<String> WriteLine = (String s) =>
        {
            Trace.WriteLine(s);
        };
    }
}
