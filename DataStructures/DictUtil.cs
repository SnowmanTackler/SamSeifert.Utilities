using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.DataStructures
{
    static class DictUtil
    {
        public static bool ArgMax<TKey, TValue>(this DefaultDict<TKey, TValue> dict, out TKey key, out TValue value)
        {
            key = default(TKey);
            value = default(TValue);

            if (!typeof(IComparable).IsAssignableFrom(typeof(TValue)))
                return false;

            bool first = true;
            foreach (var kvp in dict)
            {
                if (first) first = false;
                else if ((kvp.Value as IComparable).CompareTo(value) < 0) continue;
                key = kvp.Key;
                value = kvp.Value;
            }

            return true;
        }

        public static bool ArgMin<TKey, TValue>(this DefaultDict<TKey, TValue> dict, out TKey key, out TValue value)
        {
            key = default(TKey);
            value = default(TValue);

            if (!typeof(IComparable).IsAssignableFrom(typeof(TValue)))
                return false;

            bool first = true;
            foreach (var kvp in dict)
            {
                if (first) first = false;
                else if ((kvp.Value as IComparable).CompareTo(value) > 0) continue;
                key = kvp.Key;
                value = kvp.Value;
            }

            return true;
        }
    }
}
