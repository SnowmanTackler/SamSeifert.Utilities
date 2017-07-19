using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class StringUtil
    {
        public static Stream AsStream(this string s)
        {
            return s.AsStream(Encoding.UTF8);
        }

        public static Stream AsStream(this string s, Encoding e)
        {
            return new MemoryStream(e.GetBytes(s));
        }

    }
}
