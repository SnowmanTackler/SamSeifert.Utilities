using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class MiscUtil
    {
        public static void Swap<T>(ref T i, ref T o)
        {
            T temp = i;
            i = o;
            o = temp;
        }

        public static String GetExecutablePath()
        {
            return System.Reflection.Assembly.GetEntryAssembly().Location;
        }
    }
}
