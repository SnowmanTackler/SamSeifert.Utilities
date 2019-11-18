using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class ArrayE
    {
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
                Misc.Util.Swap(ref array[r], ref array[i]);
            }
        }



        public static void ArgMax<T>(this T[] array, out int index, out float value, Func<T, float> f)
        {
            array.ArgMin(out index, out value, (nf) => -f(nf));
        }

        public static void ArgMin<T>(this T[] array, out int index, out float value, Func<T, float> f)
        {
            value = float.MaxValue;
            index = -1;
            for (int j = 0; j < array.Length; j++)
            {
                float dist = f(array[j]);
                if (dist < value)
                {
                    index = j;
                    value = dist;
                }
            }
        }

        public static void ArgMax<T>(this T[] array, out int index, out double value, Func<T, double> f)
        {
            array.ArgMin(out index, out value, (nf) => -f(nf));
        }

        public static void ArgMin<T>(this T[] array, out int index, out double value, Func<T, double> f)
        {
            value = float.MaxValue;
            index = -1;
            for (int j = 0; j < array.Length; j++)
            {
                double dist = f(array[j]);
                if (dist < value)
                {
                    index = j;
                    value = dist;
                }
            }
        }




        public static T[] Merge<T>(T[] a1, T[] a2)
        {
            int l1 = a1.Length;
            int l2 = a2.Length;
            var ret = new T[l1 + l2];
            Array.Copy(a1, 0, ret, 0, l1);
            Array.Copy(a2, 0, ret, l1, l2);
            return ret;
        }
    }
}
