using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;
using SamSeifert.Utilities.Maths;
using SamSeifert.Utilities.Extensions;
using SamSeifert.GLE.Extensions;
using SamSeifert.Utilities;

namespace SamSeifert.GLE.CAD
{
    public class CadObject : DeleteableObject
    {
        public bool _BoolDisplay = true;
        public CadObject[] _Children = new CadObject[0];

        public String _Name = "Untitled";
        public Matrix4 _Matrix = Matrix4.Identity;
        public ColorGL _Color = null;
        public Vector3[] _Vertices;
        public Vector3[] _Normals;
        public uint[] _Indices;

        private bool _IsSetup = false;
        private int _IntInterleaveBufferID; // Used to support GL4
        private int _IntIndicesBufferID; // Used to support GL4

        private bool _BoundingSphereNeeded = true;
        private Vector3 _BoundingSphereCenter = Vector3.Zero;
        private float _BoundingSphereRadius = 0;

        internal Action AnonymousDraw = null;
        
        /// <summary>
        /// Null = don't cull face
        /// </summary>
        public CullFaceMode? _CullFaceMode = null;

        public CadObject(CadObject[] cos, String name = "Group")
        {
            if (cos != null) this._Children = cos;
            this._Name = name;
        }

        public CadObject(Vector3[] verts, Vector3[] norms, String name)
        {
            this._Name = name;
            this.InitializeWithVectorsAndNormals(verts, norms);
        }

        internal CadObject()
        {
        }

        public override string ToString()
        {
            return this._Name;
        }





        public void SetMatrix(Matrix4 m)
        {
            this._Matrix = m;
            this._BoundingSphereNeeded = true;
        }

        public CadObject Transform(Matrix4 m)
        {
            return this.Transform(ref m);
        }

        public CadObject Transform(ref Matrix4 m)
        {
            this._Matrix *= m;
            this._BoundingSphereNeeded = true;
            return this;
        }

        public void SetUseTranslationAndRotation(bool arg)
        {
            this._BoundingSphereNeeded = true;
        }

        public CadObject SetAlpha(float a)
        {
            if (this._Color != null) this._Color.setAlpha(a);
            foreach (var e in this._Children) e.SetAlpha(a);
            return this;
        }

        public CadObject SetColor(ColorGL c)
        {
            this._Color = c;
            return this;
        }

        public void SetBoundingSphere(Vector3 center, float radius)
        {
            this._BoundingSphereNeeded = false;
            this._BoundingSphereCenter = center;
            this._BoundingSphereRadius = radius;
        }

        public void GetBoundingSphere(out Vector3 center, out float radius)
        {
            this.UpdateBoundingSphere();
            center =  (this._Matrix * new Vector4(this._BoundingSphereCenter, 1)).Xyz;
            radius = this._BoundingSphereRadius;
        }

        public IEnumerable<Tuple<CadObject, int>> EnumerateFamilyTree(int depth = 0)
        {
            yield return new Tuple<CadObject, int>(this, depth);
            foreach (var child in this._Children)
                foreach (var co in child.EnumerateFamilyTree(depth + 1))
                    yield return co;
        }

        public IEnumerable<Geometry3D.Triangle> EnumerateFaces()
        {
            for (int i = 0; i < this._Indices.Length; i += 3)
            {
                yield return new Geometry3D.Triangle(
                    this._Vertices[this._Indices[i + 0]],
                    this._Vertices[this._Indices[i + 1]],
                    this._Vertices[this._Indices[i + 2]]);
            }
        }









        public void GLDelete()
        {
            if (this._Children.Length > 0)
            {
                foreach (CadObject co in this._Children) co.GLDelete();
            }
            else
            {
                if (this._IsSetup)
                {
                    this._IsSetup = false;
                    GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
                    GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);
                    this._IntIndicesBufferID = 0;
                    this._IntInterleaveBufferID = 0;
                }
            }
        }















