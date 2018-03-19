using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL; using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE.CAD
{
    public class CadObject : DeleteableObject
    {
        public bool _BoolDisplay = true;
        public CadObject[] _Children = new CadObject[0];


        internal String _Name = "Untitled";
        internal Matrix4 _Matrix = Matrix4.Identity;
        internal ColorGL _Color = null;
        internal Vector3[] _Vertices;
        internal Vector3[] _Normals;
        internal uint[] _Indices;

        private bool _BoolUseTranslationAndRotation = false;
        private bool _BoolSetupGL3 = false;
        private bool _BoolSetupGL4 = false;
        private int _IntList; // Used to support GL3
        private int _IntInterleaveBufferID; // Used to support GL4
        private int _IntIndicesBufferID; // Used to support GL4

        internal enum GLType { GL3, GL4, UNK };
        internal GLType _GLType = GLType.UNK;

        private bool _BoundingSphereNeeded = true;
        private Vector3 _BoundingSphereCenter = Vector3.Zero;
        private float _BoundingSphereRadius = 0;

        internal Action AnonymousDraw = null;

        public CadObject(CadObject[] cos, String name = "Group")
        {
            if (cos != null) this._Children = cos;
            this._Name = name;
        }

        internal CadObject(Vector3[] verts, Vector3[] norms, String name)
        {
            this._Name = name;
            this.InitializeWithVectorsAndNormals(verts, norms);
        }

        internal CadObject()
        {
        }

        public override string ToString()
        {
            return this._Name + " " + this._GLType;
        }





        public void SetMatrix(Matrix4 m)
        {
            this._Matrix = m;
            this._BoolUseTranslationAndRotation = true;
        }

        public void Transform(Matrix4 m)
        {
            this.Transform(ref m);
        }

        public void Transform(ref Matrix4 m)
        {
            this._Matrix *= m;
            this._BoolUseTranslationAndRotation = true;
        }

        public void SetUseTranslationAndRotation(bool arg, bool recursive = true)
        {
            this._BoolUseTranslationAndRotation = arg;
            if (recursive) foreach (var e in this._Children) e.SetUseTranslationAndRotation(arg);
        }

        public void SetAlpha(float a)
        {
            if (this._Color != null) this._Color.setAlpha(a);
            foreach (var e in this._Children) e.SetAlpha(a);
        }

        public void SetColor(ColorGL c, bool recursive = true)
        {
            this._Color = c;
            if (recursive) foreach (var e in this._Children) e.SetColor(c, recursive);
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









        public void GLDelete()
        {
            if (this._Children.Length > 0)
            {
                foreach (CadObject co in this._Children) co.GLDelete();
            }
            else
            {
                if (this._BoolSetupGL3)
                {
                    this._BoolSetupGL3 = false;
                    GL.DeleteLists(this._IntList, 1);
                }

                if (this._BoolSetupGL4)
                {
                    this._BoolSetupGL4 = false;
                    GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
                    GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);
                    this._IntIndicesBufferID = 0;
                    this._IntInterleaveBufferID = 0;
                }
            }
        }

        public void Draw(bool useColor = true)
        {
            if (this._BoolDisplay)
            {
                if (this._BoolUseTranslationAndRotation)
                {
                    GL.PushMatrix();
                    GL.MultMatrix(ref this._Matrix);
                    this.Draw2(useColor);
                    GL.PopMatrix();
                }
                else this.Draw2(useColor);
            }
        }














        internal void InitializeWithVectorsAndNormals(Vector3[] v, Vector3[] n)
        {
            uint count;
            if (Helpers.ConsolidateData(v, n, out count, out this._Vertices, out this._Normals, out this._Indices)) this.Initialize();
            else this._GLType = GLType.UNK;
        }

        internal void InitializeWithVectorsAndNormalsSorted(Vector3[] v, Vector3[] n, uint[] ii)
        {
            this._Vertices = v;
            this._Normals = n;
            this._Indices = ii;

            this.Initialize();
        }

        internal void Initialize()
        {
            this.GLDelete();

            if (this._Vertices.Length != this._Normals.Length)
            {
                Console.WriteLine("CadObject." + System.Reflection.MethodBase.GetCurrentMethod().Name + " vertices and normals mismatch");
                this._GLType = GLType.UNK;
                return;
            }

            this._BoolSetupGL4 = this.SetupGL4();
            if (this._BoolSetupGL4) this._GLType = GLType.GL4;
            else
            {
                this._BoolSetupGL3 = this.SetupGL3();
                if (this._BoolSetupGL3) this._GLType = GLType.GL3;
                else this._GLType = GLType.UNK;
            }
        }


        /// <summary>
        /// Checks The Viewport to see if we're inside.
        /// Requires using SamSeifert.GLE.GLR everywhere
        /// </summary>
        /// <param name="useColor"></param>
        public static bool RenderOnlyOnscreenObjects = false;
        private void Draw2(bool useColor)
        {
            if (CadObject.RenderOnlyOnscreenObjects)
            {
                this.UpdateBoundingSphere();

                if (this._BoundingSphereRadius == 0) return;

                var pos = (GL.getMatrix(MatrixMode.Modelview) * new Vector4(this._BoundingSphereCenter, 1)).Xyz;

                if (pos.Length > this._BoundingSphereRadius) // If camera is not inside object
                {
                    if (pos.Z + this._BoundingSphereRadius < -GLR.Projection_zFar) return; // Too far in front
                    if (pos.Z - this._BoundingSphereRadius > 0) return; // behind camera

                    {
                        // Check if object is to the right of the camera.
                        float angle = GLR.Projection_hFOV / 2;
                        angle -= MathHelper.DegreesToRadians(90);
                        Vector3 mover = new Vector3((float)Math.Sin(angle), 0, -(float)Math.Cos(angle));
                        if (Vector3.Dot(pos + this._BoundingSphereRadius * mover, mover) < 0) return;

                        // Check if object is to the left of the camera.
                        mover.X *= -1;
                        // Object is to the left of fov
                        if (Vector3.Dot(pos + this._BoundingSphereRadius * mover, mover) < 0) return;
                    }

                    {
                        // Check if object is above of the camera.
                        float angle = GLR.Projection_vFOV / 2;
                        angle -= MathHelper.DegreesToRadians(90);
                        Vector3 mover = new Vector3(0, (float)Math.Sin(angle), -(float)Math.Cos(angle));
                        if (Vector3.Dot(pos + this._BoundingSphereRadius * mover, mover) < 0) return;

                        // Check if object is below the camera.
                        mover.Y *= -1;
                        // Object is to the left of fov
                        if (Vector3.Dot(pos + this._BoundingSphereRadius * mover, mover) < 0) return;
                    }
                }
            }

            this.Draw3(useColor);                            
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
                if (this.AnonymousDraw == null)
                {
                    if ((useColor) && (this._Color != null)) this._Color.sendToGL();

                    switch (this._GLType)
                    {
                        case GLType.UNK:
                            {
                                break;
                            }
                        case GLType.GL3:
                            {
                                if (this._BoolSetupGL3) this.DrawGL3();
                                else if (this.SetupGL3()) this._BoolSetupGL3 = true;
                                else this._GLType = GLType.UNK;
                                break;
                            }
                        case GLType.GL4:
                            {
                                if (this._BoolSetupGL4) this.DrawGL4();
                                else if (this.SetupGL4()) this._BoolSetupGL4 = true;
                                else this._GLType = GLType.UNK;
                                break;
                            }
                    }
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









        // GL Lists are Depracted in GL4
        private bool SetupGL3()
        {
            this._IntList = GL.GenLists(1);
            GL.NewList(this._IntList, ListMode.Compile);
            {
                GL.Begin(PrimitiveType.Triangles);
                {
                    foreach (var i in this._Indices)
                    {
                        GL.Normal3(this._Normals[i]);
                        GL.Vertex3(this._Vertices[i]);
                    }
                }
                GL.End();
            }
            GL.EndList();

            return true;
        }

        private void DrawGL3()
        {
            GL.CallList(this._IntList);
        }
























        private bool SetupGL4()
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












        

        internal List<CadObject> ConsolidateMatrices()
        {
            List<CadObject> all_objects = new List<CadObject>();
            this.ConsolidateMatrices(all_objects, Matrix4.Identity);
            return all_objects;
        }

        internal void ConsolidateMatrices(List<CadObject> all_objects, Matrix4 m4)
        {
            all_objects.Add(this);
            if (this._BoolUseTranslationAndRotation)
            {
                m4 = this._Matrix * m4;
                this._BoolUseTranslationAndRotation = false;
                this._Matrix = Matrix4.Identity;
            }
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

                    t = Vector4.Transform(t, m4);

                    _Normals[i].X = t.X;
                    _Normals[i].Y = t.Y;
                    _Normals[i].Z = t.Z;
                }
            }
        }
    }
}
