using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE
{
    public class Geometry
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
            public static IEnumerable<Tuple<Vector2, Vector2>> EnumerateEdges(IEnumerable<Vector2> polygon)
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

            public static Vector2 PointOutside(IEnumerable<Vector2> polygon)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;

                foreach (var c in polygon)
                {
                    minX = Math.Min(minX, c.X);
                    minY = Math.Min(minY, c.Y);
                }

                minX -= 1;
                minY -= 1;

                return new Vector2(minX, minY);
            }

            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon,
                Vector2 pt)
            {
                return Polygon.ContainsPoint(
                    polygon,
                    pt,
                    PointOutside(polygon));
            }

            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon,
                Vector2 pt,
                Vector2 pt_outside_polygon)
            {
                int count = 0;
                foreach (var edge in EnumerateEdges(polygon))
                    if (LineSegment.InteresectsLineSegment(pt, pt_outside_polygon, edge.Item1, edge.Item2))
                        count++;
                return count % 2 == 1;
            }
        }

        public static class PolygonConvex
        {
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

                foreach (var edge in Polygon.EnumerateEdges(polygon))
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
                foreach (var edge in Polygon.EnumerateEdges(polygon))
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

        /// <summary>
        /// Non infinte lines
        /// </summary>
        public static class LineSegment
        {
            public static float Distance(Vector2 line_pt1, Vector2 line_pt2, Vector2 pt)
            {
                var line_dir = line_pt2 - line_pt1;
                var line_lens = line_dir.Length;
                if (line_lens == 0)
                    return (line_pt1 - pt).Length;

                line_dir /= line_lens;

                var pt_dir = pt - line_pt1;

                var dot = Vector2.Dot(line_dir, pt_dir);

                if (dot < 0) return (line_pt1 - pt).Length;
                else if (dot > line_lens) return (line_pt2 - pt).Length;
                else return (pt_dir - dot * line_dir).Length;
            }

            public static bool InteresectsLineSegment(
                Vector2 line_segment_1_p1,
                Vector2 line_segment_1_p2,
                Vector2 line_segment_2_p1,
                Vector2 line_segment_2_p2)
            {
                Vector2 intersection;
                return LineSegment.InteresectsLineSegment(
                    line_segment_1_p1,
                    line_segment_1_p2,
                    line_segment_2_p1,
                    line_segment_2_p2,
                    out intersection);
            }

            public static bool InteresectsLineSegment(
                Vector2 line_segment_1_p1, 
                Vector2 line_segment_1_p2,
                Vector2 line_segment_2_p1, 
                Vector2 line_segment_2_p2,
                out Vector2 intersection)
            {
                // Get the segments' parameters.
                float dx12 = line_segment_1_p2.X - line_segment_1_p1.X;
                float dy12 = line_segment_1_p2.Y - line_segment_1_p1.Y;
                float dx34 = line_segment_2_p2.X - line_segment_2_p1.X;
                float dy34 = line_segment_2_p2.Y - line_segment_2_p1.Y;

                // Solve for t1 and t2
                float denominator = (dy12 * dx34 - dx12 * dy34);

                float t1 = ((line_segment_1_p1.X - line_segment_2_p1.X) * dy34 + (line_segment_2_p1.Y - line_segment_1_p1.Y) * dx34) / denominator;
                if (float.IsInfinity(t1))
                {
                    // The lines are parallel (or close enough to it).
                    intersection = new Vector2(float.NaN, float.NaN);
                    return false;
                }

                // lines_intersect = true;

                float t2 = ((line_segment_2_p1.X - line_segment_1_p1.X) * dy12 + (line_segment_1_p1.Y - line_segment_2_p1.Y) * dx12) / -denominator;

                // Find the point of intersection.
                intersection = new Vector2(line_segment_1_p1.X + dx12 * t1, line_segment_1_p1.Y + dy12 * t1);

                // The segments intersect if t1 and t2 are between 0 and 1.
                return ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));
            }
        }



    }
}
