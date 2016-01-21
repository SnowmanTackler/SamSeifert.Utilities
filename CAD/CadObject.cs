using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL; using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE.CAD
{
    public class CadObject
    {
        internal bool BoolUseTranslationAndRotation = false;
        public bool BoolDisplay = true;
        internal String _Name = "Untitled";
        internal Matrix4 _Matrix = Matrix4.Identity;
        internal ColorGL _Color = null;

        internal CadObject[] Children = new CadObject[0];
        internal Vector3[] Vertices;
        internal Vector3[] Normals;
        internal uint[] Indices;

        private bool _BoolSetupGL3 = false;
        private bool _BoolSetupGL4 = false;
        private int _IntList; // Used to support GL3
        private int _IntInterleaveBufferID; // Used to support GL4
        private int _IntIndicesBufferID; // Used to support GL4

        internal enum GLType { GL3, GL4, UNK };
        internal GLType _GLType = GLType.UNK;

        private bool BoundingSphereNeeded = true;
        private Vector3 BoundingSphereCenter = Vector3.Zero;
        private float BoundingSphereRadius = 0;

        private static int UseColorTracker = 0;
        public static void NoColorOn() { UseColorTracker++; }
        public static void NoColorOff() { UseColorTracker--; }

        internal Action AnonymousDraw = null;

        public CadObject(CadObject[] cos, String name = "Group")
        {
            this.Children = cos;
            this._Name = name;
        }

        internal CadObject()
        {
        }

        internal CadObject(Vector3[] verts, Vector3[] norms, String name)
        {
            this._Name = name;
            this.initializeWithVectorsAndNormals(verts, norms);
        }

        public override string ToString()
        {
            return this._Name + " " + this._GLType;
        }

        public void GLDelete()
        {
            if (this.Children.Length > 0)
            {
                foreach (CadObject co in this.Children) co.GLDelete();
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

        internal void initializeWithVectorsAndNormals(Vector3[] v, Vector3[] n)
        {
            uint count;
            if (Helpers.ConsolidateData(v, n, out count, out this.Vertices, out this.Normals, out this.Indices)) this.initialize();
            else this._GLType = GLType.UNK;
        }

        internal void initializeWithVectorsAndNormalsSorted(Vector3[] v, Vector3[] n, uint[] ii)
        {
            this.Vertices = v;
            this.Normals = n;
            this.Indices = ii;

            this.initialize();
        }

        internal void initialize()
        {
            this.GLDelete();

            if (this.Vertices.Length != this.Normals.Length)
            {
                Console.WriteLine("CadObject." + System.Reflection.MethodBase.GetCurrentMethod().Name + " vertices and normals mismatch");
                this._GLType = GLType.UNK;
                return;
            }

            this._BoolSetupGL4 = this.setupGL4();
            if (this._BoolSetupGL4) this._GLType = GLType.GL4;
            else
            {
                this._BoolSetupGL3 = this.setupGL3();
                if (this._BoolSetupGL3) this._GLType = GLType.GL3;
                else this._GLType = GLType.UNK;

            }
        }

        public void setUseTranslationAndRotation(bool arg, bool recursive = true)
        {
            this.BoolUseTranslationAndRotation = arg;
            if (recursive) foreach (var e in this.Children) e.setUseTranslationAndRotation(arg);
        }

        public void setAlpha(float a)
        {
            if (this._Color != null) this._Color.setAlpha(a);
            foreach (var e in this.Children) e.setAlpha(a);
        }

        public void setColor(ColorGL c, bool recursive = true)
        {
            this._Color = c;
            if (recursive) foreach (var e in this.Children) e.setColor(c, recursive);
        }

        public void draw(bool useColor = true)
        {
            if (this.BoolDisplay)
            {
                if (this.BoolUseTranslationAndRotation)
                {
                    GL.PushMatrix();
                    {
                        GL.MultMatrix(ref this._Matrix);
                        this.draw2(useColor);
                    }
                    GL.PopMatrix();
                }
                else this.draw2(useColor);
            }
        }

        /// <summary>
        /// Checks The Viewport to see if we're inside.
        /// </summary>
        /// <param name="useColor"></param>
        private void draw2(bool useColor)
        {
            this.updateBoundingSphere();

            if (this.BoundingSphereRadius == 0) return;

            var pos = Vector3.Transform(this.BoundingSphereCenter, GL.getMatrix(MatrixMode.Modelview)); // Convert to Camera POV

            if (pos.Z + this.BoundingSphereRadius < -GLR.Projection_zFar) return; // Too far in front
            if (pos.Z - this.BoundingSphereRadius > 0) return; // behind camera

            {
                // Check if object is to the right of the camera.
                float angle = GLR.Projection_hFOV / 2;
                Vector3 Cutoff = new Vector3((float)Math.Sin(angle), 0, -(float)Math.Cos(angle));
                angle -= MathHelper.DegreesToRadians(90);
                Vector3 mover = new Vector3((float)Math.Sin(angle), 0, -(float)Math.Cos(angle));
                if (Vector3.Dot(pos + this.BoundingSphereRadius * mover, mover) < 0) return;

                // Check if object is to the left of the camera.
                mover.X *= -1;
                Cutoff.X *= -1;
                // Object is to the left of fov
                if (Vector3.Dot(pos + this.BoundingSphereRadius * mover, mover) < 0) return;
            }

            {
                // Check if object is above of the camera.
                float angle = GLR.Projection_vFOV / 2;
                Vector3 Cutoff = new Vector3(0, (float)Math.Sin(angle), -(float)Math.Cos(angle));
                angle -= MathHelper.DegreesToRadians(90);
                Vector3 mover = new Vector3(0, (float)Math.Sin(angle), -(float)Math.Cos(angle));
                if (Vector3.Dot(pos + this.BoundingSphereRadius * mover, mover) < 0) return;

                // Check if object is below the camera.
                mover.Y *= -1;
                Cutoff.Y *= -1;
                // Object is to the left of fov
                if (Vector3.Dot(pos + this.BoundingSphereRadius * mover, mover) < 0) return;
            }

            this.draw3(useColor);            
        }

        private void draw3(bool useColor)
        { 
            if (this.Children.Length > 0)
            {
                foreach (CadObject co in this.Children)
                {
                    co.draw(useColor);
                }
            }
            else
            {
                if (this.AnonymousDraw == null)
                {
                    if ((CadObject.UseColorTracker == 0) && (useColor) && (this._Color != null)) this._Color.sendToGL();

                    switch (this._GLType)
                    {
                        case GLType.UNK:
                            {
                                break;
                            }
                        case GLType.GL3:
                            {
                                if (this._BoolSetupGL3) this.drawGL3();
                                else if (this.setupGL3()) this._BoolSetupGL3 = true;
                                else this._GLType = GLType.UNK;
                                break;
                            }
                        case GLType.GL4:
                            {
                                if (this._BoolSetupGL4) this.drawGL4();
                                else if (this.setupGL4()) this._BoolSetupGL4 = true;
                                else this._GLType = GLType.UNK;
                                break;
                            }
                    }
                }
                else AnonymousDraw();
            }
        }

        internal void transformPoints(ref Matrix4 m4)
        {
            Vector4 v = Vector4.Zero;
            if (Vertices != null)
            {
                for (int i = 0; i < Vertices.Length; i++)
                {
                    v.X = Vertices[i].X;
                    v.Y = Vertices[i].Y;
                    v.Z = Vertices[i].Z;
                    v.W = 1;

                    v = Vector4.Transform(v, m4);

                    Vertices[i].X = v.X;
                    Vertices[i].Y = v.Y;
                    Vertices[i].Z = v.Z;
                }
            }

            if (Normals != null)
            {
                for (int i = 0; i < Normals.Length; i++)
                {
                    v.X = Normals[i].X;
                    v.Y = Normals[i].Y;
                    v.Z = Normals[i].Z;
                    v.W = 0;

                    v = Vector4.Transform(v, m4);

                    Normals[i].X = v.X;
                    Normals[i].Y = v.Y;
                    Normals[i].Z = v.Z;
                }
            }
        }







        private void updateBoundingSphere()
        {
            //var tempArray = new Vector3[this.Vertices.Length];  

            if (this.BoundingSphereNeeded)
            {
                this.BoundingSphereNeeded = false;

                if (this.Children.Length > 0) // Have to combine children bounding spheres into one giant bounding sphere
                {
                    // Bitter's Algorithm
                    var spheres = new List<Tuple<Vector3, float>>();

                    // Get spheres for all children!
                    foreach (var child in this.Children)
                    {
                        child.updateBoundingSphere();
                        spheres.Add(new Tuple<Vector3, float>(child.BoundingSphereCenter,
                                                              child.BoundingSphereRadius));
                    }

                    // Find Sphere Farthest from Origin
                    float furthest_distance = 0;
                    foreach (var sphere in spheres)
                    {
                        float dist = sphere.Item1.Length + sphere.Item2;

                        if (dist > furthest_distance)
                        {
                            furthest_distance = dist;
                            this.BoundingSphereCenter = sphere.Item1;
                            this.BoundingSphereRadius = sphere.Item2;
                        }
                    }

                    // Find farthest spheres not enclosed by bigger sphere
                    while (true)
                    {
                        furthest_distance = 0;
                        var furthest = new Tuple<Vector3, float>(Vector3.Zero, 0);
                        foreach (var sphere in spheres)
                        {
                            float dist = (this.BoundingSphereCenter - sphere.Item1).Length + sphere.Item2;

                            if (dist > furthest_distance)
                            {
                                furthest_distance = dist;
                                furthest = sphere;
                            }
                        }

                        if (furthest_distance < 1.005f * this.BoundingSphereRadius) break; // 1.005 is wiggle room

                        var center_to_farthest = furthest.Item1 - this.BoundingSphereCenter;
                        float new_radius = (center_to_farthest.Length + this.BoundingSphereRadius + furthest.Item2) / 2;
                        center_to_farthest.Normalize();
                        this.BoundingSphereCenter += center_to_farthest * (new_radius - this.BoundingSphereRadius);
                        this.BoundingSphereRadius = new_radius;
                    }
                }
                else
                {
                    var bestLength = 0f;
                    var bestV = Vector3.Zero;
                    foreach (var v in this.Vertices)
                    {
                        var length = (this.Vertices[0] - v).LengthSquared;
         
                        if (length > bestLength)
                        {
                            bestLength = length;
                            bestV = v;
                        }
                    }
                    bestLength = 0.0f;
                    var circlePoint1 = bestV;
                    foreach (var v in this.Vertices)
                    {
                        var length = (circlePoint1 - v).LengthSquared;

                        if (length > bestLength)
                        {
                            bestLength = length;
                            bestV = v;
                        }
                    }
                    this.BoundingSphereCenter = (circlePoint1 + bestV) / 2;
                    this.BoundingSphereRadius = (circlePoint1 - bestV).Length / 2;
                }

                this.BoundingSphereRadius *= 1.001f; // Just give a little clearance.
//                Console.WriteLine(this._Name + " " + this.BoundingSphereRadius);
            }
        }

        public void setBoundingSphere(Vector3 center, float radius)
        {
            this.BoundingSphereNeeded = false;
            this.BoundingSphereCenter = center;
            this.BoundingSphereRadius = radius;
        }









        // GL Lists are Depracted in GL4
        private bool setupGL3()
        {
            this._IntList = GL.GenLists(1);
            GL.NewList(this._IntList, ListMode.Compile);
            {
                GL.Begin(PrimitiveType.Triangles);
                {
                    foreach (var i in this.Indices)
                    {
                        GL.Normal3(this.Normals[i]);
                        GL.Vertex3(this.Vertices[i]);
                    }
                }
                GL.End();
            }
            GL.EndList();

            return true;
        }

        private void drawGL3()
        {
            GL.CallList(this._IntList);
        }
























        private bool setupGL4()
        {
            var idata = new Vector3[Vertices.Length * 2];
            for (int i = 0; i < Vertices.Length; i ++)
            {
                idata[i * 2] = this.Vertices[i];
                idata[i * 2 + 1] = this.Normals[i];
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

            bufferSizeE = this.Indices.Length * sizeof(uint);
            GL.GenBuffers(1, out _IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), this.Indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (bufferSizeE != bufferSize) return false;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
            return true;
        }

        private void drawGL4()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 2, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 2, Vector3.SizeInBytes);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(PrimitiveType.Triangles, this.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }








    }
}
