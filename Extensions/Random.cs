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
    }
}
