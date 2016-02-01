using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
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

        private readonly FrameBuffers[] _FrameBuffers = new FrameBuffers[6];
        private readonly Matrix4[] _Matrices = new Matrix4[]
        {
            Matrix4.CreateRotationY(-MathHelper.PiOver2), // Left
            Matrix4.Identity, // Front
            Matrix4.CreateRotationY(MathHelper.PiOver2), // Right
            Matrix4.CreateRotationY(MathHelper.Pi), // Back
            Matrix4.CreateRotationX(MathHelper.PiOver2), // Up
            Matrix4.CreateRotationX(-MathHelper.PiOver2), // Down
        };
         
        private readonly Size _Resolution;

        public CubeMaps(Size resolution, out bool sucess)
        {
            this._Resolution = resolution;

            for (int i = 0; i < 6; i++)
            {
                this._FrameBuffers[i] = new FrameBuffers(resolution, out sucess, interpolation_mode: TextureMinFilter.Linear);
                if (!sucess) return;
            }

            sucess = true;
        }

        public void GLDelete()
        {
            foreach (var fb in this._FrameBuffers)
                if (fb != null)
                    fb.GLDelete();
        }

        /// <summary>
        /// Current Model View Should be Straight Forward!
        /// </summary>
        /// <param name="render"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <param name="m">Model View Should to Straight Forward</param>
        public void Render(Action render, float zNear, float zFar, ref Matrix4 m)
        {
            GL.loadProjection(90, 90, zNear, zFar);
            GL.Viewport(0, 0, this._Resolution.Width, this._Resolution.Height);
            GL.MatrixMode(MatrixMode.Modelview);

            for (int i = 0; i < 6; i++)
            {
                using (this._FrameBuffers[i].asDrawable)
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.LoadIdentity();
                    GL.MultMatrix(ref this._Matrices[i]);
                    GL.MultMatrix(ref m);
                    render();
                }
            }
        }

        public void RenderOnScreen()
        {
            var ortho = Matrix4.CreateOrthographicOffCenter(0, 4, 3, 0, 0, 1);
            GL.loadProjectionOrtho(ref ortho);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);

            GL.Color3(Color.White);

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferLeft._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, 2);
            GL.TexCoord2(0, 1); GL.Vertex2(0, 2);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferFront._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(2, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(2, 2);
            GL.TexCoord2(0, 1); GL.Vertex2(1, 2);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferRight._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(2, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(3, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(3, 2);
            GL.TexCoord2(0, 1); GL.Vertex2(2, 2);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferBack._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(3, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(4, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(4, 2);
            GL.TexCoord2(0, 1); GL.Vertex2(3, 2);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferTop._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(1, 0);
            GL.TexCoord2(1, 0); GL.Vertex2(2, 0);
            GL.TexCoord2(1, 1); GL.Vertex2(2, 1);
            GL.TexCoord2(0, 1); GL.Vertex2(1, 1);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, this._FrameBufferBottom._ColorText);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(1, 2);
            GL.TexCoord2(1, 0); GL.Vertex2(2, 2);
            GL.TexCoord2(1, 1); GL.Vertex2(2, 3);
            GL.TexCoord2(0, 1); GL.Vertex2(1, 3);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }
    }
}





