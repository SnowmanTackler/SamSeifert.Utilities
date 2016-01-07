using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class DoubleBufferedForm : Form
    {
        private Bitmap _Bitmap;
        private Graphics _Graphics;
        private PaintEventArgs _PaintEventArgs;

        public DoubleBufferedForm() : base()
        {
            this.OnResize(EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (this._Bitmap != null)
            {
                this._PaintEventArgs.Dispose();
                this._Graphics.Dispose();
                this._Bitmap.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (this.DesignMode) return;

            if (this._Bitmap != null)
            {
                if (this._Bitmap.Size == this.ClientSize) return;
                this._PaintEventArgs.Dispose();
                this._Graphics.Dispose();
                this._Bitmap.Dispose();
            }

            this._Bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Format24bppRgb);
            this._Graphics = Graphics.FromImage(this._Bitmap);
            this._PaintEventArgs = new PaintEventArgs(this._Graphics, new Rectangle(
                0,
                0,
                this.ClientSize.Width,
                this.ClientSize.Height));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.DesignMode) return;
            this._Graphics.ResetClip();
            this._Graphics.ResetTransform();
            base.OnPaintBackground(this._PaintEventArgs);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode) return;
            this._Graphics.ResetClip();
            this._Graphics.ResetTransform();
            base.OnPaint(this._PaintEventArgs);
            e.Graphics.DrawImage(this._Bitmap, 0, 0);
        }
    }
}
