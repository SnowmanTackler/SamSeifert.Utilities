using OpenTK;
using SamSeifert.Utilities.Files;
using SamSeifert.Utilities.Files.Json;
using SamSeifert.Utilities.Files.Xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.CAD.Generator
{
    public static partial class FromXaml
    {
        public static CadObject Create(
            String FileText,
            String ObjectName = "",
            float xScale = 1.0f,
            float yScale = 1.0f,
            float zScale = 1.0f,
            float xOff = 0.0f,
            float yOff = 0.0f,
            float zOff = 0.0f,
            bool useAmbient = true,
            bool useDiffuse = true,
            bool useSpecular = true,
            bool useEmission = true,
            bool reduceComplexity = true,
            bool reverseFace = false
            )
        {
            var f = TagFile.ParseText(FileText);
            String match0 = "ModelVisual3D";
            var match0L = f.getGapMatches(match0).ToList();

            TagFile parent = null;

            switch (match0L.Count)
            {
                case 1:
                    parent = match0L[0];
                    break;
                default:
                    Console.WriteLine("CadObjectGenerator: Invalid XAML File");
                    return new CadObject();
            }


            var co = SamSeifert.GLE.CAD.Generator.FromXaml.xamlModelVisual3D(
                parent,
                xScale,
                yScale,
                zScale,
                useAmbient,
                useDiffuse,
                useSpecular,
                useEmission,
                reverseFace
                );

            co.SetUseTranslationAndRotation(true, false);
            co.Transform(Matrix4.CreateTranslation(xOff, yOff, zOff));

            if (!reduceComplexity)
            {
                co._Name = ObjectName;
                return co;
            }

            co.GLDelete();

            // TAKE OUT TREE STRUCTURE AND MAKE SINGLE LEVEL
            var all = co.ConsolidateMatrices();
            int old = all.Count;

            for (int i = 0; i < all.Count; i++)
            {
                var linq = all[i]._Vertices;
                if ((linq == null) || (linq.Length == 0))
                {
                    all[i].GLDelete();
                    all.RemoveAt(i--);
                }
                else all[i]._Children = new CadObject[0];
            }

            // Console.WriteLine("Removing Tree: " + old + " to " + all.Count);
            old = all.Count;

            // COMBINE COLORS
            for (int i = 0; i < all.Count; i++)
            {
                ColorGL col = all[i]._Color;

                var verts = new List<Vector3>();
                var norms = new List<Vector3>();
                var dices = new List<uint>();

                for (int j = i; j < all.Count; j++)
                {
                    if (ColorGL.CheckMatch(all[i]._Color, all[j]._Color))
                    {
                        for (int dex = 0; dex < all[j]._Indices.Length; dex++)
                            all[j]._Indices[dex] += (uint)verts.Count; // CHANGE INDICES

                        verts.AddRange(all[j]._Vertices);
                        norms.AddRange(all[j]._Normals);
                        dices.AddRange(all[j]._Indices);

                        if (i != j) all.RemoveAt(j--); // dont delete the first one!
                    }
                }

                all[i].InitializeWithVectorsAndNormalsSorted(verts.ToArray(), norms.ToArray(), dices.ToArray());
            }
            // Console.WriteLine("Combine Colors: " + old + " to " + all.Count);

            var ret = new CadObject(all.ToArray(), ObjectName);
            return ret;
        }

        private static CadObject xamlModelVisual3D(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            bool reverseFace
            )
        {
            CadObject co = new CadObject();
            var ret = new List<CadObject>();

            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;
                    switch (f1._Name)
                    {
                        case "ModelVisual3D.Transform":
                            {
                                xamlParseTransform(f1, xScale, yScale, zScale, co);
                                break;
                            }
                        case "ModelVisual3D.Content":
                            {
                                xamlModelVisual3DContent(f1, xScale, yScale, zScale, useAmbient, useDiffuse, useSpecular, useEmission, reverseFace, ret);
                                break;
                            }
                        case "ModelVisual3D.Children":
                            {
                                foreach (var e2 in f1._Children)
                                {
                                    if (e2 is TagFile)
                                    {
                                        var f2 = e2 as TagFile;
                                        switch (f2._Name)
                                        {
                                            case "ModelVisual3D":
                                                ret.Add(xamlModelVisual3D(f2, xScale, yScale, zScale, useAmbient, useDiffuse, useSpecular, useEmission, reverseFace));
                                                break;
                                            
                                            default:
                                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f2._Name);
                                                break;
                                        }
                                    }
                                }
                                break;
                            }
                        default:
                            {
#if DEBUG
                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f1._Name);
#endif
                                break;
                            }
                    }
                }
            }

            for (int i = 0; i < ret.Count; i++)
            {
                if (ret[i]._Children != null)
                    if (ret[i]._Children.Length != 0)
                        continue;

                if (ret[i]._Vertices != null)
                    if (ret[i]._Vertices.Length != 0)
                        continue;

                ret.RemoveAt(i);
                i--;
            }

            co._Children = ret.ToArray();

            return co;
        }

        public static void xamlModelVisual3DContent(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            bool reverseFace,
            List<CadObject> ls
            )
        {
            var u1 = new HashSet<string>();
            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;
                    if (u1.Contains(f1._Name)) Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " multiple " + f1._Name);
                    else
                    {
                        u1.Add(f1._Name);
                        switch (f1._Name)
                        {
                            case "Model3DGroup":
                                xamlModel3DGroup(f1, xScale, yScale, zScale, useAmbient, useDiffuse, useSpecular, useEmission, reverseFace, ls);
                                break;
                            default:
#if DEBUG
                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f1._Name);
#endif
                                break;
                        }
                    }
                }
            }
        }

        public static void xamlModel3DGroup(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            bool reverseFace,
            List<CadObject> ls)
        {
            var u1 = new HashSet<string>();
            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;
                    if (u1.Contains(f1._Name)) Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " multiple " + f1._Name);
                    else
                    {
                        u1.Add(f1._Name);
                        switch (f1._Name)
                        {
                            case "Model3DGroup.Children":
                                xamlModelVisual3DGroupChildren(f1, xScale, yScale, zScale, useAmbient, useDiffuse, useSpecular, useEmission, reverseFace, ls);
                                break;
                            default:
#if DEBUG
                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f1._Name);
#endif
                                break;
                        }
                    }
                }
            }
        }

        public static void xamlModelVisual3DGroupChildren(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            bool reverseFace,
            List<CadObject> ls)
        {
            // var u3 = new HashSet<string>();
            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;
                    // if (u3.Contains(f3._Name)) Console.WriteLine("CadObjectGenerator: Model3DGroup.Children multiple " + f3._Name);
                    // else
                    // {
                    // u3.Add(f3._Name);
                    switch (f1._Name)
                    {
                        case "GeometryModel3D":
                            ls.Add(xamlModelGeometryModel3D(f1, xScale, yScale, zScale, useAmbient, useDiffuse, useSpecular, useEmission, reverseFace));
                            break;
                        case "AmbientLight":
                            // f1.display();
                            break;
                        case "DirectionalLight":
                            // f1.display();
                            break;
                        default:
#if DEBUG
                            Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f1._Name);
#endif
                            break;
                    }
                    // }
                }
            }
        }

        public static CadObject xamlModelGeometryModel3D(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            bool reverseFace)
        {
            var co = new CadObject();

            var u1 = new HashSet<string>();

            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;

                    if (u1.Contains(f1._Name)) Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " multiple " + f1._Name);
                    else
                    {
                        u1.Add(f1._Name);
                        switch (f1._Name)
                        {
                            case "GeometryModel3D.Material":
                                {
                                    xamlParseColor(f1, useAmbient, useDiffuse, useSpecular, useEmission, co);
                                    break;
                                }
                            case "GeometryModel3D.Geometry":
                                var u2 = new HashSet<string>();
                                foreach (var e2 in f1._Children)
                                {
                                    if (e2 is TagFile)
                                    {
                                        var f2 = e2 as TagFile;
                                        if (u2.Contains(f2._Name)) Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " multiple " + f2._Name);
                                        else
                                        {
                                            switch (f2._Name)
                                            {
                                                case "MeshGeometry3D":
                                                    {
                                                        var data = f2._Params;
                                                        var verts = new List<Vector3>();
                                                        var norms = new List<Vector3>();
                                                        xamlParseDict(ref data, ref verts, ref norms, reverseFace);
                                                        var verts2array = verts.ToArray();
                                                        var norms2array = norms.ToArray();
                                                        for (int j = 0; j < verts.Count; j++)
                                                        {
                                                            verts2array[j].X *= xScale;
                                                            verts2array[j].Y *= yScale;
                                                            verts2array[j].Z *= zScale;
                                                        }
                                                        co.InitializeWithVectorsAndNormals(verts2array, norms2array);
                                                        break;
                                                    }
                                                default:
                                                    Console.WriteLine("CadObjectGenerator: GeometryModel3D.Geometry ignoring " + f2._Name);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
#if DEBUG
                                Console.WriteLine("CadObjectGenerator: GeometryModel3D ignoring " + f1._Name);
#endif
                                break;
                        }
                    }
                }
            }

            return co;
        }

        private static void xamlParseTransform(
            TagFile f0,
            float xScale,
            float yScale,
            float zScale,
            CadObject co)
        {
            var u1 = new HashSet<string>();
            foreach (var e1 in f0._Children)
            {
                if (e1 is TagFile)
                {
                    var f1 = e1 as TagFile;
                    if (u1.Contains(f1._Name)) Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " multiple " + f1._Name);
                    else
                    {
                        u1.Add(f1._Name);
                        switch (f1._Name)
                        {
                            case "MatrixTransform3D":
                                {
                                    foreach (var kv in f1._Params.AsEnumerable())
                                    {
                                        switch (kv.Key)
                                        {
                                            case "Matrix":
                                                {
                                                    var vals = kv.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                                    var points = new List<double>();
                                                    double a;
                                                    foreach (var line in vals)
                                                        if (double.TryParse(line, out a))
                                                            points.Add(a);

                                                    if (points.Count == 16)
                                                    {
                                                        int i = 0;
                                                        co.SetUseTranslationAndRotation(true, false);
                                                        co._Matrix.M11 = (float)points[i++];
                                                        co._Matrix.M12 = (float)points[i++];
                                                        co._Matrix.M13 = (float)points[i++];
                                                        co._Matrix.M14 = (float)points[i++];

                                                        co._Matrix.M21 = (float)points[i++];
                                                        co._Matrix.M22 = (float)points[i++];
                                                        co._Matrix.M23 = (float)points[i++];
                                                        co._Matrix.M24 = (float)points[i++];

                                                        co._Matrix.M31 = (float)points[i++];
                                                        co._Matrix.M32 = (float)points[i++];
                                                        co._Matrix.M33 = (float)points[i++];
                                                        co._Matrix.M34 = (float)points[i++];

                                                        co._Matrix.M41 = (float)points[i++];
                                                        co._Matrix.M42 = (float)points[i++];
                                                        co._Matrix.M43 = (float)points[i++];
                                                        co._Matrix.M44 = (float)points[i++];

                                                        co._Matrix.M41 *= xScale;
                                                        co._Matrix.M42 *= yScale;
                                                        co._Matrix.M43 *= zScale;
                                                    }
#if DEBUG
                                                    else Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " matrix doesn't have 16 values!");
#endif
                                                    break;
                                                }
                                            default:
#if DEBUG
                                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + ", MatrixTransform3D ignoring " + kv);
#endif

                                                break;
                                        }
                                    }
                                    break;
                                }
                            case "RotateTransform3D":
                                {
                                    bool no_angle = false;

                                    if (f1._Children.Length == 1)
                                    {
                                        var f2 = f1._Children[0] as TagFile;
                                        if (f2 != null)
                                        {
                                            if ((f2._Children.Length == 1) && ("RotateTransform3D.Rotation".Equals(f2._Name)))
                                            {
                                                var f3 = f2._Children[0] as TagFile;
                                                if (f3 != null)
                                                {
                                                    if ("AxisAngleRotation3D".Equals(f3._Name))
                                                    {
                                                        string angle;
                                                        if (f3._Params.TryGetValue("Angle", out angle))
                                                        {
                                                            if ("0".Equals(angle))
                                                            {
                                                                no_angle = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (!no_angle) f1.Display();
                                    break;
                                }
                            default:
#if DEBUG
                                Console.WriteLine("CadObjectGenerator." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ignoring " + f1._Name);
#endif
                                break;
                        }
                    }
                }
            }
        }

        private static void xamlParseColor(
            TagFile f0,
            bool useAmbient,
            bool useDiffuse,
            bool useSpecular,
            bool useEmission,
            CadObject co)
        {
            //            f0.display();

            String output;
            const String SCOLOR = "Color";
            const String SOPAC = "Opacity";

            String matchD = "DiffuseMaterial";
            String matchS = "SpecularMaterial";
            String matchA = "AmbientMaterial";
            String matchE = "EmissionMaterial";
            String matchX = "SolidColorBrush";

            var matchD_ = f0.getGapMatches(matchD, matchX).ToList();
            var matchS_ = f0.getGapMatches(matchS, matchX).ToList();
            var matchA_ = f0.getGapMatches(matchA, matchX).ToList();
            var matchE_ = f0.getGapMatches(matchE, matchX).ToList();

            Color diffuse = Color.Black;
            Color specular = Color.Black;
            Color ambient = Color.Black;
            Color emission = Color.Black;

            if (matchD_.Count == 1 && useDiffuse)
            {
                var dict = matchD_[0]._Params;
                if (dict.TryGetValue(SCOLOR, out output))
                {
                    Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                    diffuse = System.Drawing.Color.FromArgb(iColorInt);
                }
                if (dict.TryGetValue(SOPAC, out output))
                {
                    Double a;
                    if (Double.TryParse(output, out a))
                    {
                        Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                        diffuse = Color.FromArgb(alpha, diffuse);
                    }
                }
            }

            if (matchS_.Count == 1 && useSpecular)
            {
                var dict = matchS_[0]._Params;
                if (dict.TryGetValue(SCOLOR, out output))
                {
                    Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                    specular = System.Drawing.Color.FromArgb(iColorInt);
                }
                if (dict.TryGetValue(SOPAC, out output))
                {
                    Double a;
                    if (Double.TryParse(output, out a))
                    {
                        Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                        specular = Color.FromArgb(alpha, specular);
                    }
                }
            }

            if (matchA_.Count == 1 && useAmbient)
            {
                var dict = matchA_[0]._Params;
                if (dict.TryGetValue(SCOLOR, out output))
                {
                    Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                    ambient = System.Drawing.Color.FromArgb(iColorInt);
                }
                if (dict.TryGetValue(SOPAC, out output))
                {
                    Double a;
                    if (Double.TryParse(output, out a))
                    {
                        Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                        ambient = Color.FromArgb(alpha, ambient);
                    }
                }
            }
            else if (useAmbient) ambient = specular;

            if (matchE_.Count == 1 && useEmission)
            {
                var dict = matchE_[0]._Params;
                if (dict.TryGetValue(SCOLOR, out output))
                {
                    Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                    emission = System.Drawing.Color.FromArgb(iColorInt);
                }
                if (dict.TryGetValue(SOPAC, out output))
                {
                    Double a;
                    if (Double.TryParse(output, out a))
                    {
                        Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                        emission = Color.FromArgb(alpha, emission);
                    }
                }
            }

            co._Color = new ColorGL();
            co._Color._Diffuse[0] = diffuse.R / 255.0f;
            co._Color._Diffuse[1] = diffuse.G / 255.0f;
            co._Color._Diffuse[2] = diffuse.B / 255.0f;
            co._Color._Diffuse[3] = diffuse.A / 255.0f;

            co._Color._Ambient[0] = ambient.R / 255.0f;
            co._Color._Ambient[1] = ambient.G / 255.0f;
            co._Color._Ambient[2] = ambient.B / 255.0f;
            co._Color._Ambient[3] = ambient.A / 255.0f;

            co._Color._Specular[0] = specular.R / 255.0f;
            co._Color._Specular[1] = specular.G / 255.0f;
            co._Color._Specular[2] = specular.B / 255.0f;
            co._Color._Specular[3] = specular.A / 255.0f;

            co._Color._Emission[0] = emission.R / 255.0f;
            co._Color._Emission[1] = emission.G / 255.0f;
            co._Color._Emission[2] = emission.B / 255.0f;
            co._Color._Emission[3] = emission.A / 255.0f;
        }

        private static void xamlParseDict(ref Dictionary<String, String> dict, ref List<Vector3> verts, ref List<Vector3> norms, bool reverseFace)
        {
            String nos, ves, ins;

            const String m1 = "Normals";
            const String m2 = "Positions";
            const String m3 = "TriangleIndices";

            if (dict.TryGetValue(m1, out nos) &&
                dict.TryGetValue(m2, out ves) &&
                dict.TryGetValue(m3, out ins))
            {
                var v = xamlParseVector3(ves);
                var n = xamlParseVector3(nos);

                if (v.Count == n.Count)
                {
                    var i = xamlParseVertices(ins);

                    foreach (var trn in i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int aj = reverseFace ? (2 - j) : j;
                            verts.Add(v[trn[aj]]);
                            norms.Add(n[trn[aj]]);
                        }
                    }
                }
            }
        }

        static List<Vector3> xamlParseVector3(String input)
        {
            var verts = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var points = new List<Vector3>();

            foreach (var vline in verts)
            {
                var vxs = vline.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (vxs.Length == 3)
                {
                    double a, b, c;
                    Double.TryParse(vxs[0], out a);
                    Double.TryParse(vxs[1], out b);
                    Double.TryParse(vxs[2], out c);
                    points.Add(new Vector3((float)a, (float)b, (float)c));
                }
            }

            return points;
        }

        static List<int[]> xamlParseVertices(String input)
        {
            var verts = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var points = new List<int[]>();

            foreach (var vline in verts)
            {
                var vxs = vline.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (vxs.Length == 3)
                {
                    int a, b, c;
                    int.TryParse(vxs[0], out a);
                    int.TryParse(vxs[1], out b);
                    int.TryParse(vxs[2], out c);
                    points.Add(new int[] { a, b, c });
                }
            }

            return points;
        }
    }
}
