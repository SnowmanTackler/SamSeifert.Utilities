using OpenTK;
using SamSeifert.GLE.Extensions;
using SamSeifert.GLE.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Algorithms
{
    public static class EnclosingCircle
    {
        /* 
         * Returns the smallest circle that encloses all the given points. Runs in expected O(n) time, randomized.
         * Note: If 0 points are given, a circle of radius -1 is returned. If 1 point is given, a circle of radius 0 is returned.
         */
        // Initially: No boundary points known
        public static Circle2 MakeCircle(IList<Vector2> points)
        {
            // Clone list to preserve the caller's data, do Durstenfeld shuffle
            List<Vector2> shuffled = new List<Vector2>(points);
            Random rand = new Random();
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                Vector2 temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            // Progressively add points to circle or recompute circle
            Circle2 c = null;
            for (int i = 0; i < shuffled.Count; i++)
            {
                Vector2 p = shuffled[i];
                if (c == null || !c.Contains(p))
                    c = MakeCircleOnePoint(shuffled.GetRange(0, i + 1), p);
            }
            return c;
        }

        // One boundary point known
        private static Circle2 MakeCircleOnePoint(List<Vector2> points, Vector2 p)
        {
            Circle2 c = new Circle2(p, 0);
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 q = points[i];
                if (!c.Contains(q))
                {
                    if (c.Radius == 0)
                        c = MakeDiameter(p, q);
                    else
                        c = MakeCircleTwoPoints(points.GetRange(0, i + 1), p, q);
                }
            }
            return c;
        }


        // Two boundary points known
        private static Circle2 MakeCircleTwoPoints(List<Vector2> points, Vector2 p, Vector2 q)
        {
            Circle2 circ = MakeDiameter(p, q);
            Circle2 left = null;
            Circle2 right = null;

            // For each point not in the two-point circle
            Vector2 pq = q - p;
            foreach (Vector2 r in points)
            {
                if (circ.Contains(r))
                    continue;

                // Form a circumcircle and classify it on left or right side
                float cross = Vector2E.Cross(pq, r - p);
                Circle2 c = MakeCircumcircle(p, q, r);
                if (c == null)
                    continue;
                else if (cross > 0 && (left == null || Vector2E.Cross(pq, c.Center - p) > Vector2E.Cross(pq, left.Center - p)))
                    left = c;
                else if (cross < 0 && (right == null || Vector2E.Cross(pq, c.Center - p) < Vector2E.Cross(pq, right.Center - p)))
                    right = c;
            }

            // Select which circle to return
            if (left == null && right == null)
                return circ;
            else if (left == null)
                return right;
            else if (right == null)
                return left;
            else
                return left.Radius <= right.Radius ? left : right;
        }


        public static Circle2 MakeDiameter(Vector2 a, Vector2 b)
        {
            Vector2 c = new Vector2((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            return new Circle2(c, Math.Max((c - a).Length, (c - b).Length));
        }


        public static Circle2 MakeCircumcircle(Vector2 a, Vector2 b, Vector2 c)
        {
            // Mathematical algorithm from Wikipedia: Circumscribed circle
            float ox = (Math.Min(Math.Min(a.X, b.X), c.X) + Math.Max(Math.Min(a.X, b.X), c.X)) / 2;
            float oy = (Math.Min(Math.Min(a.Y, b.Y), c.Y) + Math.Max(Math.Min(a.Y, b.Y), c.Y)) / 2;
            float ax = a.X - ox, ay = a.Y - oy;
            float bx = b.X - ox, by = b.Y - oy;
            float cx = c.X - ox, cy = c.Y - oy;
            float d = (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) * 2;
            if (d == 0)
                return null;
            float x = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            float y = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            Vector2 p = new Vector2(ox + x, oy + y);
            float r = new Vector2[] { a, b, c }.Select(it => (p - it).Length).Max();
            return new Circle2(p, r);
        }

    }



}
