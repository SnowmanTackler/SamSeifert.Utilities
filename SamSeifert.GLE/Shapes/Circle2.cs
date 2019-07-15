using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Shapes
{
    public class Circle2
    {
        public readonly Vector2 Center;   // Center
        public readonly float Radius;  // Radius
    
        public Circle2(Vector2 c, float r)
        {
            this.Center = c;
            this.Radius = r;
        }
        
        public bool Contains(Vector2 p)
        {
            return (Center - p).Length <= Radius * 1.000001f;
        }


        public bool Contains(ICollection<Vector2> ps)
        {
            foreach (Vector2 p in ps)
            {
                if (!Contains(p))
                    return false;
            }
            return true;
        }
    }
}
