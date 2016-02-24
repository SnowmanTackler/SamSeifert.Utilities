using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class UnitConverter
    {
        public static float FahrenheitToCelsius(float fahrenheit)
        {
            return (fahrenheit - 32) * (5 / 9.0f);
        }

        public static float FeetToMeters(float feet)
        {
            return 0.3048f * feet;
        }

        public static float InchesToMeters(float inches)
        {
            return FeetToMeters(inches / 12);
        }
    }
}
