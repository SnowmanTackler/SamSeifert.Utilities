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

        public static T PopLast<T>(this List<T> data)
        {
            int last = data.Count - 1;
            var ret = data[last];
            data.RemoveAt(last);
            return ret;
        }

        public static bool AddIfNotNull<T>(this List<T> data, T item)
        {
            if (item == null)
            {
                return false;
            }
            else {
                data.Add(item);
                return true;
            }
        }

        public static void Shuffle<T>(this List<T> array, Random rand)
        {
            int n = array.Count;
            for (int i = 0; i < n; i++)
            {
                // Pick a new index higher than current for each item in the array
                int r = i + rand.Next(0, n - i);
                T temp = array[r];
                array[r] = array[i];
                array[i] = temp;
            }
        }

        public static void MatchLength<T>(this List<T> ls, int length, Func<T> func)
        {
            while (ls.Count < length)
                ls.Add(func());
            while (ls.Count > length)
                ls.RemoveAt(ls.Count - 1);
        }







        public static void ArgMax<T>(this List<T> ls, out int index, out float value, Func<T, float> f)
        {
            ls.ArgMin(out index, out value, (nf) => -f(nf));
        }

        public static void ArgMin<T>(this List<T> ls, out int index, out float value, Func<T, float> f)
        {
            value = float.MaxValue;
            index = -1;
            for (int j = 0; j < ls.Count; j++)
            {
                float dist = f(ls[j]);
                if (dist < value)
                {
                    index = j;
                    value = dist;
                }
            }
        }
    }
}
