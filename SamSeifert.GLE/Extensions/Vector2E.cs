using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE.Extensions
{
    public static class Vector2E
    {
        public static Vector2 Blend01(this float alpha, Vector2 value_at_0, Vector2 value_at_1)
        {
            return alpha * value_at_1 + (1 - alpha) * value_at_0;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