        internal void InitializeWithVectorsAndNormals(Vector3[] v, Vector3[] n)
        {
            uint count;
            var consolidated = Helpers.ConsolidateData(v, n, out count, out this._Vertices, out this._Normals, out this._Indices);
            Assert.IsTrue(consolidated);
            this.GLDelete();
            this._Vertices.Length.AssertEquals(this._Normals.Length);
        }

        internal void InitializeWithVectorsAndNormalsSorted(Vector3[] v, Vector3[] n, uint[] ii)
        {
            this._Vertices = v;
            this._Normals = n;
            this._Indices = ii;
            this.GLDelete();
            this._Vertices.Length.AssertEquals(this._Normals.Length);
        }





        /// <summary>
        /// Checks The Viewport to see if we're inside.
        /// Requires using SamSeifert.GLE.GLR everywhere
        /// </summary>
        /// <param name="useColor"></param>
        public static bool RenderOnlyOnscreenObjects = false;

        public void Draw(bool useColor = true)
        {
            if (this._BoolDisplay)
            {
                GL.PushMatrix();
                GL.MultMatrix(ref this._Matrix);
                this.Draw2(useColor);
                GL.PopMatrix();
            }
        }

        private void Draw2(bool useColor)
        {
            bool draw = true;

            try
            {
                if (!CadObject.RenderOnlyOnscreenObjects) return;

                var current_camera = GL._Camera;
                if (current_camera == null) return;

                draw = false;

                this.UpdateBoundingSphere();

                if (this._BoundingSphereRadius == 0) return;

                var current_model_view_matrix = GL.getMatrix(MatrixMode.Modelview);
                var pos_camera_frame = (new Vector4(this._BoundingSphereCenter, 1) * current_model_view_matrix).Xyz;
                
                Vector3 valid_region_center;
                Vector3[][] valid_region_faces;

                current_camera.GetVisiblePolygonMesh(out valid_region_faces, out valid_region_center);

                { // Check if center of sphere is within valid_region_faces;
                    int same_side_count = 0;

                    foreach (var face in valid_region_faces)
                    {
                        var pivot_point = face[1];
                        var cross = Vector3.Cross(face[0] - pivot_point, face[2] - pivot_point);

                        // true if both negative, both positive, or if either is 0
                        // valid_region_center can't be zero
                        bool same_side = 0 <=
                            Vector3.Dot(cross, pos_camera_frame - pivot_point) *
                            Vector3.Dot(cross, valid_region_center - pivot_point);

                        if (same_side) same_side_count++;
                        else break;
                    }

                    if (same_side_count == valid_region_faces.Length)
                    {
                        draw = true;
                        return;
                    }
                }

                // Check if any side intersects sphere
                foreach (var face in valid_region_faces)
                    if (Geometry3D.Sphere.IntersectsPolygonConvex(pos_camera_frame, this._BoundingSphereRadius, face))
                    {
                        draw = true;
                        return;
                    }
            }
            finally
            {
                if (draw) this.Draw3(useColor);

                /*
                GL.PointSize(5);
                GL.Disable(EnableCap.Lighting);
                GL.DepthOff();
                GL.Begin(PrimitiveType.Points);
                GL.Color3(1, 0, 0);
                GL.Vertex3(this._BoundingSphereCenter);
                GL.End();
                GL.DepthOn();
                GL.Enable(EnableCap.Lighting);
                */
            }
        }


        private void Draw3(bool useColor)
        {
            if (this._Children.Length > 0)
            {
                foreach (CadObject co in this._Children)
                {
                    co.Draw(useColor);
                }
            }
            else
            {
                CullFaceModeE.sendToGL(this._CullFaceMode);
                if (this.AnonymousDraw == null)
                {
                    if (useColor)
                    {
                        if (this._Color == null)
                        {
                            new ColorGL(System.Drawing.Color.LimeGreen).sendToGL();
                            GL.Disable(EnableCap.Blend);
                        }
                        else if (this._Color.maxAlpha() < 0.001f)
                        {
                            return;
                        }
                        else if (this._Color.minAlpha() > 0.999f)
                        {
                            GL.Disable(EnableCap.Blend);
                            this._Color.sendToGL();
                        }
                        else
                        {
                            GL.Enable(EnableCap.Blend);
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                            this._Color.sendToGL();
                        }
                    }

                    if (this._IsSetup) this.DrawGL4();
                    else if (this.SetupGL()) this._IsSetup = true;
                }
                else AnonymousDraw();
            }
        }








