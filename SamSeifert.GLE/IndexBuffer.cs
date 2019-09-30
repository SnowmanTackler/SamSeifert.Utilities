using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using SamSeifert.Utilities.Maths;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class IndexBuffer : DeleteableObject
    {
        private int BufferID = 0;
        private int Length = 0;

        public IndexBuffer(uint[] indices)
        {
            int bufferSize;
            int bufferSizeE = indices.Length * sizeof(uint);
            GL.GenBuffers(1, out this.BufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.BufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            if (bufferSizeE == bufferSize)
            {
                this.Length = indices.Length;
            }
            else
            {
                this.GLDelete();
            }
        }

        public void GLDelete()
        {
            if (this.BufferID != 0)
            {
                GL.DeleteBuffer(this.BufferID);
                this.BufferID = 0;
                this.Length = 0;
            }
        }

        public void Draw(PrimitiveType pt)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.BufferID);
            GL.DrawElements(pt, this.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public bool IsSetup
        {
            get
            {
                return this.BufferID != 0;
            }
        }
    }
}
