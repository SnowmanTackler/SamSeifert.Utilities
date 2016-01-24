using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class HueFilter : UserControl
    {
        public float BandWidth { get; private set; } = 0.5f;
        public float BandCenter { get; private set; } = 0.5f;

        public void setInitialValues(float band_center, float band_width)
        {
            this.BandWidth = band_width;
            this.BandCenter = band_center;
        }

        public event EventHandler ValueChanged;

        public HueFilter()
        {
            InitializeComponent();
        }

        private void ControlLoad(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            this.setImageForControl();
        }

        private void ControlSizeChanged(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            this.setImageForControl();
        }

        private static Byte castByte(float f)
        {
            return (Byte)Math.Max(0, Math.Min(255, (int)(255 * f)));
        }

        private void setImageForControl()
        {
            int w = this.pictureBox1.Width;
            int h = this.pictureBox1.Height;

            Bitmap i;

            if (this.pictureBox1.Image == null || this.pictureBox1.Image.Width != w || this.pictureBox1.Image.Height != h)
            {
                if (this.pictureBox1.Image != null) this.pictureBox1.Image.Dispose();
                i = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                this.pictureBox1.Image = i;
            }
            else
            {
                i = this.pictureBox1.Image as Bitmap;
            }

            unsafe
            {
                BitmapData bmdNew = i.LockBits(
                    new Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, i.PixelFormat);

                byte * rowNew;
                int y, x, xx;

                float wD = (float)(w - 1);
                float hD = (float)(h - 1);
                float hue, lum;

                float r, g, b;

                for (y = 0; y < h; y++)
                {
                    rowNew = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                    lum = 1 - y / hD;

                    for (x = 0, xx = 0; x < w; x++, xx += 3)
                    {
                        hue = x / wD;

                        ColorUtil.hsl2rgb(hue, 0.5f, lum, out r, out g, out b);


                        if (this.checkHue(hue))
                        {
                            rowNew[xx + 2] = castByte(r);
                            rowNew[xx + 1] = castByte(g);
                            rowNew[xx + 0] = castByte(b);
                        }
                        else
                        {
                            Byte temp = castByte((r + g + b) / 3);
                            rowNew[xx + 2] = temp;
                            rowNew[xx + 1] = temp;
                            rowNew[xx + 0] = temp;
                        }
                    }
                }

                i.UnlockBits(bmdNew);
            }

            this.pictureBox1.Invalidate();
        }
      
        public Boolean checkHue(float hue)
        {
            return ColorUtil.CheckHue(hue, this.BandCenter, this.BandWidth);
        }



        public void setCenterNUD(NumericUpDown nud)
        {
            nud.Minimum = -.01m;
            nud.Maximum = 1;
            nud.Increment = 0.01m;
            nud.DecimalPlaces = 2;
            nud.Value = (Decimal)this.BandCenter;
            nud.ValueChanged += CenterValueChanged;
        }

        private void CenterValueChanged(object sender, EventArgs e)
        {
            var nud = sender as NumericUpDown;
            if (nud.Value == 1.0m) nud.Value = 0;
            else if (nud.Value == -.01m) nud.Value = 0.99m;
            else
            {
                this.BandCenter = (float)nud.Value;
                this.setImageForControl();
                if (this.ValueChanged != null) this.ValueChanged(this, EventArgs.Empty);
            }
        }

        public void setWidthNUD(NumericUpDown nud)
        {
            nud.Minimum = 0;
            nud.Maximum = 1;
            nud.Increment = 0.01m;
            nud.DecimalPlaces = 2;
            nud.Value = (Decimal)this.BandWidth;
            nud.ValueChanged += WidthValueChanged;
        }

        private void WidthValueChanged(object sender, EventArgs e)
        {
            var nud = sender as NumericUpDown;
            this.BandWidth = (float)nud.Value;
            this.setImageForControl();
            if (this.ValueChanged != null) this.ValueChanged(this, EventArgs.Empty);
        }

    }
}
