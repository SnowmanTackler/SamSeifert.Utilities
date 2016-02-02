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
    public class FrameBuffers : DeleteableObject
    {
        public int _ColorText { get; private set; } = 0;
        private int _FrameBuffer = 0;
        public int _DepthBuffer { get; private set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="sucess"></param>
        /// <param name="color_texture_pixel_format"></param>
        /// <param name="color_texture_pixel_type"></param>
        /// <param name="depth_texture_pixel_type"></param>
        /// <param name="interpolation_mode">InterpolationMode</param>
        public FrameBuffers(
            Size resolution,
            out bool sucess,
            PixelFormat color_texture_pixel_format = PixelFormat.Rgb,
            PixelType color_texture_pixel_type = PixelType.Byte,
            PixelInternalFormat depth_texture_pixel_type = PixelInternalFormat.DepthComponent24,
            TextureMinFilter interpolation_mode = TextureMinFilter.Nearest
            )
        {
            try
            {
                this._FrameBuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._FrameBuffer);

                int temp;

                GL.GenTextures(1, out temp); this._ColorText = temp;
                GL.BindTexture(TextureTarget.Texture2D, this._ColorText);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)interpolation_mode);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)interpolation_mode);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgb8,
                    resolution.Width,
                    resolution.Height,
                    0,
                    color_texture_pixel_format,
                    color_texture_pixel_type,
                    IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this._ColorText, 0);

                GL.GenTextures(1, out temp); this._DepthBuffer = temp;
                GL.BindTexture(TextureTarget.Texture2D, this._DepthBuffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)interpolation_mode);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)interpolation_mode);
                GL.TexImage2D(TextureTarget.Texture2D, 0, depth_texture_pixel_type, resolution.Width, resolution.Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, this._DepthBuffer, 0);

                // Not doing depth as texture means we can't draw it!
                // this._DepthBuffer = GL.GenTextures(1, out this._DepthBuffer);
                // GLO.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this._DepthBuffer);
                // GLO.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, sz.Width, sz.Height);
                // GLO.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment RenderbufferTarget.Renderbuffer, this._DepthBuffer);

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
            }
            finally
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
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
                GL.DeleteTexture(this._DepthBuffer);
                this._DepthBuffer = 0;
            }
            if (this._FrameBuffer != 0)
            {
                GL.DeleteFramebuffer(this._FrameBuffer);
                this._FrameBuffer = 0;
            }
        }


        /// <summary>
        /// If we want to draw on the buffer, we can call using(this.asDrawable) and this 
        /// automatically sets up the drawing and turns it off when we're done!
        /// </summary>
        public class Drawable : IDisposable
        {
            public Drawable(int frame_buffer_index)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frame_buffer_index);
                GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0);
            }

            public void Dispose()
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.DrawBuffer(DrawBufferMode.Back);
            }
        }

        /// <summary>
        /// Make sure you wrap this is an using()
        /// </summary>
        public Drawable asDrawable
        {
            get
            {
                return new Drawable(this._FrameBuffer);
            }
        }
    }
}

