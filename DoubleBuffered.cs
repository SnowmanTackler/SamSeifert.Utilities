using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.Utilities
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel() : base()
        {
        }

        private Bitmap _Bitmap;
        private Graphics _Graphics;

        protected override void OnResize(EventArgs eventargs)
        {
            if (this.DesignMode) return;

            base.OnResize(eventargs);

            if (this._Bitmap == null)
            {
                if (this._Bitmap.Size == base.Size) return;
                this._Graphics.Dispose();
                this._Bitmap.Dispose();
            }

            this._Bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format24bppRgb);
            this._Graphics = Graphics.FromImage(this._Bitmap);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
//            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode) return;

            base.OnPaint(new PaintEventArgs(this._Graphics,
                new Rectangle(0, 0, this._Bitmap.Width, this._Bitmap.Height)));

            e.Graphics.DrawImage(this._Bitmap, 0, 0);
        }
    }

    public partial class DoubleBufferedForm : Form
    {
        public DoubleBufferedForm() : base()
        {
            this.Load += ControlLoaded;
        }

        private void ControlLoaded(object sender, EventArgs e)
        {
            this.updateBitmap();
        }

        private Bitmap _Bitmap;
        private Graphics _Graphics;
        private PaintEventArgs _PaintEventArgs;

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
        }

        private void updateBitmap()
        {
            if (this._Bitmap != null) this._Bitmap.Dispose();
            this._Bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            this._Graphics = Graphics.FromImage(this._Bitmap);
            this._PaintEventArgs = new PaintEventArgs(this._Graphics, new Rectangle(
                0,
                0,
                this.ClientSize.Width,
                this.ClientSize.Height));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this._Graphics.ResetClip();
            this._Graphics.ResetTransform();
            base.OnPaint(this._PaintEventArgs);
            e.Graphics.DrawImage(this._Bitmap, 0, 0);
        }
    }
}
