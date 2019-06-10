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
    public class IndicesBuffer : DeleteableObject
    {
        private int BufferID = 0;
        private int Length = 0;

        public IndicesBuffer(uint[] indices)
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

        public static Vector3[] Interleave(IList<Vector3> first, IList<Vector3> second)
        {
            int lens = first.Count;
            lens.AssertEquals(second.Count);        
            var ret = new Vector3[lens * 2];
            int index = 0;
            for (int i = 0; i < lens; i++)
            {
                ret[index++] = first[i];
                ret[index++] = second[i];
            }
            return ret;
        }
    }
}
