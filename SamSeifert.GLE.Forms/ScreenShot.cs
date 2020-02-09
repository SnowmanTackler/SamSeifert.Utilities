using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace SamSeifert.GLE {
    public static class ScreenShot {
        /// <summary>
        /// Example Usage: 
        ///     GL.Finish();
        ///     GL.Flush();
        ///     glControl1.SwapBuffers();
        ///     var bmp = glControl1.TakeScreenshot();
        /// </summary>
        /// <param name="glControl"></param>
        /// <returns></returns>
        public static Bitmap TakeScreenshot(this OpenTK.GLControl glControl, Rectangle? rect = null) {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();
            int w = glControl.ClientSize.Width;
            int h = glControl.ClientSize.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(glControl.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (!rect.HasValue)
                return bmp;

            Bitmap target = new Bitmap(rect.Value.Width, rect.Value.Height);
            using (Graphics g = Graphics.FromImage(target)) {
                g.DrawImage(bmp, new Rectangle(0, 0, target.Width, target.Height),
                                 rect.Value,
                                 GraphicsUnit.Pixel);
            }

            bmp.Dispose();

            return target;
        }
    }
}
