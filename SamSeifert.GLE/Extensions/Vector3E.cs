using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>An arbitrary vector perpindicular to this one</returns>
        public static Vector3 Perpindicular(this Vector3 vec)
        {
            Vector3 other = Vector3.UnitX;
            if (Math.Abs(Vector3.Dot(other, vec.Normalized())) > 0.707f)
                other = Vector3.UnitY;

            return Vector3.Cross(vec, other).Normalized();
        }

        public static Vector3 toVector3(this Color c)
        {
            return new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
        }
    }
}
