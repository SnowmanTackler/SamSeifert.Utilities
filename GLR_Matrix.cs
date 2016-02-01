using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace SamSeifert.GLE
{
    public static partial class GLR
    {

        private static OpenTK.Graphics.OpenGL.MatrixMode _MatrixMode = OpenTK.Graphics.OpenGL.MatrixMode.Projection;

        private static Dictionary<OpenTK.Graphics.OpenGL.MatrixMode, List<Matrix4>>
            _MatrixDict = new Dictionary<OpenTK.Graphics.OpenGL.MatrixMode, List<Matrix4>>();

        private static List<Matrix4> _MatrixList { get { return GLR._MatrixDict[GLR._MatrixMode]; } }

        static GLR()
        {
            foreach (MatrixMode mm in Enum.GetValues(typeof(MatrixMode)))
            {
                GLR._MatrixDict[mm] = new List<Matrix4>();
                GLR._MatrixDict[mm].Add(Matrix4.Identity);
            }
        }

        public static void PushMatrix()
        {
            GL.PushMatrix();
            var ls = GLR._MatrixList;
            ls.Add(ls.Last());
        }

        public static void PopMatrix()
        {
            GL.PopMatrix();
            var ls = GLR._MatrixList;
            ls.RemoveAt(ls.Count - 1);
        }

        public static void MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode m)
        {
            GL.MatrixMode(m);
            GLR._MatrixMode = m;
        }

        public static void LoadIdentity()
        {
            Matrix4 eye = Matrix4.Identity;
            GLR.LoadMatrix(ref eye);
        }

        public static void LoadMatrix(ref Matrix4 m)
        {
            GL.LoadMatrix(ref m);
            var ls = GLR._MatrixList;
            ls[ls.Count - 1] = m;
        }

        public static void MultMatrix(ref Matrix4 m)
        {
            GL.MultMatrix(ref m);
            var ls = GLR._MatrixList;
            ls[ls.Count - 1] = m * ls[ls.Count - 1];
        }

        /// <summary>
        /// Only modify the matrix in OpenGL.  Not in our matrix holders!
        /// </summary>
        /// <param name="m"></param>
        public static void MultMatrixOnlyGL(ref Matrix4 m)
        {
            GL.MultMatrix(ref m);
        }



        public static void Translate(Vector3 v)
        {
            GLR.Translate(v.X, v.Y, v.Z);
        }

        public static void Translate(float v1, float v2, float v3)
        {
            Matrix4 m = Matrix4.CreateTranslation(v1, v2, v3);
            GLR.MultMatrix(ref m);
        }

        public static void Rotate(float degrees, Vector3 v)
        {
            GLR.Rotate(degrees, v.X, v.Y, v.Z);
        }

        public static void Rotate(float degrees, float x, float y, float z)
        {
            float radians = MathHelper.DegreesToRadians(degrees);

            float s = (float)Math.Sin(radians);
            float c = (float)Math.Cos(radians);

            Matrix4 m = Matrix4.Identity;

            m.M11 = x * x * (1 - c) + c;
            m.M21 = x * y * (1 - c) - z * s;
            m.M31 = x * z * (1 - c) + y * s;
            m.M12 = y * x * (1 - c) + z * s;
            m.M22 = y * y * (1 - c) + c;
            m.M32 = y * z * (1 - c) - x * s;
            m.M13 = z * x * (1 - c) - y * s;
            m.M23 = z * y * (1 - c) + x * s;
            m.M33 = z * z * (1 - c) + c;

            GLR.MultMatrix(ref m);
        }

        public static Matrix4 getMatrix(OpenTK.Graphics.OpenGL.MatrixMode m)
        {
            var ls = GLR._MatrixDict[m];
            return ls[ls.Count - 1];
        }




        public static float Projection_zNear { get; private set; }
        public static float Projection_zFar { get; private set; }

        /// <summary>
        /// Vertical Field Of View (Radians)
        /// </summary>
        public static float Projection_vFOV { get; private set; }

        /// <summary>
        /// Horizontal Field Of View (Radians)
        /// </summary>
        public static float Projection_hFOV { get; private set; }

        public static Matrix4 setProjection(
            int viewport_width,
            int viewport_height,
            float vertical_fov_degrees,
            float zNear,
            float zFar,
            int viewport_x = 0,
            int viewport_y = 0)
        {
            float aspect = viewport_width;
            aspect /= viewport_height;

            GLR.Projection_vFOV = MathHelper.DegreesToRadians(vertical_fov_degrees);
            GLR.Projection_hFOV = GLR.Projection_vFOV * aspect;
            GLR.Projection_zFar = zFar;
            GLR.Projection_zNear = zNear;

            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(GLR.Projection_vFOV, aspect, zNear, zFar);
            GL.LoadMatrix(ref p);
            GL.MatrixMode(GLR._MatrixMode);

            GL.Viewport(viewport_x, viewport_y, viewport_width, viewport_height);

            return p;
        }

        public static void loadProjection(ref Matrix4 p, float vertical_fov_degrees, float horizontal_fov_degrees, float zNear, float zFar)
        {
            GLR.Projection_vFOV = MathHelper.DegreesToRadians(vertical_fov_degrees);
            GLR.Projection_hFOV = MathHelper.DegreesToRadians(horizontal_fov_degrees);
            GLR.Projection_zFar = zFar;
            GLR.Projection_zNear = zNear;

            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadMatrix(ref p);
            GL.MatrixMode(GLR._MatrixMode);
        }

        public static void loadProjection(float vertical_fov_degrees, float horizontal_fov_degrees, float zNear, float zFar)
        {
            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(vertical_fov_degrees), horizontal_fov_degrees / vertical_fov_degrees, zNear, zFar);
            GLR.loadProjection(ref p, vertical_fov_degrees, horizontal_fov_degrees, zNear, zFar);
        }

        public static void loadProjectionOrtho(ref Matrix4 p)
        {
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadMatrix(ref p);
            GL.MatrixMode(GLR._MatrixMode);
        }
    }
}
