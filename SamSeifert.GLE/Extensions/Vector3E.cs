using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Extensions
{
    public static class Vector3E
    {
        public static Vector3 Blend01(this float alpha, Vector3 value_at_0, Vector3 value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }


        public static Vector3 NormalizedSafe(this Vector3 vec)
        {
            var lens = vec.Length;
            if (lens == 0) return vec;
            else return vec / lens;
        }
    }
}
