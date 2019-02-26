using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Maths
{
    public static class Statistics
    {
        /// <summary>
        /// Samples will be normalized to sum to 1, so do whatever you want!
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Entropy(this IEnumerable<int> a)
        {
            double sum = a.Sum();

            if (sum == 0) throw new Exception("No Input!");

            double entropy = 0;

            foreach (var i in a)
            {
                if (i == 0) continue;
                double p = i / sum;
                entropy -= p * Math.Log(p, 2);
            }

            return entropy;
        }


        /// <summary>
        /// Samples will be normalized to sum to 1, so do whatever you want!
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Entropy(this IEnumerable<float> a)
        {
            double sum = a.Sum();

            if (sum == 0) throw new Exception("No Input!");

            double entropy = 0;

            foreach (var i in a)
            {
                if (i == 0) continue;
                double p = i / sum;
                entropy -= p * Math.Log(p, 2);
            }

            return entropy;
        }
    }
}
