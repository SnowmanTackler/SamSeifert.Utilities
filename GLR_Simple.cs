using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace SamSeifert.GLE
{
    public static partial class GLR
    {
        public static void ActiveTexture(TextureUnit tu)
        {
            GL.ActiveTexture(tu);
        }

        public static void Color4(float red, float green, float blue, float alpha)
        {
            GL.Color4(red, green, blue, alpha);
        }

        public static void ClearColor(Color c)
        {
            GL.ClearColor(c);
        }

        public static void ClearColor(float v1, float v2, float v3, float v4)
        {
            GL.ClearColor(v1, v2, v3, v4);
        }

        public static void DeleteBuffer(int id)
        {
            GL.DeleteBuffer(id);
        }

        public static void DepthFunc(DepthFunction lequal)
        {
            GL.DepthFunc(lequal);
        }
        public static void DisableClientState(ArrayCap ac)
        {
            GL.DisableClientState(ac);
        }

        public static void DrawBuffer(DrawBufferMode dbm)
        {
            GL.DrawBuffer(dbm);
        }

        public static void DrawArrays(PrimitiveType pt, int first, int count)
        {
            GL.DrawArrays(pt, first, count);
        }


        public static void Flush()
        {
            GL.Flush();
        }

        public static FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget fbt)
        {
            return GL.CheckFramebufferStatus(fbt);
        }

        public static void GenFramebuffers(int v, out int id)
        {
            GL.GenFramebuffers(v, out id);
        }

        public static void GetBufferParameter(BufferTarget bt, BufferParameterName bpn, out int p)
        {
            GL.GetBufferParameter(bt, bpn, out p);
        }

        public static void Hint(HintTarget ht, HintMode hm)
        {
            GL.Hint(ht, hm);
        }

        public static void Viewport(Rectangle rectangle)
        {
            GL.Viewport(rectangle);
        }

        public static int GetUniformLocation(int program, string uniformName)
        {
            return GL.GetUniformLocation(program, uniformName);
        }

        public static void Uniform1(int location, int v)
        {
            GL.Uniform1(location, v);
        }

        public static void DeleteShader(int fs)
        {
            GL.DeleteShader(fs);
        }

        public static int CreateProgram()
        {
            return GL.CreateProgram();
        }

        public static void AttachShader(int prog, int shader)
        {
            GL.AttachShader(prog, shader);
        }

        public static void LinkProgram(int shader)
        {
            GL.LinkProgram(shader);
        }

        public static void GetProgram(int shader, ProgramParameter validateStatus, out int param)
        {
            GL.GetProgram(shader, validateStatus, out param);
        }

        public static void ValidateProgram(int program)
        {
            GL.ValidateProgram(program);
        }

        public static void CullFace(CullFaceMode back)
        {
            GL.CullFace(back);
        }

        public static int CreateShader(ShaderType vertexShader)
        {
            return GL.CreateShader(vertexShader);
        }

        public static void GetShader(int fragShader, ShaderParameter compileStatus, out int param)
        {
            GL.GetShader(fragShader, compileStatus, out param);
        }

        public static void ShaderSource(int vertShader, string vertexCode)
        {
            GL.ShaderSource(vertShader, vertexCode);
        }

        public static void CompileShader(int vertShader)
        {
            GL.CompileShader(vertShader);
        }

        public static void DrawElements(PrimitiveType pt, int v1, DrawElementsType det, int v2)
        {
            GL.DrawElements(pt, v1, det, v2);
        }

        public static void UseProgram(int program)
        {
            GL.UseProgram(program);
        }

        public static void Color3(float v1, float v2, float v3)
        {
            GL.Color3(v1, v2, v3);
        }

        public static void Color3(Vector3 vector3)
        {
            GL.Color3(vector3);
        }

        public static void DeleteLists(int list, int range)
        {
            GL.DeleteLists(list, range);
        }

        public static void EnableClientState(ArrayCap ac)
        {
            GL.EnableClientState(ac);
        }

        public static void NormalPointer(NormalPointerType npt, int stride, int v)
        {
            GL.NormalPointer(npt, stride, v);
        }

        public static int GenLists(int v)
        {
            return GL.GenLists(v);
        }

        public static void Vertex2(float v1, float v2)
        {
            GL.Vertex2(v1, v2);
        }

        public static void Vertex2(Vector2 v1)
        {
            GL.Vertex2(v1);
        }

        public static void NewList(int ls, ListMode lm)
        {
            GL.NewList(ls, lm);
        }

        public static int GenFramebuffer()
        {
            return GL.GenFramebuffer();
        }

        public static int GenRenderbuffer()
        {
            return GL.GenRenderbuffer();
        }

        public static void Color3(System.Drawing.Color white)
        {
            GL.Color3(white);
        }

        public static void Vertex3(float x, float y, float z)
        {
            GL.Vertex3(x, y, z);
        }

        public static void Vertex3(Vector3 v)
        {
            GLR.Vertex3(v.X, v.Y, v.Z);
        }

        public static void Normal3(float x, float y, float z)
        {
            GL.Normal3(x, y, z);
        }

        public static void Normal3(Vector3 v)
        {
            GL.Normal3(v);
        }


        public static void BlendFunc(BlendingFactorSrc src, BlendingFactorDest dst)
        {
            GL.BlendFunc(src, dst);
        }

        public static void TexCoord2(int v1, int v2)
        {
            GL.TexCoord2(v1, v2);
        }

        public static void TexCoord2(float v1, float v2)
        {
            GL.TexCoord2(v1, v2);
        }

        public static void Begin(PrimitiveType pt)
        {
            GL.Begin(pt);
        }

        public static void End()
        {
            GL.End();
        }

        public static void DrawRangeElements(PrimitiveType pt, int v1, int v2, int v3, DrawElementsType det, IntPtr ip)
        {
            GL.DrawRangeElements(pt, v1, v2, v3, det, ip);
        }

        public static void DeleteTexture(int tex)
        {
            GL.DeleteTexture(tex);
        }

        public static void Material(MaterialFace mf, MaterialParameter mp, float[] nums)
        {
            GL.Material(mf, mp, nums);
        }

        public static void DeleteBuffers(int v, ref int index)
        {
            GL.DeleteBuffers(v, ref index);
        }

        public static void BindBuffer(BufferTarget bt, int dex)
        {
            GL.BindBuffer(bt, dex);
        }

        public static void Viewport(int x, int y, int w, int h)
        {
            GL.Viewport(x, y, w, h);
        }

        public static void VertexPointer(int size, VertexPointerType vpt, int stride, IntPtr zero)
        {
            GL.VertexPointer(size, vpt, stride, zero);
        }

        public static void Clear(ClearBufferMask cbm)
        {
            GL.Clear(cbm);
        }

        public static void Light(LightName light0, LightParameter position, float[] v)
        {
            GL.Light(light0, position, v);
        }

        public static void Disable(EnableCap ec)
        {
            GL.Disable(ec);
        }

        public static void Enable(EnableCap ec)
        {
            GL.Enable(ec);
        }

        public static void LineWidth(float v)
        {
            GL.LineWidth(v);
        }

        public static void GenBuffers(int v, out int id)
        {
            GL.GenBuffers(v, out id);
        }

        public static void BufferData(BufferTarget bt, IntPtr ip, Vector3[] dat, BufferUsageHint buh)
        {
            GL.BufferData(bt, ip, dat, buh);
        }

        public static void BufferData(BufferTarget bt, IntPtr ip, uint[] dat, BufferUsageHint buh)
        {
            GL.BufferData(bt, ip, dat, buh);
        }

        public static void BufferSubData(BufferTarget target, IntPtr offset, int size, IntPtr data)
        {
            GL.BufferSubData(target, offset, size, data);
        }


        public static void BindTexture(TextureTarget tt, int dex)
        {
            GL.BindTexture(tt, dex);
        }

        public static void TexCoordPointer(int v, TexCoordPointerType tcpf, int stride, int sizeInBytes)
        {
            GL.TexCoordPointer(v, tcpf, stride, sizeInBytes);
        }

        public static void DrawElements(PrimitiveType pt, int cnt, DrawElementsType det, IntPtr ip)
        {
            GL.DrawElements(pt, cnt, det, ip);
        }

        public static void EndList()
        {
            GL.EndList();
        }

        public static void CallList(int _List)
        {
            GL.CallList(_List);
        }

        public static void GenTextures(int v, out int output)
        {
            GL.GenTextures(v, out output);
        }

        internal static int GenTexture()
        {
            return GL.GenTexture();
        }


        public static void TexParameterI(TextureTarget tt, TextureParameterName tpn, ref int param)
        {
            GL.TexParameterI(tt, tpn, ref param);
        }

        public static void TexParameter(TextureTarget tt, TextureParameterName tpn, float param)
        {
            GL.TexParameter(tt, tpn, param);
        }

        public static void TexEnv(TextureEnvTarget tet, TextureEnvParameter tep, float param)
        {
            GL.TexEnv(tet, tep, param);
        }

        public static void TexImage2D(
            TextureTarget tt,
            int level,
            PixelInternalFormat pif,
            int width,
            int height,
            int border,
            PixelFormat pf,
            PixelType pt,
            IntPtr ip)
        {
            GL.TexImage2D(tt, level, pif, width, height, border, pf, pt, ip);
        }

        public static void TexImage2D<T8>(
            OpenTK.Graphics.OpenGL.TextureTarget target, 
            Int32 level, 
            OpenTK.Graphics.OpenGL.PixelInternalFormat internalformat,
            Int32 width,
            Int32 height, 
            Int32 border,
            OpenTK.Graphics.OpenGL.PixelFormat format, 
            OpenTK.Graphics.OpenGL.PixelType type, 
            [InAttribute, OutAttribute] T8[] pixels)
            where T8 : struct
        {
            GL.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels);

        }

        public static void FramebufferTexture2D(
            FramebufferTarget fbt,
            FramebufferAttachment fba, 
            TextureTarget tt,
            int texture, 
            int level)
        {
            GL.FramebufferTexture2D(
                fbt,
                fba,
                tt,
                texture,
                level);
        }

        public static void ReadBuffer(ReadBufferMode none)
        {
            GL.ReadBuffer(none);
        }

        public static void BindFramebuffer(FramebufferTarget target, int framebuffer)
        {
            GL.BindFramebuffer(target, framebuffer);
        }

        public static void ColorMask(bool v1, bool v2, bool v3, bool v4)
        {
            GL.ColorMask(v1, v2, v3, v4);
        }

        public static void GetTexImage<T>(TextureTarget target, int level, PixelFormat format, PixelType type, T[] pixels)
            where T : struct
        {
            GL.GetTexImage<T>(target, level, format, type, pixels);
        }

        public static void ColorPointer(int size, ColorPointerType type, int stride, int offset)
        {
            GL.ColorPointer(size, type, stride, offset);
        }

        public static void DeleteFramebuffer(int id)
        {
            GL.DeleteFramebuffer(id);
        }

        public static void DeleteRenderbuffer(int buffer)
        {
            GL.DeleteRenderbuffer(buffer);
        }

        public static void Color4(Color color)
        {
            GL.Color4(color);
        }

        public static void DepthMask(bool flag)
        {
            GL.DepthMask(flag);
        }

        public static void ClearColor(Vector3 color)
        {
            GL.ClearColor(color.X, color.Y, color.Z, 0);
        }
    }
}
