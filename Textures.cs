
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using PF = System.Drawing.Imaging.PixelFormat;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;

using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
using MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace;
using SamSeifert.Utilities.Extensions;

namespace SamSeifert.GLE
{
    public class Textures : DeleteableObject
    {
        public static void BindTexture(int program, TextureUnit textureUnit, string UniformName)
        {
            GL.Uniform1(GL.GetUniformLocation(program, UniformName), textureUnit - TextureUnit.Texture0);
        }

        private static int GetGLTexture(Image im)
        {
            int w = im.Width, h = im.Height;

            Bitmap bmp = new Bitmap(w, h, PF.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp)) g.DrawImage(im, 0, 0, w, h);

            try
            {
                return GetGLTextureBitmap(bmp, PF.Format24bppRgb);
            }
            finally
            {
                bmp.Dispose();
            }
        }


        private static int GetGLTextureBitmap(
            Bitmap im,
            PF format,
            bool mipmap = true
            )
        {
            int w = im.Width, h = im.Height;

            PixelFormat glpf;
            PixelInternalFormat pif;

            switch (format)
            {
                case PF.Format24bppRgb:
                    pif = PixelInternalFormat.Three;
                    glpf = PixelFormat.Bgr;
                    break;
                case PF.Format32bppArgb:
                    pif = PixelInternalFormat.Four;
                    glpf = PixelFormat.Bgra;
                    break;
                default:
                    throw new NotImplementedException();
            }

            int output;

            //get the data out of the bitmap
            using (var TextureData = im.Locked(
               System.Drawing.Imaging.ImageLockMode.ReadOnly,
               format))
            {
                //Code to get the data to the OpenGL Driver

                //generate one texture and put its ID number into the "Texture" variable
                GL.GenTextures(1, out output);
                //tell OpenGL that this is a 2D texture
                GL.BindTexture(TextureTarget.Texture2D, output);

                //the following code sets certian parameters for the texture
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);

                // tell OpenGL to build mipmaps out of the bitmap data
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, mipmap ? 1.0f : 0.0f);

                // load the texture
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0, // level
                    pif,
                    w,
                    h,
                    0, // border
                    glpf,
                    PixelType.UnsignedByte,
                    TextureData.Scan0
                    );

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            return output;
        }

        public int _Int { get; private set; } = 0;
        public readonly Size _Size;

        public Textures(out bool success, Image im, PF pf)
        {
            if (im == null) success = false;
            else
            {
                if (im is Bitmap) this._Int = GetGLTextureBitmap(im as Bitmap, pf);
                else this._Int = GetGLTexture(im);
                this._Size = im.Size;
                success = this._Int != 0;
            }
        }


        public void GLDelete()
        {
            if (this._Int != 0)
            {
                GL.DeleteTexture(this._Int);
                this._Int = 0;
            }
        }
    }
}
