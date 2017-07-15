using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.CAD.Generator
{
    public class Anonymous
    {
        public static CadObject Create(Action a, Vector3[] vertices)
        {
            var co = new CadObject();
            co._Vertices = vertices;
            co.AnonymousDraw = a;
            return co;
        }
    }
}
