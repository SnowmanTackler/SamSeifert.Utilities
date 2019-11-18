using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class IEnumerableE
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> func)
        {
            foreach (var arg in ie)
            {
                func(arg);
            }
        }

        public static T[] Sorted<T>(this IEnumerable<T> ie)
        {
            var arg = ie.ToArray();
            Array.Sort(arg);
            return arg;
        }

    }
}
