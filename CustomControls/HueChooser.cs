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
    public delegate void ValueChangedEventHandler(object sender, EventArgs e);

    public partial class HueChooser : UserControl
    {
        const int nudScale1 = 360;
        const int nudScale2 = 720;

        public float HueBandWidth { get; private set; }
        public float HueBandCenter { get; private set; }
        public bool GrayBack { get; private set; }

        public event ValueChangedEventHandler ValueChanged;

        public HueChooser(ColorFilterOptions c)
        {
            InitializeComponent();
            this.setInitialValues(c);
        }

        public HueChooser()
        {
            InitializeComponent();
            this.setInitialValues(0.5f, 0.25f, true);
        }

        public void setInitialValues(ColorFilterOptions c)
        {
            this.setInitialValues(c.HueBandCenter, c.HueBandWidth, c.GrayBack);
        }

        public void setInitialValues(float BandCenter, float BandWidth, bool GrayBack)
        {
            this.nudBandCenter.Value = (Decimal)(BandCenter * nudScale1);
            this.nudBandWidth.Value = (Decimal)(BandWidth * nudScale2);
            this.checkBox1.Checked = GrayBack;
            this.changed(null, EventArgs.Empty);
        }



        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            this.setImageForControl();
        }

        private Bitmap lastImage = null;
        private void setImageForControl()
        {
            int w = this.pictureBox1.Width;
            int h = this.pictureBox1.Height;

            if (this.lastImage == null || this.lastImage.Width != w || this.lastImage.Height != h)
            {
                this.pictureBox1.Image = null;
                if (this.lastImage != null) this.lastImage.Dispose();
                this.lastImage = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            }

            unsafe
            {
                BitmapData bmdNew = this.lastImage.LockBits(
                    new Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, this.lastImage.PixelFormat);

                byte * rowNew;
                int y, x, xx;

                float wD = (float)(w - 1);
                float hD = (float)(h - 1);
                float hue, lum;
                Byte b;

                for (y = 0; y < h; y++)
                {
                    rowNew = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                    lum = 1 - y / hD;

                    for (x = 0, xx = 0; x < w; x++, xx += 3)
                    {
                        hue = x / wD;

                        HueChooser.hsl2rgb(hue, 0.5f, lum, out rowNew[xx + 2], out rowNew[xx + 1], out rowNew[xx + 0]);

                        if (this.checkHue(hue))
                        {
                            if (this.GrayBack) b = (Byte)((rowNew[xx + 2] + rowNew[xx + 1] + rowNew[xx + 0]) / 3);
                            else b = 0;
                            rowNew[xx + 2] = b;
                            rowNew[xx + 1] = b;
                            rowNew[xx + 0] = b;
                        }
                    }
                }

                this.lastImage.UnlockBits(bmdNew);
            }

            this.pictureBox1.Image = this.lastImage;
        }













        public class ColorFilterOptions
        {
            public float HueBandCenter;
            public float HueBandWidth;
            public Boolean GrayBack;

            public ColorFilterOptions(float HueBandCenter, float HueBandWidth, Boolean GrayBack)
            {
                this.HueBandCenter = HueBandCenter;
                this.HueBandWidth = HueBandWidth;
                this.GrayBack = GrayBack;
            }
        }

        private Boolean checkHue(float hue)
        {
            return HueChooser.checkHue(hue, this.HueBandCenter, this.HueBandWidth);
        }

        private static Boolean checkHue(float hue, float cent, float thresh)
        {
            return Math.Min(1 + cent - hue, Math.Min(Math.Abs(cent - hue), 1 + hue - cent)) > thresh;
        }

        public static void filterColor(ref float r, ref float g, ref float b, ColorFilterOptions c)
        {
            HueChooser.filterColor(ref r, ref g, ref b, c.HueBandCenter, c.HueBandWidth, c.GrayBack);
        }

        public static void filterColor(ref float r, ref float g, ref float b, float BandCenter, float BandWidth, bool GrayBack)
        {            
            r = Math.Max(0, Math.Min(1, r));
            g = Math.Max(0, Math.Min(1, g));
            b = Math.Max(0, Math.Min(1, b));

            double rIn = r;
            double gIn = g;
            double bIn = b;

            float hue = (float)(Math.Atan2(1.73205080757d * (gIn - bIn), 2 * rIn - gIn - bIn) / (2 * Math.PI));

            if (HueChooser.checkHue(hue, BandCenter, BandWidth))
            {
                float f = GrayBack ? (r + g + b) / 3 : 0;
                r = f;
                g = f;
                b = f;
            }
        }

        public void filterColor(ref float r, ref float g, ref float b)
        {
            HueChooser.filterColor(ref r, ref g, ref b, this.HueBandCenter, this.HueBandWidth, this.GrayBack);
        }







       public static Color hsl2color(float h, float s, float l)
       {
           Byte R, G, B;
           HueChooser.hsl2rgb(h, s, l, out R, out G, out B);
           return Color.FromArgb(255, R, G, B);
       }

        /// <summary>
        /// HSL all on scale of 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void hsl2rgb(float h, float s, float l, out Byte r, out Byte g, out Byte b)
        {
            h *= 6;
            float c = s * (1 - Math.Abs(2 * l - 1));
            float x = c * (1 - Math.Abs(h % 2 - 1));
            float m = l - c / 2;

            Byte bx = (Byte)Math.Min(255, Math.Max(0, 255 * (x + m)));
            Byte bc = (Byte)Math.Min(255, Math.Max(0, 255 * (c + m)));
            Byte bm = (Byte)Math.Min(255, Math.Max(0, 255 * m));

            switch ((int)h)
            {
                case 0:
                    {
                        r = bc;
                        g = bx;
                        b = bm;
                        break;
                    }
                case 1:
                    {
                        r = bx;
                        g = bc;
                        b = bm;
                        break;
                    }
                case 2:
                    {
                        r = bm;
                        g = bc;
                        b = bx;
                        break;
                    }
                case 3:
                    {
                        r = bm;
                        g = bx;
                        b = bc;
                        break;
                    }
                case 4:
                    {
                        r = bx;
                        g = bm;
                        b = bc;
                        break;
                    }
                default:
                    {
                        r = bc;
                        g = bm;
                        b = bx;
                        break;
                    }
            }
        }

        /// <summary>
        /// HSL Scale 0 to 1
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        public static void rgb2hsl(Byte r, Byte g, Byte b, out float h, out float s, out float l)
        {
            double rIn = r / 255d;
            double gIn = g / 255d;
            double bIn = b / 255d;

            double max = Math.Max(Math.Max(rIn, gIn), bIn);
            double min = Math.Min(Math.Min(rIn, gIn), bIn);
            double delta = max - min;
            double L = (max + min) / 2;

            double H = (Math.Atan2(1.73205080757d * (gIn - bIn), 2 * rIn - gIn - bIn) / (2 * Math.PI));

            double denom = 1 - Math.Abs(2 * L - 1);

            double S = denom == 0 ? 0.0 : delta / denom;

            s = (float)(Math.Max(Math.Min(1, S), 0));
            l = (float)(Math.Max(Math.Min(1, L), 0));
            h = (float)(Math.Max(Math.Min(1, H), 0));
        }

        /// <summary>
        /// HSL Scale 0 to 1
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        public static void rgb2hsv(Byte r, Byte g, Byte b, out float h, out float s, out float v)
        {
            double rIn = r / 255d;
            double gIn = g / 255d;
            double bIn = b / 255d;

            double max = Math.Max(Math.Max(rIn, gIn), bIn);
            double min = Math.Min(Math.Min(rIn, gIn), bIn);
            double delta = max - min;

            double H = (Math.Atan2(1.73205080757d * (gIn - bIn), 2 * rIn - gIn - bIn) / (2 * Math.PI));

            double S = delta == 0 ? 0.0 : delta / max;

            s = (float)(Math.Max(Math.Min(1, S), 0));
            h = (float)(Math.Max(Math.Min(1, H), 0));
            v = (float)(Math.Max(Math.Min(1, max), 0));
        }








        private bool changed_block = true;
        private void changed(object sender, EventArgs e)
        {
            if (sender == null) changed_block = false;
            if (changed_block) return;

            if (this.nudBandCenter.Value == -1)
            {
                this.nudBandCenter.Value = 359;
                return;
            }

            if (this.nudBandCenter.Value == 360)
            {
                this.nudBandCenter.Value = 0;
                return;
            }

            this.GrayBack = this.checkBox1.Checked;
            this.HueBandCenter = (float)(this.nudBandCenter.Value / nudScale1);
            this.HueBandWidth = (float)(this.nudBandWidth.Value / nudScale2);

            this.setImageForControl();
            if (this.ValueChanged != null) this.ValueChanged(this, EventArgs.Empty);
        }

        private void HueChooser_BackColorChanged(object sender, EventArgs e)
        {
            this.checkBox1.BackColor = this.BackColor;
        }

        private void HueChooser_Resize(object sender, EventArgs e)
        {
            this.setImageForControl();
        }
    }
}
