using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class Permutate<T>
    {
        /// <summary>
        /// Maintains Length, Just Reorders
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> AllOrderingsOf(T[] data)
        {
            var ls = new List<int>();
            for (int i = 0; i < data.Length; i++)
                ls.Add(i);

            return AllOrderingsOf(ls.ToArray(), new int[0], data);
        }

        private static IEnumerable<T[]> AllOrderingsOf(int[] to_select, int[] selected, T[] data)
        {
            if (to_select.Length == 0)
            {
                var ls = new List<T>();
                foreach (var sel in selected)
                    ls.Add(data[sel]);

                yield return ls.ToArray();
            }
            else
            {
                for (int i = 0; i < to_select.Length; i++)
                {
                    var new_to_select = new List<int>(to_select);
                    var new_selected = new List<int>(selected);

                    new_selected.Add(new_to_select[i]);
                    new_to_select.RemoveAt(i);

                    foreach (var t in AllOrderingsOf(new_to_select.ToArray(), new_selected.ToArray(), data))
                        yield return t;
                }
            }
        }






        public static IEnumerable<bool[]> AllOnAndOffs(int count)
        {
            var ret = new bool[count];
            for (int i = 0; i < count; i++)
                ret[i] = false;

            return AllOnAndOffs(count - 1, ret);
        }

        private static IEnumerable<bool[]> AllOnAndOffs(int index, bool[] ret)
        {
            ret[index] = true;

            if (index == 0) yield return ret;
            else
                foreach (var r in AllOnAndOffs(index - 1, ret))
                    yield return r;

            ret[index] = false;

            if (index == 0) yield return ret;
            else
                foreach (var r in AllOnAndOffs(index - 1, ret))
                    yield return r;

        }

        public static IEnumerable<T[]> AllSubsetsOfSize_MaintainOrder(T[] data, int size)
        {
            if (size <= 0) yield break;
               
            for (int i = 0; i < data.Length + 1 - size; i++)
            {
                T[] ret = new T[size];
                ret[0] = data[i];

                if (size > 1)
                {
                    foreach (var sa in AllSubsetsOfSize_MaintainOrder(data.SubArray(i + 1), size - 1))
                    {
                        Array.Copy(sa, 0, ret, 1, size - 1);
                        yield return ret;
                    }
                }
                else yield return ret;
            }
    
        }
    }

}