        private void UpdateBoundingSphere()
        {
            if (this._BoundingSphereNeeded)
            {
                this._BoundingSphereNeeded = false;

                if (this._Children.Length > 0) // Have to combine children bounding spheres into one giant bounding sphere
                {
                    // Bitter's Algorithm
                    var spheres = new List<Tuple<Vector3, float>>();

                    // Get spheres for all children!
                    foreach (var child in this._Children)
                    {
                        child.UpdateBoundingSphere();
                        spheres.Add(new Tuple<Vector3, float>(child._BoundingSphereCenter,
                                                              child._BoundingSphereRadius));
                    }

                    // Find Sphere Farthest from Origin
                    float furthest_distance = 0;
                    foreach (var sphere in spheres)
                    {
                        float dist = sphere.Item1.Length + sphere.Item2;

                        if (dist > furthest_distance)
                        {
                            furthest_distance = dist;
                            this._BoundingSphereCenter = sphere.Item1;
                            this._BoundingSphereRadius = sphere.Item2;
                        }
                    }

                    // Find farthest spheres not enclosed by bigger sphere
                    while (true)
                    {
                        furthest_distance = 0;
                        var furthest = new Tuple<Vector3, float>(Vector3.Zero, 0);
                        foreach (var sphere in spheres)
                        {
                            float dist = (this._BoundingSphereCenter - sphere.Item1).Length + sphere.Item2;

                            if (dist > furthest_distance)
                            {
                                furthest_distance = dist;
                                furthest = sphere;
                            }
                        }

                        if (furthest_distance < 1.001f * this._BoundingSphereRadius) break; // 1.001 is wiggle room

                        var center_to_farthest = furthest.Item1 - this._BoundingSphereCenter;
                        float new_radius = (center_to_farthest.Length + this._BoundingSphereRadius + furthest.Item2) / 2;
                        center_to_farthest.Normalize();
                        this._BoundingSphereCenter += center_to_farthest * (new_radius - this._BoundingSphereRadius);
                        this._BoundingSphereRadius = new_radius;
                    }
                }
                else
                {
                    // Bitter's Algorithm
                    var furthest_distance = 0f;
                    var bestV = Vector3.Zero;

                    // Find Point Farthest From Origin
                    foreach (var v in this._Vertices)
                    {
                        var length = (this._Vertices[0] - v).LengthSquared;         
                        if (length > furthest_distance)
                        {
                            furthest_distance = length;
                            bestV = v;
                        }
                    }

                    furthest_distance = 0.0f;
                    var circlePoint1 = bestV;

                    // Find Point Farthest From That Point
                    foreach (var v in this._Vertices)
                    {
                        var length = (circlePoint1 - v).LengthSquared;

                        if (length > furthest_distance)
                        {
                            furthest_distance = length;
                            bestV = v;
                        }
                    }

                    // Setup Sphere
                    this._BoundingSphereCenter = (circlePoint1 + bestV) / 2;
                    this._BoundingSphereRadius = (circlePoint1 - bestV).Length / 2;

                    // Find farthest spheres not enclosed by bigger sphere
                    while (true)
                    {
                        furthest_distance = 0;
                        var furthest = Vector3.Zero;
                        foreach (var v in this._Vertices)
                        {
                            float dist = (this._BoundingSphereCenter - v).Length;

                            if (dist > furthest_distance)
                            {
                                furthest_distance = dist;
                                furthest = v;
                            }
                        }

                        if (furthest_distance < 1.001f * this._BoundingSphereRadius) break; // 1.001 is wiggle room

                        // Expand Spherer to Include Furthest
                        var center_to_farthest = furthest - this._BoundingSphereCenter;
                        float new_radius = (center_to_farthest.Length + this._BoundingSphereRadius) / 2;
                        center_to_farthest.Normalize();
                        this._BoundingSphereCenter += center_to_farthest * (new_radius - this._BoundingSphereRadius);
                        this._BoundingSphereRadius = new_radius;
                    }
                }

                this._BoundingSphereRadius *= 1.001f; // Just give a little clearance.
            }
        }



        











