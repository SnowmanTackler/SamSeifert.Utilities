
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.Utilities
{
    public static class ColorMethods
    {
        public static Boolean CheckHue(float hue, float BandCenter, float BandWidth)
        {
            hue = Helpers.ModGuaranteePositive(hue, 1.0f);
            return ColorMethods.GetDistanceWithRollOver(hue, BandCenter) < BandWidth / 2;
        }

        public static float GetDistanceWithRollOver(float v1, float v2)
        {
            return Math.Min(1 + v1 - v2, Math.Min(Math.Abs(v1 - v2), 1 + v2 - v1));
        }

        /// <summary>
        /// HSL and RGB both on scale of 0 to 1.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void hsl2rgb(float h, float s, float l, out float r, out float g, out float b)
        {
            h = 6 * Helpers.ModGuaranteePositive(h, 1);
            float c = s * (1 - Math.Abs(2 * l - 1));
            float x = c * (1 - Math.Abs(h % 2 - 1));
            float m = l - c / 2;

            float bx = Math.Min(1, Math.Max(0, (x + m)));
            float bc = Math.Min(1, Math.Max(0, (c + m)));
            float bm = Math.Min(1, Math.Max(0, m));

            switch ((int)Math.Floor(h))
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
        /// HSL on scale 0 to 1
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsl(Color c, out float h, out float s, out float l)
        {
            ColorMethods.rgb2hsl(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, out h, out s, out l);
        }

        /// <summary>
        /// HSL and RGB both on scale of 0 to 1.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsl(float r, float g, float b, out float h, out float s, out float l)
        {
            const float PIF = (float)Math.PI;
            const float PIF2 = PIF * 2;

            float hh = (float)(Math.Atan2(1.73205080757f * (g - b), 2 * r - g - b) / PIF2);
            if (hh < 0) hh = 1 + hh;

            float mx = Math.Max(r, Math.Max(g, b));
            float mn = Math.Min(r, Math.Min(g, b));

            float ll = (mx + mn) / 2;
            float delta = mx - mn;

            h = hh;
            s = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * ll - 1));
            l = ll;
        }









        /// <summary>
        /// HSV on scale of 0 to 1.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color hsv2rgb(float h, float s, float v)
        {
            Color c;
            ColorMethods.hsv2rgb(h, s, v, out c);
            return c;
        }

        /// <summary>
        /// HSV and RGB both on scale of 0 to 1.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="c"></param>
        public static void hsv2rgb(float h, float s, float v, out Color c)
        {
            float r, g, b;
            ColorMethods.hsv2rgb(h, s, v, out r, out g, out b);
            c = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void hsv2rgb(float h, float s, float v, out float r, out float g, out float b)
        {
            int i;
            float f, p, q, t;
            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
                return;
            }
            h = 6 * Helpers.ModGuaranteePositive(h, 1);
            i = (int)Math.Floor(h);
            f = h - i;          // factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:        // case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }


        /// <summary>
        /// HSV on scale of 0 to 1.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsv(Color c, out float h, out float s, out float v)
        {
            ColorMethods.rgb2hsv(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, out h, out s, out v);
        }

        /// <summary>
        /// HSV and RGB both on scale of 0 to 1.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsv(float fr, float fg, float fb, out float h, out float s, out float v)
        {
            /*            const float PIF = (float)Math.PI;
                        const float PIF2 = PIF * 2;

                        float hh = (float)(Math.Atan2(1.73205080757f * (fg - fb), 2 * fr - fg - fb) / PIF2);
                        if (hh < 0) hh = 1 + hh;

                        float mx = Math.Max(fr, Math.Max(fg, fb));
                        float mn = Math.Min(fr, Math.Min(fg, fb));


                        h = hh;
                        s = mx == 0 ? 0 : 1 - (mn / mx);
                        v = mx;*/

            float max = Math.Max(Math.Max(fr, fg), fb);
            float min = Math.Min(Math.Min(fr, fg), fb);
            float delta = max - min;


            v = max;

            if (max != 0) s = delta / max;       // s
            else
            {
                s = 0;
                h = 0;
                return;
            }

            if (fr == max)
                h = (fg - fb) / delta;       // between yellow & magenta
            else if (fg == max)
                h = 2 + (fb - fr) / delta;   // between cyan & yellow
            else
                h = 4 + (fr - fg) / delta;   // between magenta & cyan

            h /= 6;               // degrees
            if (h < 0) h += 1;
        }
    }
}
