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
    }
}
