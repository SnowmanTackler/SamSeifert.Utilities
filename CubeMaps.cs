using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class CubeMaps : DeleteableObject
    {
        private FrameBuffers _FrameBufferLeft
        {
            get
            {
                return this._FrameBuffers[0];
            }
            set
            {
                this._FrameBuffers[0] = value;
            }
        }

        private FrameBuffers _FrameBufferFront
        {
            get
            {
                return this._FrameBuffers[1];
            }
            set
            {
                this._FrameBuffers[1] = value;
            }
        }

        private FrameBuffers _FrameBufferRight
        {
            get
            {
                return this._FrameBuffers[2];
            }
            set
            {
                this._FrameBuffers[2] = value;
            }
        }

        private FrameBuffers _FrameBufferBack
        {
            get
            {
                return this._FrameBuffers[3];
            }
            set
            {
                this._FrameBuffers[3] = value;
            }
        }

        private FrameBuffers _FrameBufferTop
        {
            get
            {
                return this._FrameBuffers[4];
            }
            set
            {
                this._FrameBuffers[4] = value;
            }
        }

        private FrameBuffers _FrameBufferBottom
        {
            get
            {
                return this._FrameBuffers[5];
            }
            set
            {
                this._FrameBuffers[5] = value;
            }
        }

        private FrameBuffers[] _FrameBuffers = new FrameBuffers[6];
        private Matrix4[] _Matrices = new Matrix4[6];
         
        private readonly Size _Resolution;

        public CubeMaps(Size resolution, out bool sucess)
        {
            this._Resolution = resolution;

            this._FrameBufferLeft = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferFront = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferRight = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferBack = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferTop = new FrameBuffers(resolution, out sucess);
            if (!sucess) return;
            this._FrameBufferBottom = new FrameBuffers(resolution, out sucess);
        }

        public void GLDelete()
        {
            foreach (var fb in this._FrameBuffers)
                if (fb != null)
                    fb.GLDelete();
        }

        public void Render(Action a, float zNear, float zFar)
        {
            GL.loadProjection(90, 90, zNear, zFar);
            GL.Viewport(0, 0, this._Resolution.Width, this._Resolution.Height);

            for (int i = 0; i < 6; i++)
            {
                using (this._FrameBuffers[i].asDrawable)
                {
                    GL.PushMatrix();
                    {
                        GL.MultMatrix(ref this._Matrices[i]);

                    }
                    GL.PopMatrix();
                }
            }
        }

    }
}





