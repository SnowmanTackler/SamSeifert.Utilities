using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class ArrayE
    {
        public static T[] Sorted<T>(this IEnumerable<T> ie)
        {
            var arg = ie.ToArray();
            Array.Sort(arg);
            return arg;
        }

        public static void Fill<T>(this T[] ie, T t)
        {
            for (int i = 0; i < ie.Length; i++)
                ie[i] = t;
        }

        public static void Fill<T>(this T[] ie, Func<T> t)
        {
            for (int i = 0; i < ie.Length; i++)
                ie[i] = t();
        }

        public static void Fill<T>(this T[] ie, Func<int, T> t)
        {
            for (int i = 0; i < ie.Length; i++)
                ie[i] = t(i);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int index)
        {
            return data.SubArray(index, data.Length - index);
        }

        public static void Shuffle<T>(this T[] array, Random rand)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                // Pick a new index higher than current for each item in the array
                int r = i + rand.Next(0, n - i);
                MiscUtil.Swap(ref array[r], ref array[i]);
            }
        }
    }
}
