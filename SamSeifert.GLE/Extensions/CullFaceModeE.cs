using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE.Extensions
{
    public static class CullFaceModeE
    {
        public static void sendToGL(this CullFaceMode cfm)
        {
            GL.CullFace(cfm);
        }

        public static void sendToGL(this CullFaceMode? cfm)
        {
            if (cfm == null)
            {
                GL.Disable(EnableCap.CullFace);
            }
            else
            {
                GL.Enable(EnableCap.CullFace);                
                GL.CullFace(cfm.Value);
            }
        }
    }
}
