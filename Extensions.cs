using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace SamSeifert.GLE
{
    public static class Extensions
    {

        public static Vector2 RotateClockwise(this Vector2 v, float angle_degrees)
        {
            float ct = SamSeifert.Utilities.UnitConverter.CosDegrees(angle_degrees);
            float st = SamSeifert.Utilities.UnitConverter.SinDegrees(angle_degrees);
            return new Vector2(
                v.X * ct - v.Y * st,
                v.Y * ct + v.X * st
                );
        }

        public static Vector2 RotateCounterClockwise(this Vector2 v, float angle_degrees)
        {
            return (v.RotateClockwise(-angle_degrees));
        }

        public static Vector2 asDirectionFromRadians(this float angle)
        {
            return new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));
        }

    }
}
