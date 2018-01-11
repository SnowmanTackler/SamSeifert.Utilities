using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class HashSetE
    {
        public static void Add<T>(this HashSet<T> t, IEnumerable<T> ie)
        {
            foreach (var i in ie)
                t.Add(i);
        }
    }
}
