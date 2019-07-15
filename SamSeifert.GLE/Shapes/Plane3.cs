using OpenTK;
using SamSeifert.GLE.Extensions;
using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Shapes
{
    public class Plane3
    {
        public readonly Vector3 Point;
        public readonly Vector3 Normal;  // Radius

        public Plane3(Vector3 point, Vector3 normal, bool normalize = true)
        {
            this.Point = point;        
            this.Normal = normalize ? normal.Normalized() : normal;
        }

        public Plane3As2DCoordinateSystem asCoordinateSystem()
        {
            return new Plane3As2DCoordinateSystem(this.Point, this.Normal, false);
        }

        public class Plane3As2DCoordinateSystem : Plane3
        {
            public readonly Vector3 AxisX;
            public readonly Vector3 AxisY;

            public Plane3As2DCoordinateSystem(
                Vector3 p,
                Vector3 n, 
                bool normalize = true
                ) : base(p, n, normalize)
            {
                this.AxisX = this.Normal.Perpindicular();
                this.AxisY = Vector3.Cross(this.Normal, this.AxisX);
            }

            public Vector2 Project(Vector3 vec)
            {
                var diff = vec - this.Point;

                return new Vector2(
                    Vector3.Dot(this.AxisX, diff),
                    Vector3.Dot(this.AxisY, diff)
                    );
            }

            public Vector2[] Project(params Vector3[] vecs)
            {
                var result = new Vector2[vecs.Length];
                result.Fill(i => this.Project(vecs[i]));
                return result;
            }

            public Vector3 Unproject(Vector2 vec)
            {
                return this.Point + this.AxisX * vec.X + this.AxisY * vec.Y;
            }
        }
    }
}
