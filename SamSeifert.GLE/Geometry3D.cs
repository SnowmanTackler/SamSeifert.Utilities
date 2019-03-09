using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SamSeifert.GLE
{
    public static class Geometry3D
    {
        /// <summary>
        /// Checks if p1 and p2 are on the same side of the line defined by l1 and l2, with respect the plane defined by its normal;
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SameSide(Vector3 l1, Vector3 l2, Vector3 p1, Vector3 p2, Vector3 plane_normal)
        {
            p1 -= l1;
            p2 -= l1;
            l2 -= l1;
            // l1 is now origin.
            return Vector3.Dot(Vector3.Cross(l2, p1), plane_normal) * Vector3.Dot(Vector3.Cross(l2, p2), plane_normal) >= 0;
        }

        public class Sphere
        {
            /// <summary>
            /// True if the polygon is inside circle, or the circle is inside the polygon, or the perimeter of the circle insects the perimeter of the polygon
            /// </summary>
            /// <param name="sphere_center"></param>
            /// <param name="sphere_radius"></param>
            /// <param name="polygon_vertices"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IntersectsPolygonConvex(Vector3 sphere_center, float sphere_radius, IEnumerable<Vector3> polygon_vertices)
            {
                int count = 0;

                Vector3 point_inside_polygon = Vector3.Zero;
                var first_three = new Vector3[3]; // Define a plane
                foreach (var p in polygon_vertices)
                {
                    if (count < 3)
                        first_three[count] = p;
                    point_inside_polygon += p;
                    count++;
                }

                if (count <= 2) throw new Exception("Polygons need more than 2 vertices");

                // Check if polygon is completely within circle (if it is, all of the vertices will be within circle, so only check one)
                if ((first_three[0] - sphere_center).Length < sphere_radius)
                    return true;

                point_inside_polygon /= count;

                Vector3 plane_normal;

                // Start by projecting the sphere onto the polygon plane.  Find the point of its center, and find the cross sectional radius at that point
                float projected_radius;
                Vector3 projected_center;
                {
                    var pivot_point = first_three[1];
                    plane_normal = Vector3.Cross(first_three[0] - pivot_point, first_three[2] - pivot_point).Normalized();
                    var diff = sphere_center - pivot_point;
                    float distance_from_sphere_center_to_plane = Vector3.Dot(diff, plane_normal);
                    projected_center = pivot_point + diff - plane_normal * distance_from_sphere_center_to_plane;

                    distance_from_sphere_center_to_plane = Math.Abs(distance_from_sphere_center_to_plane);
                    if (distance_from_sphere_center_to_plane > sphere_radius) return false;
                    projected_radius = (float)Math.Sqrt((2 * sphere_radius - distance_from_sphere_center_to_plane) * distance_from_sphere_center_to_plane); 
                }

                // Check if circle is inside polygon
                bool all_same_side = true;
                foreach (var edge in Geometry.EnumerateEdges(polygon_vertices))
                    if (!SameSide(edge.Item1, edge.Item2, projected_center, point_inside_polygon, plane_normal))
                    {
                        all_same_side = false;
                        break;
                    }
                if (all_same_side) return true;

                // Check if edge of polygon is inside circle at any point
                Vector3 closest;
                foreach (var edge in Geometry.EnumerateEdges(polygon_vertices))
                    if (LineSegment.DistanceToPoint(edge.Item1, edge.Item2, projected_center, out closest) <= projected_radius)
                        return true;

                return false;
            }
        }


        /// <summary>
        /// Non infinte lines
        /// </summary>
        public class LineSegment
        {
            public readonly Vector3 _Point1;
            public readonly Vector3 _Point2;

            public LineSegment(Vector3 pt1, Vector3 pt2)
            {
                this._Point1 = pt1;
                this._Point2 = pt2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float DistanceToPoint(Vector3 line_pt1, Vector3 line_pt2, Vector3 pt)
            {
                Vector3 closest;
                return DistanceToPoint(line_pt1, line_pt2, pt, out closest);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float DistanceToPoint(Vector3 line_pt1, Vector3 line_pt2, Vector3 pt, out Vector3 closest)
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

                var dot = Vector3.Dot(line_dir, pt_dir);

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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IntersectsTriangle(Triangle t, out Vector3 intersection)
            {
                return IntersectsTriangle(this._Point1, this._Point2, t._Point1, t._Point2, t._Point3, out intersection);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IntersectsTriangle(Vector3 line_pt1, Vector3 line_pt2, Vector3 t1, Vector3 t2, Vector3 t3, out Vector3 intersection)
            {
                var triangle_norm = Vector3.Cross(t1 - t2, t3 - t2).Normalized();

                var line_norm = (line_pt1 - line_pt2).Normalized();

                var x = Vector3.Dot(t1 - line_pt1, triangle_norm) / Vector3.Dot(line_norm, triangle_norm);

                intersection = line_pt1 + line_norm * x;

                if (x <= 0) return false;
                if (x >= (line_pt1 - line_pt2).Length) return false;

                if (!SameSide(t1, t2, t3, intersection, triangle_norm)) return false;
                if (!SameSide(t1, t3, t2, intersection, triangle_norm)) return false;
                if (!SameSide(t2, t3, t1, intersection, triangle_norm)) return false;

                return true;
            }
        }

        public class Triangle
        {
            public readonly Vector3 _Point1;
            public readonly Vector3 _Point2;
            public readonly Vector3 _Point3;

            public Triangle(Vector3 pt1, Vector3 pt2, Vector3 pt3)
            {
                this._Point1 = pt1;
                this._Point2 = pt2;
                this._Point3 = pt3;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IntersectsLineSegment(LineSegment l, out Vector3 intersection)
            {
                return LineSegment.IntersectsTriangle(l._Point1, l._Point2, this._Point1, this._Point2, this._Point3, out intersection);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IntersectsTriangle(Vector3 line_pt1, Vector3 line_pt2, Vector3 t1, Vector3 t2, Vector3 t3, out Vector3 intersection)
            {
                return LineSegment.IntersectsTriangle(line_pt1, line_pt2, t1, t2, t3, out intersection);
            }
        }

        public class Cylinder
        {
            public readonly Vector3 _Point1;
            public readonly Vector3 _Point2;
            public readonly float _Radius;

            public Cylinder(Vector3 pt1, Vector3 pt2, float cylinder_radius)
            {
                this._Point1 = pt1;
                this._Point2 = pt2;
                this._Radius = cylinder_radius;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(Vector3 pt)
            {
                return Contains(this._Point1, _Point2, this._Radius, pt);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool Contains(Vector3 cylinder_pt1, Vector3 cylinder_pt2, float cylinder_radius, Vector3 pt)
            { 
                var line_dir = cylinder_pt2 - cylinder_pt1;
                var line_lens = line_dir.Length;
                if (line_lens == 0)
                {
                    return false;
                }

                line_dir /= line_lens;

                var pt_dir = pt - cylinder_pt1;

                var dot = Vector3.Dot(line_dir, pt_dir);

                if (dot < 0)
                {
                    return false;
                }
                else if (dot > line_lens)
                {
                    return false;
                }
                else
                {
                    return (pt_dir - dot * line_dir).Length <= cylinder_radius;
                }
            }
        }
    }
}
