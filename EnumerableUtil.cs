using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class EnumerableUtil
    {
        /// <summary>
        /// Throws an exception if the dictionary is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static float ArgMax<T>(this Dictionary<float, T> e) where T : IComparable<T>
        {
            if (e.Count == 0) throw new Exception("No Entries");

            float ret = 0;

            T min = e.First().Value;

            foreach (var kvp in e)
               if (kvp.Value.CompareTo(min) >= 0) // Must do greater than equals so we catch first
                {
                    min = kvp.Value;
                    ret = kvp.Key;
                }

            return ret;
        }

        /// <summary>
        /// Returns number of non zeros!
        /// </summary>
        /// <returns></returns>
        public static int NumberOfNonZeros(this IEnumerable<int> nums)
        {
            int ret = 0;
            foreach (var num in nums)
                if (num != 0)
                    ret++;
            return ret;
        }

        /// <summary>
        /// Returns number of non zeros!
        /// </summary>
        /// <returns></returns>
        public static int NumberOfNonZeros(this IEnumerable<float> nums)
        {
            int ret = 0;
            foreach (var num in nums)
                if (num != 0)
                    ret++;
            return ret;
        }

        /// <summary>
        /// Get a sub array from data!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Get a sub array from data!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<T> SubList<T>(this List<T> data, int index, int length)
        {
            return data.GetRange(index, length);
        }

        public static T[] SubArray<T>(this T[] data, int index)
        {
            return data.SubArray(index, data.Length - index);
        }

        public static List<T> SubList<T>(this List<T> data, int index)
        {
            return data.GetRange(index, data.Count - index);
        }


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











        public static void Add<T>(this HashSet<T> t, IEnumerable<T> ie)
        {
            foreach (var i in ie)
                t.Add(i);
        }

    }
}
