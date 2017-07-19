using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class ArrayUtil<T>
    {
        public static T[] Merge(T[] a1, T[] a2)
        {
            int l1 = a1.Length;
            int l2 = a2.Length;
            var ret = new T[l1 + l2];
            Array.Copy(a1, 0, ret, 0, l1);
            Array.Copy(a2, 0, ret, l1, l2);
            return ret;
        }
    }
}
