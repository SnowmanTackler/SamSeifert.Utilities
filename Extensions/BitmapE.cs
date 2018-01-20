using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Extensions
{
    public static class BitmapE
    {
        public class LockedBits : IDisposable
        {
            private readonly Bitmap _Bitmap;
            private readonly BitmapData _Data;

            internal LockedBits(Bitmap b, BitmapData bd)
            {
                this._Bitmap = b;
                this._Data = bd;
            }

            public void Dispose()
            {
                this._Bitmap.UnlockBits(this._Data);
            }

            public IntPtr Scan0
            {
                get
                {
                    return this._Data.Scan0;
                }
            }

            public int Stride
            {
                get
                {
                    return this._Data.Stride;
                }
            }

            public int Height { get { return this._Data.Height; } }
            public int Width { get { return this._Data.Width; } }
        }

        public static LockedBits Locked(this Bitmap b, Rectangle rect, ImageLockMode flags, PixelFormat format)
        {
            return new LockedBits(b, b.LockBits(rect, flags, format));
        }

        public static LockedBits Locked(this Bitmap b, ImageLockMode flags, PixelFormat format)
        {
            return b.Locked(new Rectangle(0, 0, b.Width, b.Height), flags, format);
        }

    }
}
