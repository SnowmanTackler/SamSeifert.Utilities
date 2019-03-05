using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Extensions
{
    public static class Vector3Es
    {
        public static float PathLength(this ICollection<Vector3> vecs)
        {
            bool first = true;
            Vector3 current = Vector3.Zero;
            float dist = 0f;
            foreach (var p in vecs)
            {
                if (!first)
                    dist += (p - current).Length;
                first = false;
                current = p;
            }
            return dist;
        }
    }
}
