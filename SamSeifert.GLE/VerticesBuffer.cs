using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using SamSeifert.Utilities;
using SamSeifert.Utilities.Maths;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class VertexBuffer : DeleteableObject
    {
        private int BufferID = 0;
        private int Length = 0;

        public VertexBuffer(Vector3[] vertices)
        {
            int bufferSize;
            int bufferSizeE = vertices.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this.BufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.BufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            if (bufferSizeE == bufferSize)
            {
                this.Length = vertices.Length;
            }
            else
            {
                Logger.WriteLine("VertexBuffer allocation error");
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

        public IDisposable Bind()
        {
            return new Binding(this.BufferID);
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



        /// <summary>
        /// Must be called after everything is bound!
        /// </summary>
        public void Draw(PrimitiveType pt = PrimitiveType.Points)
        {
            GL.DrawArrays(pt, 0, this.Length);
        }
        
        public bool IsSetup
        {
            get
            {
                return this.BufferID != 0;
            }
        }

        public class Binding : IDisposable
        {
            public Binding(int id)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                GL.EnableClientState(ArrayCap.VertexArray);
            }

            public void Dispose()
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
    }
}
