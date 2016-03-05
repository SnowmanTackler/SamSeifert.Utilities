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
    }
}
