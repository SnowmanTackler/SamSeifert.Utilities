﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class MathUtil
    {
        public static int ModGuaranteePositive(int x, int mod)
        {
            return (x % mod + mod) % mod;
        }

        public static float ModGuaranteePositive(float x, float mod)
        {
            return (x % mod + mod) % mod;
        }

        public const float PIF = (float)Math.PI;

        public static float toRadiansf(float degrees)
        {
            return degrees * (MathUtil.PIF / 180);
        }

        public static float toDegreesf(float radians)
        {
            return radians * (180 / MathUtil.PIF);
        }

        public static double toRadiansd(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double toDegreesd(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static float SinDf(float degrees)
        {
            return (float)Math.Sin(MathUtil.toRadiansf(degrees));
        }

        public static float CosDf(float degrees)
        {
            return (float)Math.Cos(MathUtil.toRadiansf(degrees));
        }

        public static float SinDf(double degrees)
        {
            return (float)Math.Sin(MathUtil.toRadiansd(degrees));
        }

        public static float CosDf(double degrees)
        {
            return (float)Math.Cos(MathUtil.toRadiansd(degrees));
        }

        public static float Clamp(int min, int max, int val)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clamp(float min, float max, float val)
        {
            return Math.Min(max, Math.Max(min, val));
        }
    }
}

