using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public class ColorGL
    {
        public float[] _Ambient = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Diffuse = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Emission = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Specular = new float[] { 0f, 0f, 0f, 1.0f };        
        public float[] _Shininess = new float[] { 0 }; // max 128

        public ColorGL()
        {
        }

        public ColorGL(System.Drawing.Color c)
        {
            this.setColor(c);
        }

        public ColorGL(float r, float g, float b)
        {
            this.setColor(r, g, b);
        }

        public void SendToGL()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, this._Ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, this._Diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, this._Emission);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, this._Specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, this._Shininess);
        }

        public void setColor(System.Drawing.Color c)
        {
            this.setColor(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
        }

        public void setColor(float r, float g, float b, float a = 0.4f, float d = 0.7f, float e = 0, float s = 0)
        {
            this._Ambient[0] = a * r;
            this._Ambient[1] = a * g;
            this._Ambient[2] = a * b;

            this._Diffuse[0] = d * r;
            this._Diffuse[1] = d * g;
            this._Diffuse[2] = d * b;

            this._Emission[0] = e * r;
            this._Emission[1] = e * g;
            this._Emission[2] = e * b;

            this._Specular[0] = s * r;
            this._Specular[1] = s * g;
            this._Specular[2] = s * b;
        }

    }
}
