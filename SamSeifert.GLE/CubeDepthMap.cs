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
using SamSeifert.Utilities;
using SamSeifert.GLE.Extensions;

namespace SamSeifert.GLE
{
    public class CubeDepthMap : DeleteableObject
    {
        private readonly int _Resolution = 0;
        public int _ColorText { get; private set; } = 0;
        private int _FrameBuffer = 0;
        private int _DepthBuffer = 0;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="resolution"></param>
        /// <param name="min_interpolate_mode">All.Nearest or All.Linear casted to int</param>
        /// <param name="max_interpolate_mode">All.Nearest or All.Linear casted to int</param>
        /// <param name="pif"></param>
        /// <param name="pt"></param>
        public CubeDepthMap(
            out bool success, 
            int resolution,
            int min_interpolate_mode = (int)All.Nearest,
            int max_interpolate_mode = (int)All.Nearest,
            PixelInternalFormat pif = PixelInternalFormat.DepthComponent32f,
            PixelType pt = PixelType.Float)
        {
            try
            {
                this._Resolution = resolution;

                this._FrameBuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);

                // For Frame Buffer
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

                {
                    this._ColorText = GL.GenTexture();
                    GL.BindTexture(TextureTarget.TextureCubeMap, this._ColorText);

                    // Warning: if you want to read the depth values directly as floats, 
                    // you must disable depth comparison mode using GL_TEXTURE_COMPARE_MODE. 
                    // Reading a depth texture as a color texture without disabling depth comparison is undefined behavior.
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);

                    // PRevent black lines around edge of map.
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0); // Prevents Mip Maps
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, 0);

                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, min_interpolate_mode); 
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, max_interpolate_mode);

                    //Define all 6 faces
                    for (int i = 0; i < 6; i++)
                    {
                        this._Use[i] = true;
                        GL.TexImage2D(
                            CubeColorMap._TextureTargets[i],
                            0,
                            pif,
                            this._Resolution,
                            this._Resolution,
                            0,
                            PixelFormat.DepthComponent,
                            pt,
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
        public void Render(Action<CameraDescriptor> render, float zNear, float zFar, Matrix4 m)
        {
            // var actual_inverse = m.Inverted(); // To Compare

            m = m.InvertedViewMatrix();

            // Logger.WriteLine(m.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);
            GL.DrawBuffer(DrawBufferMode.None); // Don't draw color on anything?

            for (int i = 0; i < 6; i++)
            {
                if (this._Use[i])
                {
                    var eye = m * CubeColorMap._Matrices[i];

                    var cam = new CameraDescriptor(
                        this._Resolution,
                        this._Resolution,
                        90,
                        true,
                        zNear,
                        zFar,
                        eye
                        );

                    GL.FramebufferTexture2D(
                        FramebufferTarget.Framebuffer,
                        FramebufferAttachment.DepthAttachment,
                        CubeColorMap._TextureTargets[i],
                        this._ColorText,
                        0);

                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    cam.SendToGL();
                    render(cam);
                }
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