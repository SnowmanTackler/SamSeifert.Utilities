﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

namespace SamSeifert.MathNet.Numerics.Extensions
{
    public static class VectorUtil
    {
        /// <summary>
        /// 2 normal!
        /// </summary>
        /// <param name="v"></param>
        /// <returns>the length of the vetctor</returns>
        public static float Normalize(this Vector<float> v)
        {
            float lens = 0;

            foreach (var f in v)
                lens += f * f;

            lens = (float)Math.Sqrt(lens);

            for (int i = 0; i < v.Count; i++)
                v[i] /= lens;

            return lens;
        }

        public static float StandardDeviation(this IEnumerable<float> vector)
        {
            float mean;
            return vector.StandardDeviation(out mean);
        }

        public static float StandardDeviation(this IEnumerable<float> vector, out float mean)
        {
            float xi2 = 0;
            float xi = 0;
            int count = 0;

            foreach (float val in vector)
            {
                count++;
                xi += val;
                xi2 += val * val;
            }

            mean = xi / count;

            return (float)Math.Sqrt(xi2 / count - mean * mean);
        }


    }
}
