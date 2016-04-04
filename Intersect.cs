using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE
{
    public class Intersect
    {
        /// <summary>
        /// Computes the cross product of two 2d vectors
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private static float Cross(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.Y) - (v1.Y * v2.X);
        }

        /// <summary>
        /// Checks if p1 and p2 are on the same side of the line defined by l1 and l2;
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static bool SameSide(Vector2 l1, Vector2 l2, Vector2 p1, Vector2 p2)
        {
            p1 -= l1;
            p2 -= l1;
            l2 -= l1;
            // l1 is now origin.
            return Cross(l2, p1) * Cross(l2, p2) >= 0;
        }



        public static class Polygon
        {
            /// <summary>
            /// Give all edges for a closed polygon
            /// </summary>
            /// <param name="polygon"></param>
            /// <returns></returns>
            private static IEnumerable<Tuple<Vector2, Vector2>> EnumeratePolygonEdges(IEnumerable<Vector2> polygon)
            {
                bool f = true;
                Vector2 first = Vector2.Zero;
                Vector2 previous = Vector2.Zero;

                foreach (var current in polygon)
                {
                    if (f)
                    {
                        f = false;
                        first = current;
                    }
                    else
                    {
                        yield return new Tuple<Vector2, Vector2>(previous, current);
                    }
                    previous = current;
                }

                // Wrap Around
                yield return new Tuple<Vector2, Vector2>(previous, first);
            }

            /// <summary>
            /// Check if pt is inside closed polygon defined by polygon.  Polygon should be concave out always.
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="pt"></param>
            /// <returns></returns>
            public static bool ContainsPoint(IEnumerable<Vector2> polygon, Vector2 pt)
            {
                Vector2 center = Vector2.Zero;
                int count = 0;
                foreach (var vec in polygon)
                {
                    center += vec;
                    count++;
                }
                if (count == 0) return false;
                else return ContainsPoint(polygon, center / count, pt);
            }

            /// <summary>
            /// Check if pt is inside closed polygon defined by polygon.  Polygon should be concave out always.
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="polygon_center"></param>
            /// <param name="pt"></param>
            /// <returns></returns>
            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon, 
                Vector2 polygon_center, 
                Vector2 pt)
            {
                // Checks if center and the pt are on the same side of each edge.
                // If they are, return true.
                // If they aren't, return false.

                foreach (var edge in EnumeratePolygonEdges(polygon))
                    if (!SameSide(edge.Item1, edge.Item2, pt, polygon_center))
                        return false;

                return true;
            }

            public static bool IntersectsCircle(
                IEnumerable<Vector2> polygon,
                Vector2 polygon_center,
                Vector2 circle_center, 
                float circle_radius)
            {

                // Check if circle is inside polygon
                if (ContainsPoint(polygon, polygon_center, circle_center))
                    return true;

                // Check if one of corners is inside circle
                foreach (var corner in polygon)
                    if ((circle_center - corner).Length <= circle_radius)
                        return true;

                // Check if edge of polygon intersects circle
                foreach (var edge in EnumeratePolygonEdges(polygon))
                {
                    var point = circle_center - edge.Item1;
                    var line = edge.Item2 - edge.Item1;

                    var line_length = line.Length;

                    line /= line_length;

                    var dot = Vector2.Dot(line, point);

                    // Closes point on circle to infinite line isn't on actual line.
                    if (dot < 0) continue;
                    else if (dot > line_length) continue;

                    var projected = line * Vector2.Dot(line, point);

                    if ((point - projected).Length <= circle_radius) return true;
                }

                return false;
            }


        }



    }
}
