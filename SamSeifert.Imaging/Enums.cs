using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public enum SectType
    {
        RGB_R,
        RGB_G,
        RGB_B,

        Gray,

        HSL_H,
        HSL_S,
        HSL_L,

        HSV_H,
        HSV_S,
        HSV_V,

        Hough_Foot_Of_Normal,
        Hough_Rho_Theta,

        NaN,
        Holder
    };

    public enum ResizeType
    { 
        NearestNeighbor,
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
