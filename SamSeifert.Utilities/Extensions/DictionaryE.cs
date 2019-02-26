﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class DictionaryE
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
    }
}