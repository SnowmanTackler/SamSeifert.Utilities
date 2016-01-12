using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class Helpers
    {
        public static void Swap<T>(ref T i, ref T o)
        {
            T temp = i;
            i = o;
            o = temp;
        }
    }
}
