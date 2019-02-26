using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class Array2E
    {
        public static void Fill<T>(this T[,] ie, T t)
        {
            for (int i = 0; i < ie.GetLength(0); i++)
                for (int j = 0; j < ie.GetLength(1); j++)
                    ie[i, j] = t;
        }

        public static void Fill<T>(this T[,] ie, Func<T> t)
        {
            for (int i = 0; i < ie.GetLength(0); i++)
                for (int j = 0; j < ie.GetLength(1); j++)
                    ie[i, j] = t();
        }

        public static void Fill<T>(this T[,] ie, Func<int, int, T> t)
        {
            for (int i = 0; i < ie.GetLength(0); i++)
                for (int j = 0; j < ie.GetLength(1); j++)
                    ie[i, j] = t(i, j);
        }
    }
}
