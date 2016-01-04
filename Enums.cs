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
        HSL_H,
        [Description("Sat")]
        HSL_S,
        [Description("Lum")]
        HSL_L,

        [Description("Hue")]
        HSV_H,
        [Description("Sat")]
        HSV_S,
        [Description("Value")]
        HSV_V,

        Hough_Foot_Of_Normal,
        Hough_Rho_Theta,

        [Description("Holder")]
        Holder
    };

    public static class EnumMethods
    {
        public static string GetDescription<T>(this T enumerationValue)
                    where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }

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
