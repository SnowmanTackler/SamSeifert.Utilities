using OpenTK;
using SamSeifert.Utilities;
using SamSeifert.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.CAD.Generator
{
    public static class FromStl
    {
        private static readonly char[] WhiteSpace = new char[] { ' ', '\t' };

        public static CadObject Create(String FileText, double scale = 1.0)
        {
            if (FileText.StartsWith("solid"))
            {
                return CreateAscii(FileText, scale);
            }
            else
            {
                Logger.Default.Warn("Binary STL");
                return null;
            }
        }

        private static CadObject CreateAscii(String FileText, double scale = 1.0)
        {
            var t = ReadLines(FileText);

            if (!t.MoveNext())
            {
                return null;
            }

            return CreateAscii(t, scale);
        }

        private class NormalVertex
        {
            public readonly Vector3 Normal;
            public readonly Vector3 Vertex;

            public NormalVertex(Vector3 normal, Vector3 vertex)            
            {
                this.Normal = normal;
                this.Vertex = vertex;
            }
        }

        public static CadObject CreateAscii(IEnumerator<String> FileLines, double scale = 1.0)
        {
            String name = null;

            try
            {
                List<NormalVertex> temp = CreateAsciiSolid(FileLines, out name, scale);

                if (temp == null)
                {
                    return null;
                }

                var co = new CadObject(
                    temp.Select((it) => it.Vertex).ToArray(),
                    temp.Select((it) => it.Normal).ToArray(),
                    name);

                co._CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode.Back;
                        
                return co;
            }
            catch (Exception exc)
            {
                Logger.Default.Warn("Parser Error", exc);
                return null;
            }
        }

        private static List<NormalVertex> CreateAsciiSolid(IEnumerator<String> FileLines, out String name, double scale)
        {
            var start = FileLines.Current;
            var parts = start.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if ((parts.Length != 2) || (parts[0] != "solid") ) {
                name = null;
                return null;
            }

            FileLines.MoveNext();

            name = parts[1];

            var ret = new List<NormalVertex>();

            List<NormalVertex> temp;
            while ((temp = CreateAsciiFacet(FileLines, scale)) != null)
            {
                ret.AddRange(temp);
            }

            parts = FileLines.Current.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if ((parts.Length != 2) || (parts[0] != "endsolid")) throw new Exception("Solid not ending: " + FileLines.Current);
            if (parts[1] != name) throw new Exception("Wrong solid ending: " + name + " " + parts[1]);

            FileLines.MoveNext();
            return ret;
        }

        private static List<NormalVertex> CreateAsciiFacet(IEnumerator<String> FileLines, double scale)
        {
            var start = FileLines.Current;
            var parts = start.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 5) return null;
            else if (parts[0] != "facet") return null;
            else if (parts[1] != "normal") return null;

            FileLines.MoveNext();

            double n1, n2, n3;
            if (!Double.TryParse(parts[2], out n1) ||
                !Double.TryParse(parts[3], out n2) ||
                !Double.TryParse(parts[4], out n3))
                throw new Exception("Malformed normal: " + start);

            var normal = new Vector3((float)n1, (float)n2, (float)n3);
            var ret = new List<NormalVertex>();

            List<Vector3> temp;

            while ((temp = CreateAsciiLoop(FileLines, scale)) != null)
            {
                ret.AddRange(temp.Select((vertex) => new NormalVertex(normal, vertex)));
            }

            parts = FileLines.Current.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if ((parts.Length != 1) || (parts[0] != "endfacet")) throw new Exception("Facet not ending: " + FileLines.Current);

            FileLines.MoveNext();
            return ret;
        }

        private static List<Vector3> CreateAsciiLoop(IEnumerator<String> FileLines, double scale)
        {
            var parts = FileLines.Current.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2) return null;
            else if (parts[0] != "outer") return null;
            else if (parts[1] != "loop") return null;

            FileLines.MoveNext();

            var ls = new List<Vector3>();
            Vector3? temp;        
            while ((temp = CreateAsciiVertex(FileLines, scale)).HasValue)
            {
                ls.Add(temp.Value);
            }

            parts = FileLines.Current.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if ((parts.Length != 1) || (parts[0] != "endloop")) throw new Exception("Loop not ending: " + FileLines.Current);
            if (ls.Count != 3) throw new Exception("Wrong number of vertices:" + ls.Count);

            FileLines.MoveNext();
            return ls;
        }

        private static Vector3? CreateAsciiVertex(IEnumerator<String> FileLines, double scale)
        {
            var start = FileLines.Current;
            var parts = start.Split(WhiteSpace, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 4) return null;
            else if (parts[0] != "vertex") return null;

            double v1, v2, v3;

            if (!Double.TryParse(parts[1], out v1) ||
                !Double.TryParse(parts[2], out v2) ||
                !Double.TryParse(parts[3], out v3))
                throw new Exception("Malformed vertex: " + start);

            FileLines.MoveNext();
            return new Vector3((float)(v1 * scale), (float)(v2 * scale), (float)(v3 * scale));
        }

        private static IEnumerator<String> ReadLines(String FileText)
        {
            var sb = new StringBuilder();
            foreach (var c in FileText)
            {
                switch (c)
                {
                    case '\n':
                    case '\r':
                        if (sb.Length != 0)
                        {
                            yield return sb.ToString();
                            sb.Clear();
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

        }
    }
}
