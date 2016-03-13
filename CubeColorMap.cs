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
    public class CubeColorMap : DeleteableObject
    {
        public static readonly Matrix4[] _Matrices = new Matrix4[]
        {
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitX, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitX, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitY, -Vector3.UnitZ),
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, -Vector3.UnitY),
        };

        public static readonly TextureTarget[] _TextureTargets = new TextureTarget[]
        {
            TextureTarget.TextureCubeMapPositiveX,
            TextureTarget.TextureCubeMapNegativeX,
            TextureTarget.TextureCubeMapPositiveY,
            TextureTarget.TextureCubeMapNegativeY,
            TextureTarget.TextureCubeMapPositiveZ,
            TextureTarget.TextureCubeMapNegativeZ,
        };

        private readonly int _Resolution = 0;
        public int _ColorText { get; private set; } = 0;
        private int _FrameBuffer = 0;
        private int _DepthBuffer = 0;

        public CubeColorMap(int resolution, out bool success)
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
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, 0);

                    //Define all 6 faces
                    for (int i = 0; i < 6; i++)
                    {
                        this._Use[i] = true;
                        GL.TexImage2D(
                            CubeColorMap._TextureTargets[i],
                            0,
                            PixelInternalFormat.Rgb,
                            this._Resolution,
                            this._Resolution,
                            0,
                            PixelFormat.Bgr,
                            PixelType.UnsignedByte,
                            IntPtr.Zero);
                    }
                }
                {
                    // Not doing depth as texture means we can't draw it!
                    this._DepthBuffer = GLO.GenRenderbuffer();
                    GLO.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this._DepthBuffer);
                    GLO.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, this._Resolution, this._Resolution);

                    // Assign render buffer to the frame buffer.
                    GLO.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, this._DepthBuffer);
                }

                switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
                {
                    case FramebufferErrorCode.FramebufferComplete:
                        success = true;
                        break;
                    default:
                        Console.WriteLine("CubeColorMap Error");
                        success = false;
                        break;
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
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0);

            for (int i = 0; i < 6; i++)
            {
                if (this._Use[i])
                {
                    GL.FramebufferTexture2D(
                        FramebufferTarget.Framebuffer,
                        FramebufferAttachment.ColorAttachment0,
                        CubeColorMap._TextureTargets[i],
                        this._ColorText,
                        0);

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.LoadMatrix(ref CubeColorMap._Matrices[i]);
                    GL.MultMatrix(ref m);

                    render();
                }
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        public void RenderOnScreen()
        {
            var ortho = Matrix4.CreateOrthographicOffCenter(0, 4, 0, 3, 0, 1);
            GL.loadProjectionOrtho(ref ortho);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);

            GL.Color3(Color.White);

            GL.Enable(EnableCap.TextureCubeMap);
            GL.BindTexture(TextureTarget.TextureCubeMap, this._ColorText);

            GL.Begin(PrimitiveType.Quads);

            // Left
            GLO.TexCoord3(-1,  1, -1); GL.Vertex2(1, 2);
            GLO.TexCoord3(-1,  1,  1); GL.Vertex2(0, 2);
            GLO.TexCoord3(-1, -1,  1); GL.Vertex2(0, 1);
            GLO.TexCoord3(-1, -1, -1); GL.Vertex2(1, 1);

            // Front
            GLO.TexCoord3( 1,  1, -1); GL.Vertex2(2, 2);
            GLO.TexCoord3(-1,  1, -1); GL.Vertex2(1, 2);
            GLO.TexCoord3(-1, -1, -1); GL.Vertex2(1, 1);
            GLO.TexCoord3( 1, -1, -1); GL.Vertex2(2, 1);

            // Right 
            GLO.TexCoord3( 1,  1,  1); GL.Vertex2(3, 2);
            GLO.TexCoord3( 1,  1, -1); GL.Vertex2(2, 2);
            GLO.TexCoord3( 1, -1, -1); GL.Vertex2(2, 1);
            GLO.TexCoord3( 1, -1,  1); GL.Vertex2(3, 1);
               
            // Back
            GLO.TexCoord3(-1,  1,  1); GL.Vertex2(4, 2);
            GLO.TexCoord3( 1,  1,  1); GL.Vertex2(3, 2);
            GLO.TexCoord3( 1, -1,  1); GL.Vertex2(3, 1);
            GLO.TexCoord3(-1, -1,  1); GL.Vertex2(4, 1);

            // Bottom
            GLO.TexCoord3(-1, -1,  1); GL.Vertex2(1, 0);
            GLO.TexCoord3( 1, -1,  1); GL.Vertex2(2, 0);
            GLO.TexCoord3( 1, -1, -1); GL.Vertex2(2, 1);
            GLO.TexCoord3(-1, -1, -1); GL.Vertex2(1, 1);

            // Top
            GLO.TexCoord3(-1,  1, -1);  GL.Vertex2(1, 2);
            GLO.TexCoord3( 1,  1, -1);  GL.Vertex2(2, 2);
            GLO.TexCoord3( 1,  1,  1);  GL.Vertex2(2, 3);
            GLO.TexCoord3(-1,  1,  1);  GL.Vertex2(1, 3);

            GL.End();

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            GL.Disable(EnableCap.TextureCubeMap);

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
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