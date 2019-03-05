using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE
{
    public static class Geometry
    {
        /// <summary>
        /// Give all edges for a closed shape.  Should be called on vector2 or vector3's
        /// </summary>
        /// <param name="vertices_loop"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Tuple<T, T>> EnumerateEdges<T>(IEnumerable<T> vertices_loop)
        {
            bool f = true;
            T first = default(T);
            T previous = default(T);

            foreach (var current in vertices_loop)
            {
                if (f)
                {
                    f = false;
                    first = current;
                }
                else
                {
                    yield return new Tuple<T, T>(previous, current);
                }
                previous = current;
            }

            // Wrap Around
            yield return new Tuple<T, T>(previous, first);
        }
    }
}
