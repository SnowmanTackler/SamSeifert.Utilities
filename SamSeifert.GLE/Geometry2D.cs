using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SamSeifert.GLE
{
    public class Geometry2D
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
            /// Finds a point that is outside the given polygon
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <returns></returns>
            public static Vector2 PointOutside(IEnumerable<Vector2> polygon_vertices)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;

                foreach (var c in polygon_vertices)
                {
                    minX = Math.Min(minX, c.X);
                    minY = Math.Min(minY, c.Y);
                }

                minX -= 1;
                minY -= 1;

                return new Vector2(minX, minY);
            }

            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon_vertices,
                Vector2 pt)
            {
                return Polygon.ContainsPoint(
                    polygon_vertices,
                    pt,
                    PointOutside(polygon_vertices));
            }

            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon_vertices,
                Vector2 pt,
                Vector2 pt_outside_polygon)
            {
                int count = 0;
                foreach (var edge in Geometry.EnumerateEdges(polygon_vertices))
                    if (LineSegment.InteresectsLineSegment(pt, pt_outside_polygon, edge.Item1, edge.Item2))
                        count++;
                return count % 2 == 1;
            }
        }

        public static class PolygonConvex
        {
            /// <summary>
            /// Finds a point that is inside the given convex polygon
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <returns></returns>
            public static Vector2 PointInside(IEnumerable<Vector2> polygon_vertices)
            {
                Vector2 center = Vector2.Zero;
                int count = 0;
                foreach (var vec in polygon_vertices)
                {
                    center += vec;
                    count++;
                }
                if (count < 2) throw new Exception("Polygon needs more sides");
                return center / count;
            }

            /// <summary>
            /// Check if pt is inside closed polygon defined by polygon.  Polygon should be concave out always.
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <param name="pt"></param>
            /// <returns></returns>
            public static bool ContainsPoint(IEnumerable<Vector2> polygon_vertices, Vector2 pt)
            {
                return ContainsPoint(polygon_vertices, PointInside(polygon_vertices), pt);
            }

            /// <summary>
            /// Check if pt is inside closed polygon defined by polygon.  Polygon should be concave out always.
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <param name="point_inside_polygon"></param>
            /// <param name="pt"></param>
            /// <returns></returns>
            public static bool ContainsPoint(
                IEnumerable<Vector2> polygon_vertices, 
                Vector2 point_inside_polygon, 
                Vector2 pt)
            {
                // Checks if center and the pt are on the same side of each edge.
                // If they are, return true.
                // If they aren't, return false.

                foreach (var edge in Geometry.EnumerateEdges(polygon_vertices))
                    if (!SameSide(edge.Item1, edge.Item2, pt, point_inside_polygon))
                        return false;

                return true;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <param name="point_inside_polygon"></param>
            /// <param name="circle_center"></param>
            /// <param name="circle_radius"></param>
            /// <returns>True if the polygon is inside circle, or the circle is inside the polygon, or the perimeter of the circle insects the perimeter of the polygon</returns>
            public static bool IntersectsCircle(
                IEnumerable<Vector2> polygon_vertices,
                Vector2 point_inside_polygon,
                Vector2 circle_center, 
                float circle_radius)
            {
                return Circle.IntersectsPolygonConvex(circle_center, circle_radius, polygon_vertices, point_inside_polygon);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="polygon_vertices"></param>
            /// <param name="circle_center"></param>
            /// <param name="circle_radius"></param>
            /// <returns>True if the polygon is inside circle, or the circle is inside the polygon, or the perimeter of the circle insects the perimeter of the polygon</returns>
            public static bool IntersectsCircle(
                IEnumerable<Vector2> polygon_vertices,
                Vector2 circle_center,
                float circle_radius)
            {
                return Circle.IntersectsPolygonConvex(circle_center, circle_radius, polygon_vertices, PointInside(polygon_vertices));
            }

        }

        public class Circle
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="circle_center"></param>
            /// <param name="circle_radius"></param>
            /// <param name="polygon_vertices"></param>
            /// <returns>True if the polygon is inside circle, or the circle is inside the polygon, or the perimeter of the circle insects the perimeter of the polygon</returns>
            public static bool IntersectsPolygonConvex(Vector2 circle_center, float circle_radius, IEnumerable<Vector2> polygon_vertices)
            {
                return IntersectsPolygonConvex(circle_center, circle_radius, polygon_vertices, PolygonConvex.PointInside(polygon_vertices));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="circle_center"></param>
            /// <param name="circle_radius"></param>
            /// <param name="polygon_vertices"></param>
            /// <param name="point_inside_polygon"></param>
            /// <returns>True if the polygon is inside circle, or the circle is inside the polygon, or the perimeter of the circle insects the perimeter of the polygon</returns>
            public static bool IntersectsPolygonConvex(Vector2 circle_center, float circle_radius, IEnumerable<Vector2> polygon_vertices, Vector2 point_inside_polygon)
            {
                // Check if circle is inside polygon
                if (PolygonConvex.ContainsPoint(polygon_vertices, point_inside_polygon, circle_center))
                    return true;

                // Check if edge of polygon is inside circle at any point
                foreach (var edge in Geometry.EnumerateEdges(polygon_vertices))
                    if (LineSegment.DistanceToPoint(edge.Item1, edge.Item2, circle_center) <= circle_radius)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Non infinte lines
        /// </summary>
        public static class LineSegment
        {
            public static float DistanceToPoint(Vector2 line_pt1, Vector2 line_pt2, Vector2 pt)
            {
                Vector2 closest;
                return DistanceToPoint(line_pt1, line_pt2, pt, out closest);
            }

            public static float DistanceToPoint(Vector2 line_pt1, Vector2 line_pt2, Vector2 pt, out Vector2 closest)
            {
                var line_dir = line_pt2 - line_pt1;
                var line_lens = line_dir.Length;
                if (line_lens == 0)
                {
                    closest = line_pt1;
                    return (line_pt1 - pt).Length;
                }

                line_dir /= line_lens;

                var pt_dir = pt - line_pt1;

                var dot = Vector2.Dot(line_dir, pt_dir);

                if (dot < 0)
                {
                    closest = line_pt1;
                    return (line_pt1 - pt).Length;
                }
                else if (dot > line_lens)
                {
                    closest = line_pt2;
                    return (line_pt2 - pt).Length;
                }
                else
                {
                    closest = line_pt1 + dot * line_dir;
                    return (pt_dir - dot * line_dir).Length;
                }
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
