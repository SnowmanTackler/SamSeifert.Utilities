using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Logging
{
    public static class Logger
    {
        public static volatile BaseLogger Default = new TimeLogger();
    }
}
