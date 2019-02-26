using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.GLE
{
    public partial class GLR
    {
        private static Vector4 __ClippingPlane0 = Vector4.Zero;
        public static Vector4 _ClippingPlane0
        {
            get
            {
                return GLR.__ClippingPlane0;
            }
            set
            {
                GLR.ClipPlane(OpenTK.Graphics.OpenGL.ClipPlaneName.ClipDistance0, ref value);
                GLR.__ClippingPlane0 = value;
            }
        }

    }
}
