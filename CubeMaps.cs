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
    public class CubeMaps : DeleteableObject
    {
        private readonly Matrix4[] _Matrices = new Matrix4[]
        {
            Matrix4.CreateRotationY(MathHelper.PiOver2) , // Front OF START
            Matrix4.CreateRotationZ(MathHelper.Pi) * Matrix4.CreateRotationZ(-MathHelper.Pi), // Bottom
            Matrix4.CreateRotationY(-MathHelper.PiOver2) , // Back OF START
            Matrix4.CreateRotationX(MathHelper.Pi) * Matrix4.CreateRotationZ(-MathHelper.Pi), // Top OF START
            Matrix4.CreateRotationX(MathHelper.PiOver2)* Matrix4.CreateRotationZ(-MathHelper.Pi), // Left OF START
            Matrix4.CreateRotationX(-MathHelper.PiOver2)* Matrix4.CreateRotationZ(-MathHelper.Pi), // Right OF START
        };

        private readonly TextureTarget[] _TextureTargets = new TextureTarget[]
       {
            TextureTarget.TextureCubeMapNegativeX,
            TextureTarget.TextureCubeMapNegativeZ,
            TextureTarget.TextureCubeMapPositiveX,
            TextureTarget.TextureCubeMapPositiveZ,
            TextureTarget.TextureCubeMapPositiveY,
            TextureTarget.TextureCubeMapNegativeY
       };

        private readonly int _Resolution = 0;
        public int _ColorText { get; private set; } = 0;
        private int _FrameBuffer = 0;
        private int _DepthBuffer = 0;

        public CubeMaps(int resolution, out bool sucess)
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
                        GL.TexImage2D(
                            this._TextureTargets[i],
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
                    /*
                    // Not doing depth as texture means we can't draw it!
                    this._DepthBuffer = GLO.GenRenderbuffer();
                    GLO.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this._DepthBuffer);
                    GLO.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, this._Resolution, this._Resolution);
                    GLO.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, this._DepthBuffer); //*/

                    GL.GenTextures(1, out this._DepthBuffer);
                    GL.BindTexture(TextureTarget.Texture2D, this._DepthBuffer);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, this._Resolution, this._Resolution, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, this._DepthBuffer, 0);
                }

                switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
                {
                    case FramebufferErrorCode.FramebufferComplete:
                        sucess = true;
                        break;
                    default:
                        Console.WriteLine("FrameBufferIndices Error");
                        sucess = false;
                        break;
                }

                sucess = true;
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
            /*
            if (this._ColorText != 0)
            {
                GL.DeleteTexture(this._ColorText);
                this._ColorText = 0;
            }*/
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
            GL.Viewport(0, 0, this._Resolution, this._Resolution);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0);

            for (int i = 0; i < 6; i++)
            {
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0, 
                    this._TextureTargets[i], 
                    this._ColorText,
                    0);

                /*
                switch (i)
                {
                    case 0: GL.ClearColor(Color.Red); break; // Left
                    case 1: GL.ClearColor(Color.Green); break; // Front
                    case 2: GL.ClearColor(Color.Blue); break; // Right
                    case 3: GL.ClearColor(Color.Yellow); break; // Back
                    case 4: GL.ClearColor(Color.Cyan); break; // Up
                    case 5: GL.ClearColor(Color.Magenta); break; // Down
                }
                */

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.LoadMatrix(ref this._Matrices[i]);
                GL.MultMatrix(ref m);

                render();
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DrawBuffer(DrawBufferMode.Back);

            GL.ClearColor(Color.Black);
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
            GLO.TexCoord3(-1,  1, -1); GL.Vertex2(0, 1);
            GLO.TexCoord3(-1,  1,  1); GL.Vertex2(1, 1);
            GLO.TexCoord3(-1, -1,  1); GL.Vertex2(1, 2);
            GLO.TexCoord3(-1, -1, -1); GL.Vertex2(0, 2);

            // Front
            GLO.TexCoord3( 1,  1, -1); GL.Vertex2(1, 1);
            GLO.TexCoord3(-1,  1, -1); GL.Vertex2(2, 1);
            GLO.TexCoord3(-1, -1, -1); GL.Vertex2(2, 2);
            GLO.TexCoord3( 1, -1, -1); GL.Vertex2(1, 2);

            // Right
            GLO.TexCoord3( 1,  1,  1); GL.Vertex2(2, 1);
            GLO.TexCoord3( 1,  1, -1); GL.Vertex2(3, 1);
            GLO.TexCoord3( 1, -1, -1); GL.Vertex2(3, 2);
            GLO.TexCoord3( 1, -1,  1); GL.Vertex2(2, 2);
               
            // Back
            GLO.TexCoord3(-1,  1,  1); GL.Vertex2(3, 1);
            GLO.TexCoord3( 1,  1,  1); GL.Vertex2(4, 1);
            GLO.TexCoord3( 1, -1,  1); GL.Vertex2(4, 2);
            GLO.TexCoord3(-1, -1,  1); GL.Vertex2(3, 2);

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
    }
}





