using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class ListE
    {
        public static List<T> SubList<T>(this List<T> data, int index, int length)
        {
            return data.GetRange(index, length);
        }

        public static List<T> SubList<T>(this List<T> data, int index)
        {
            return data.GetRange(index, data.Count - index);
        }
    }
}
