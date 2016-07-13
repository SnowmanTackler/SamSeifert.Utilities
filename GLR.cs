using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;



namespace SamSeifert.GLE
{
    public static partial class GLR
    {
        public static IDisposable MatrixPush
        {
            get
            {
                return new MatrixPushClass();
            }
        }

        private class MatrixPushClass : IDisposable
        {
            public MatrixPushClass()
            {
                GLR.PushMatrix();
            }

            void IDisposable.Dispose()
            {
                GLR.PopMatrix();
            }
        }

        public static void DepthOff()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
        }

        public static void DepthOn()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }

    }
}
