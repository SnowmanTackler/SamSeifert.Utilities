﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class StringE
    {
        public static Stream AsStream(this string s)
        {
            return s.AsStream(Encoding.UTF8);
        }

        public static Stream AsStream(this string s, Encoding e)
        {
            return new MemoryStream(e.GetBytes(s));
        }

        public static String RemoveDoubleSpaces(this string s)
        {
            int lens = -1;

            while (lens != s.Length)
            {
                lens = s.Length;
                s = s.Replace("  ", " ");
            }

            return s;
        }
    }
}
