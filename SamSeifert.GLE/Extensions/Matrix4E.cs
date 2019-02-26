using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Extensions
{
    public static class Matrix4E
    {
        /// <summary>
        /// Performs transpose of everything but translation
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix4 InvertedViewMatrix(this Matrix4 m)
        {
            m.Inverted();
            var test = new Vector4(m.Row3.Xyz, 0);
            m.Row3.Xyz = new Vector3(0);
            m.Transpose();
            m.Row3.Xyz = -(test * m).Xyz;
            return m;
        }
    }
}
