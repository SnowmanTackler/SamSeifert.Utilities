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

        [Description("Saturation")]
        HSL_S,
        [Description("Ligtness")]
        HSL_L,

        [Description("Saturation")]
        HSV_S,
        [Description("Value")]
        HSV_V,

        Hough_Foot_Of_Normal,
        Hough_Rho_Theta,

        [Description("Holder")]
        Holder
    };

    public enum ColorMapType 
    { 
        Cold_Hot, 
        Hue
    }


}
