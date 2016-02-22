using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.Utilities;

using OpenTK;

namespace SamSeifert.GLE.Geometry2D
{
    public class Triangle
    {
        public readonly Vector2 P1;
        public readonly Vector2 P2;
        public readonly Vector2 P3;
        public readonly Vector2 Center;

        public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            this.P1 = v1;
            this.P2 = v2;
            this.P3 = v3;
            this.Center = (v1 + v2 + v3) / 3;
        }

        public IEnumerable<Vector2> Corners
        {
            get
            {
                yield return P1;
                yield return P2;
                yield return P3;
            }
        }

        float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public bool Contains(Vector2 pt)
        {
            bool b1, b2, b3;

            b1 = sign(pt, P1, P2) < 0.0f;
            b2 = sign(pt, P2, P3) < 0.0f;
            b3 = sign(pt, P3, P1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }
    }
}
