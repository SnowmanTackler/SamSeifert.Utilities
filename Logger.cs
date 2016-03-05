using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class Logger
    {
        public static Action<String> WriteLine = (String s) =>
        {
            Console.WriteLine(s);
        };
    }
}