        private bool SetupGL()
        {
            var idata = new Vector3[_Vertices.Length * 2];
            for (int i = 0; i < _Vertices.Length; i ++)
            {
                idata[i * 2] = this._Vertices[i];
                idata[i * 2 + 1] = this._Normals[i];
            }

            int bufferSize;
            int bufferSizeE;

            bufferSizeE = idata.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), idata, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (bufferSizeE != bufferSize) return false;
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            bufferSizeE = this._Indices.Length * sizeof(uint);
            GL.GenBuffers(1, out _IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), this._Indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (bufferSizeE != bufferSize) return false;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
            return true;
        }

        private void DrawGL4()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 2, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 2, Vector3.SizeInBytes);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(PrimitiveType.Triangles, this._Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }











        public CadObject Center()
        {
            Vector3 center;
            float size;
            this.GetBoundingSphere(out center, out size);
            this.Transform(Matrix4.CreateTranslation(-center));
            return this;
        }

        public CadObject ConsolidateColors()
        {
            this.GLDelete();

            var all = new List<CadObject>(this._Children);

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

            return new CadObject(all.ToArray(), this._Name);
        }

        public CadObject ConsolidateMatrices()
        {
            this.GLDelete();

            List<CadObject> all_objects = new List<CadObject>();
            this.ConsolidateMatrices(all_objects, Matrix4.Identity);

            for (int i = 0; i < all_objects.Count; i++)
            {
                var linq = all_objects[i]._Vertices;
                if ((linq == null) || (linq.Length == 0))
                {
                    all_objects[i].GLDelete();
                    all_objects.RemoveAt(i--);
                }
                else all_objects[i]._Children = new CadObject[0];
            }

            if (all_objects.Count == 1)
            {
                all_objects[0]._Name = this._Name;
                return all_objects[0];
            }
            else
            {
                return new CadObject(all_objects.ToArray(), this._Name);
            }
        }

        internal void ConsolidateMatrices(List<CadObject> all_objects, Matrix4 m4)
        {
            all_objects.Add(this);
            m4 = this._Matrix * m4;
            this._BoundingSphereNeeded = true;
            this._Matrix = Matrix4.Identity;
            foreach (var child in this._Children)
            {
                child.ConsolidateMatrices(all_objects, m4);
            }
            this.TransformPoints(ref m4);
        }

        internal void TransformPoints(ref Matrix4 m4)
        {
            Vector4 t = Vector4.Zero;
            if (_Vertices != null)
            {
                for (int i = 0; i < _Vertices.Length; i++)
                {
                    t.X = _Vertices[i].X;
                    t.Y = _Vertices[i].Y;
                    t.Z = _Vertices[i].Z;
                    t.W = 1;

                    t = Vector4.Transform(t, m4);

                    _Vertices[i].X = t.X;
                    _Vertices[i].Y = t.Y;
                    _Vertices[i].Z = t.Z;
                }
            }

            if (_Normals != null)
            {
                for (int i = 0; i < _Normals.Length; i++)
                {
                    t.X = _Normals[i].X;
                    t.Y = _Normals[i].Y;
                    t.Z = _Normals[i].Z;
                    t.W = 0;

                    var n = Vector4.Transform(t, m4).Xyz.NormalizedSafe();

                    _Normals[i].X = n.X;
                    _Normals[i].Y = n.Y;
                    _Normals[i].Z = n.Z;
                }
            }
        }
    }
}
