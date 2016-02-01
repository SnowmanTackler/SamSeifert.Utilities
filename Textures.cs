
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL; using GL = SamSeifert.GLE.GLR;
using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;
using MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace;

namespace SamSeifert.GLE
{
    public class Textures
    {
        public static void BindTexture(int program, TextureUnit textureUnit, string UniformName)
        {
            GL.Uniform1(GL.GetUniformLocation(program, UniformName), textureUnit - TextureUnit.Texture0);
        }

        public static int getGLTexture(Image im)
        {
            if (im == null) return 0;
            else if (im is Bitmap) return getGLTextureBitmap(im as Bitmap);

            int w = im.Width, h = im.Height;

            Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp)) g.DrawImage(im, 0, 0, w, h);

            try
            {
                return getGLTextureBitmap(bmp);
            }
            finally
            {
                bmp.Dispose();
            }
        }


        public static int getGLTextureBitmap(Bitmap im)
        {
            if (im == null) return 0;

            int w = im.Width, h = im.Height;

            //get the data out of the bitmap
            System.Drawing.Imaging.BitmapData TextureData = im.LockBits(
                new System.Drawing.Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //Code to get the data to the OpenGL Driver

            int output;

            //generate one texture and put its ID number into the "Texture" variable
            GL.GenTextures(1, out output);
            //tell OpenGL that this is a 2D texture
            GL.BindTexture(TextureTarget.Texture2D, output);

            //the following code sets certian parameters for the texture
            GL.TexEnv(TextureEnvTarget.TextureEnv,
                    TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);

            // tell OpenGL to build mipmaps out of the bitmap data
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (float)1.0f);

            // load the texture
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0, // level
                PixelInternalFormat.Three,
                w, h,
                0, // border
                OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                PixelType.UnsignedByte,
                TextureData.Scan0
                );

            //free the bitmap data (we dont need it anymore because it has been passed to the OpenGL driver
            im.UnlockBits(TextureData);

            return output;
        }
    }
}
