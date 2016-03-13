using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLO = OpenTK.Graphics.OpenGL.GL;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class CubeDepthMap : DeleteableObject
    {
        private readonly int _Resolution = 0;
        public int _ColorText { get; private set; } = 0;
        private int _FrameBuffer = 0;
        private int _DepthBuffer = 0;

        public CubeDepthMap(int resolution, out bool success)
        {
            try
            {
                this._Resolution = resolution;

                this._FrameBuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);

                {
                    this._ColorText = GL.GenTexture();
                    GL.BindTexture(TextureTarget.TextureCubeMap, this._ColorText);

                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, 0);

                    //Define all 6 faces
                    for (int i = 0; i < 6; i++)
                    {
                        this._Use[i] = true;
                        GL.TexImage2D(
                            CubeColorMap._TextureTargets[i],
                            0,
                            PixelInternalFormat.DepthComponent,
                            this._Resolution,
                            this._Resolution,
                            0,
                            PixelFormat.DepthComponent,
                            PixelType.UnsignedByte,
                            IntPtr.Zero);
                    }
                }

                success = true;
            }
            finally
            {
                GL.BindTexture(TextureTarget.TextureCubeMap, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GLO.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
        }

        public void GLDelete()
        {
            if (this._ColorText != 0)
            {
                GL.DeleteTexture(this._ColorText);
                this._ColorText = 0;
            }
            if (this._DepthBuffer != 0)
            {
                GL.DeleteRenderbuffer(this._DepthBuffer);
                this._DepthBuffer = 0;
            }
            if (this._FrameBuffer != 0)
            {
                GL.DeleteFramebuffer(this._FrameBuffer);
                this._FrameBuffer = 0;
            }
        }

        /// <summary>
        /// Current Model View Should be Straight Forward!
        /// </summary>
        /// <param name="render"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <param name="m">Model View Should to Straight Forward</param>
        public void Render(Action render, float zNear, float zFar, Matrix4 m)
        {
            m.Invert();

            GL.loadProjection(90, 90, zNear, zFar);
            GL.Viewport(0, 0, this._Resolution, this._Resolution);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);
            GL.DrawBuffer(DrawBufferMode.None);

            for (int i = 0; i < 6; i++)
            {
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.DepthAttachment,
                    CubeColorMap._TextureTargets[i],
                    this._ColorText,
                    0);

                GL.Clear(ClearBufferMask.DepthBufferBit);

                GL.LoadMatrix(ref CubeColorMap._Matrices[i]);
                GL.MultMatrix(ref m);

                render();
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        public bool[] _Use = new bool[6];
        public void Skip(TextureTarget t)
        {
            for (int i = 0; i < 6; i++)
                if (CubeColorMap._TextureTargets[i] == t)
                {
                    this._Use[i] = false;
                    break;
                }
        }
    }
}