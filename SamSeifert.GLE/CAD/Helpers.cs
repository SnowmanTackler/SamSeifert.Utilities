using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace SamSeifert.GLE.CAD
{
    internal class Helpers
    {
        internal static bool ConsolidateData(
            Vector3[] verts, Vector3[] norms,
            out uint count, out Vector3[] vout, out Vector3[] nout, out uint[] iout)
        {
            if (verts.Length != norms.Length)
            {
                vout = null;
                nout = null;
                iout = null;
                count = 0;
                return false;
            }
            else
            {
                var m1a = new List<Vector3>();
                var m2a = new List<Vector3>();
                var m3a = new List<uint>();

                var dict = new Dictionary<Tuple<float, float, float, float, float, float>, uint>();
                uint dex;
                count = 0;

                for (int i = 0; i < verts.Length; i++)
                {
                    var v = verts[i];
                    var n = norms[i];

                    var sort = new Tuple<float, float, float, float, float, float>(
                        v.X,
                        v.Y,
                        v.Z,
                        n.X,
                        n.Y,
                        n.Z
                        );

                    if (!dict.TryGetValue(sort, out dex))
                    {
                        dex = count;
                        m1a.Add(new Vector3(v.X, v.Y, v.Z));
                        m2a.Add(new Vector3(n.X, n.Y, n.Z));
                        dict.Add(sort, dex);
                        count++;
                    }

                    m3a.Add(dex);
                }

                vout = m1a.ToArray();
                nout = m2a.ToArray();
                iout = m3a.ToArray();

                return true;
            }
        }

        internal static bool ExpandData(
            Vector3[] verts, Vector3[] norms, uint[] indices,
            out uint count, out Vector3[] vout, out Vector3[] nout)
        {
            if (verts.Length != norms.Length)
            {
                vout = null;
                nout = null;
                count = 0;
                return false;
            }
            else
            {
                var voutL = new List<Vector3>();
                var noutL = new List<Vector3>();

                count = 0;

                foreach (int dex in indices)
                {
                    voutL.Add(verts[dex]);
                    noutL.Add(norms[dex]);
                    count++;
                }

                vout = voutL.ToArray();
                nout = noutL.ToArray();
                return true;
            }
        }
    }
}
