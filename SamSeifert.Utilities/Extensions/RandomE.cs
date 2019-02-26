using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class RandomE
    {
        public static float NextFloat(this Random r)
        {
            return (float) r.NextDouble();
        }

        /// <summary>
        /// Returns a zero mean, 1 standard deviation gaussian.
        /// </summary>
        /// <param name="std"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static float NextGaussian(this Random rand, Single std = 1.0f, Single mean = 0.0f)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + std * (float)randStdNormal;
            // double randNormal = mean + stdDev * randStdNormal;
        }

        /// <summary>
        /// Returns a zero mean, 1 standard deviation gaussian.
        /// </summary>
        /// <param name="std"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static Double NextGaussian(this Random rand, Double std = 1.0f, Double mean = 0.0f)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + std * randStdNormal;
            // double randNormal = mean + stdDev * randStdNormal;
        }
    }
}
