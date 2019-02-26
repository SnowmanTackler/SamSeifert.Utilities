using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class IndicesBuffer : DeleteableObject
    {
        private int _Int = 0;
        public readonly int _Count = 0;

        public IndicesBuffer(out bool success, uint[] indices)
        {
            int bufferSize;
            int bufferSizeE = indices.Length * sizeof(uint);
            GL.GenBuffers(1, out this._Int);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._Int);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            success = bufferSizeE == bufferSize;
            this._Count = indices.Length;
        }

        public void GLDelete()
        {
            if (this._Int != 0)
            {
                GL.DeleteBuffer(this._Int);
                this._Int = 0;
            }
        }

        public void Draw(PrimitiveType pt)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._Int);
            GL.DrawElements(pt, this._Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }

    public class VerticesBuffer : DeleteableObject
    {
        private int _Int = 0;
        public readonly int _Count = 0;

        public VerticesBuffer(out bool success, Vector3[] vertices)
        {
            int bufferSize;
            int bufferSizeE = vertices.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._Int);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._Int);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            success = bufferSizeE == bufferSize;
            this._Count = vertices.Length;
        }

        public void GLDelete()
        {
            if (this._Int != 0)
            {
                GL.DeleteBuffer(this._Int);
                this._Int = 0;
            }
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._Int);
        }
    }
}
