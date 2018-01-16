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
        public readonly Size _Resolution;
        public readonly Rectangle _Viewport;
        public readonly Matrix4 _Projection;
        public readonly float _zFar;
        public readonly float _ZNear;
        public readonly float _HorizontalFOV;
        public readonly float _VerticalFOV;
        public readonly Matrix4 _ModelView;

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
                fov_degrees /= aspect;

            this._VerticalFOV = MathHelper.DegreesToRadians(fov_degrees);
            this._HorizontalFOV = this._VerticalFOV * aspect;
            this._zFar = zFar;
            this._ZNear = zNear;

            this._Projection = Matrix4.CreatePerspectiveFieldOfView(this._VerticalFOV, aspect, zNear, zFar);
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
            this._ModelView = Matrix4.LookAt(eye, target, up);
        }

        /// <summary>
        /// Sets MatrixMode to ModelView
        /// </summary>
        public void SendToGL()
        {
            GLR.LoadCamera(this);
        }
    }
}
