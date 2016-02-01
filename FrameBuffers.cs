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
        public int _FrameBuffer { get; private set; } = 0;
        public int _DepthBuffer { get; private set; } = 0;

        public FrameBuffers(
            Size resolution,
            out bool sucess,
            PixelFormat color_texture_pixel_format = PixelFormat.Rgb,
            PixelType color_texture_pixel_type = PixelType.Byte,
            PixelInternalFormat depth_texture_pixel_type = PixelInternalFormat.DepthComponent24
            )
        {
            try
            {
                this._FrameBuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._FrameBuffer);

                int temp;

                GL.GenTextures(1, out temp); this._ColorText = temp;
                GL.BindTexture(TextureTarget.Texture2D, this._ColorText);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
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
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this._ColorText, 0);

                GL.GenTextures(1, out temp); this._DepthBuffer = temp;
                GL.BindTexture(TextureTarget.Texture2D, this._DepthBuffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
                GL.TexImage2D(TextureTarget.Texture2D, 0, depth_texture_pixel_type, resolution.Width, resolution.Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, this._DepthBuffer, 0);

                // Not doing depth as texture means we can't draw it!
                // this._DepthBuffer = GL.GenTextures(1, out this._DepthBuffer);
                // GLO.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, this._DepthBuffer);
                // GLO.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.DepthComponent24, sz.Width, sz.Height);
                // GLO.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, this._DepthBuffer);

                switch (GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
                {
                    case FramebufferErrorCode.FramebufferCompleteExt:
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
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
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
    }
}

