using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace SamSeifert.CSCV
{
    public enum SectType
    {
        [Description("NULL")]
        NaN,

        [Description("Red")]
        RGB_R,
        [Description("Green")]
        RGB_G,
        [Description("Blue")]
        RGB_B,

        [Description("Gray")]
        Gray,

        [Description("Hue")]
        Hue,

        [Description("HSL Saturation")]
        HSL_S,
        [Description("HSL Ligtness")]
        HSL_L,

        [Description("HSV Saturation")]
        HSV_S,
        [Description("HSV Value")]
        HSV_V,

        Hough_Foot_Of_Normal,
        Hough_Rho_Theta,

        [Description("Holder")]
        Holder
    };

    public enum ResizeType
    {
        [Description("Nearest Neighbor")]
        NearestNeighbor,

        [Description("Bilinear")]
        Bilinear
    };

    public enum NoiseType 
    {
        Gaussian,
        Uniform,
        SaltAndPepper
    };

    public enum ColorMapType 
    { 
        Cold_Hot, 
        Hue
    }

    public enum GrayScaleType
    { 
        Mean, 
        Maximum, 
        Minimum 
    };

}
