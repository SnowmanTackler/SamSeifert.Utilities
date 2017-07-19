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

        public static float CelsiusToFahrenheit(float celsius)
        {
            return 32 + celsius * (9 / 5.0f);
        }

        public static float FeetToMeters(float feet)
        {
            return 0.3048f * feet;
        }

        public static float InchesToMeters(float inches)
        {
            return FeetToMeters(inches / 12);
        }








        public const float PIF = (float)Math.PI;

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (PIF / 180);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (180 / PIF);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static float SinDegrees(float degrees)
        {
            return (float)Math.Sin(DegreesToRadians(degrees));
        }

        public static float CosDegrees(float degrees)
        {
            return (float)Math.Cos(DegreesToRadians(degrees));
        }

        public static double SinDegrees(double degrees)
        {
            return Math.Sin(DegreesToRadians(degrees));
        }

        public static double CosDegrees(double degrees)
        {
            return Math.Cos(DegreesToRadians(degrees));
        }

    }
}
