using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class CameraDescriptor
    {
        public readonly Rectangle _Viewport;
        public readonly Matrix4 _Projection;
        public readonly float _zFar;
        public readonly float _zNear;
        public readonly float _HorizontalFOV_Radians;
        public readonly float _VerticalFOV_Radians;
        public readonly Matrix4 _ModelView;

        public readonly Vector3 _ModelView_Eye;
        public readonly Vector3 _ModelView_Target;
        public readonly Vector3 _ModelView_Up;

        public Size _Resolution { get { return this._Viewport.Size; } }
        public float _Width { get { return this._Viewport.Width; } }
        public float _Height { get { return this._Viewport.Height; } }

        public float _HorizontalFOV_Degrees { get { return MathHelper.RadiansToDegrees(this._HorizontalFOV_Radians); } }
        public float _VerticalFOV_Degrees { get { return MathHelper.RadiansToDegrees(this._VerticalFOV_Radians); } }

        private CameraDescriptor(
            int viewport_width,
            int viewport_height,
            float fov_degrees,
            bool fov_vertical_true__fov_horizontal_false,
            float zNear,
            float zFar,
            int viewport_x,
            int viewport_y)
        {
            this._Viewport = new Rectangle(viewport_x, viewport_y, viewport_width, viewport_height);

            float aspect = viewport_width;
            aspect /= viewport_height;

            if (!fov_vertical_true__fov_horizontal_false)
                fov_degrees = CalculateVerticalFov(fov_degrees, aspect);

            this._VerticalFOV_Radians = MathHelper.DegreesToRadians(fov_degrees);
            this._HorizontalFOV_Radians = CalculateHorizontalFov(this._VerticalFOV_Radians, aspect);
            this._zNear = zNear;
            this._zFar = zFar;

            this._Projection = Matrix4.CreatePerspectiveFieldOfView(this._VerticalFOV_Radians, aspect, zNear, zFar);
        }

        public CameraDescriptor(
            int viewport_width,
            int viewport_height,
            float vertical_fov_degrees,
            bool fov_vertical_true__fov_horizontal_false,
            float zNear,
            float zFar,
            Matrix4 eye,
            int viewport_x = 0,
            int viewport_y = 0) : this(viewport_width, viewport_height, vertical_fov_degrees, fov_vertical_true__fov_horizontal_false, zNear, zFar, viewport_x, viewport_y)
        {
            this._ModelView = eye;

            eye.Invert();

            this._ModelView_Eye = eye.Row3.Xyz;

            // TODO: CALCULATE THESE CORRECTLY.
            this._ModelView_Target = -Vector3.UnitZ;
            this._ModelView_Up = Vector3.UnitY;
        }

        public CameraDescriptor(
            int viewport_width,
            int viewport_height,
            float vertical_fov_degrees,
            bool fov_vertical_true__fov_horizontal_false,
            float zNear,
            float zFar,
            Vector3 eye,
            Vector3 target,
            Vector3 up,
            int viewport_x = 0,
            int viewport_y = 0) : this(viewport_width, viewport_height, vertical_fov_degrees, fov_vertical_true__fov_horizontal_false, zNear, zFar, viewport_x, viewport_y)
        {
            this._ModelView_Eye = eye;
            this._ModelView_Target = target;
            this._ModelView_Up = up;
            this._ModelView = Matrix4.LookAt(eye, target, up);
        }

        /// <summary>
        /// Sets MatrixMode to ModelView
        /// </summary>
        public void SendToGL()
        {
            GLR.LoadCamera(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizontal_fov_radians"></param>
        /// <param name="aspect"> width / height</param>
        /// <returns>radians</returns>
        public static float CalculateVerticalFov(float horizontal_fov_radians, float aspect)
        {
            // tan(fovy / 2) = (h / 2) / L
            // tan(fovx / 2) = (w / 2) / L
            return 2 * (float)Math.Atan(((float)Math.Tan(horizontal_fov_radians / 2)) / aspect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertical_fov_radians"></param>
        /// <param name="aspect"> width / height</param>
        /// <returns>radians</returns>
        public static float CalculateHorizontalFov(float vertical_fov_radians, float aspect)
        {
            // tan(fovy / 2) = (h / 2) / L
            // tan(fovx / 2) = (w / 2) / L
            return 2 * (float)Math.Atan(((float)Math.Tan(vertical_fov_radians / 2)) * aspect);
        }







        private Vector3[][] _PolygonMesh = null;
        private Vector3 _PolygonMeshCenter = Vector3.Zero;
        /// <summary>
        /// Returns a set of polygons that enclose the visible region of the camera
        /// </summary>
        /// <param name="valid_region_faces"></param>
        /// <param name="valid_region_center"></param>
        public void GetVisiblePolygonMesh(out Vector3[][] valid_region_faces, out Vector3 valid_region_center)
        {
            if (this._PolygonMesh == null)
            {
                float half_angle_horiz = this._HorizontalFOV_Radians / 2;
                float half_angle_vert = this._VerticalFOV_Radians / 2;

                // Unit Vectors to top left, right, etc corners of view port
                Vector3 bot_r, bot_l, top_l, top_r = new Vector3(
                    (float)Math.Tan(half_angle_horiz),
                    (float)Math.Tan(half_angle_vert),
                    -1
                    ).Normalized();
                top_l = top_r;
                top_l.X *= -1;
                bot_r = top_r;
                bot_r.Y *= -1;
                bot_l = top_l;
                bot_l.Y *= -1;

                this._PolygonMeshCenter = -Vector3.UnitZ * (this._zNear + this._zFar) / 2;

                float zn = this._zNear / Math.Abs(top_r.Z); // Adjust zNear and zFar because we'll be multiplying 
                float zf = this._zFar / Math.Abs(top_r.Z); // them by a vector that doesn't point directly to z

                this._PolygonMesh = new Vector3[][]
                {
                    new Vector3[] { top_l * zn, top_l * zf, top_r * zf, top_r * zn }, // Top
                    new Vector3[] { bot_l * zn, bot_l * zf, bot_r * zf, bot_r * zn }, // Bottom

                    new Vector3[] { top_l * zn, top_l * zf, bot_l * zf, bot_l * zn }, // Left
                    new Vector3[] { top_r * zn, top_r * zf, bot_r * zf, bot_r * zn }, // Right

                    new Vector3[] { top_l * zn, top_r * zn, bot_r * zn, bot_l * zn }, // Near
                    new Vector3[] { top_l * zf, top_r * zf, bot_r * zf, bot_l * zf }, // Far
                };
            }
            valid_region_faces = this._PolygonMesh;
            valid_region_center = this._PolygonMeshCenter;
        }
    }
}
