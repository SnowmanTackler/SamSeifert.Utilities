using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SamSeifert.GLE
{
    public static class Extensions
    {
/*        public static Quaternion QuaternionFromEulerAnglesDegrees(float heading, float attitude, float bank)
        {
            return new Quaternion(
                MathHelper.DegreesToRadians(attitude),
                MathHelper.DegreesToRadians(heading),
                MathHelper.DegreesToRadians(bank)
                );
        }*/

        public static Quaternion QuaternionFromTaitBrynDegrees(float yaw, float pitch, float roll)
        {
            
            return Quaternion.Multiply(Quaternion.Multiply(
                Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(yaw)),
                Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(pitch))),
                Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(roll)));
        }

    }
}
