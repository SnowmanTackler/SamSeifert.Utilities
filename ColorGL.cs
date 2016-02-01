using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;


namespace SamSeifert.GLE
{
    public class ColorGL
    {
        private float r, g, b;
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
            this.r = r;
            this.g = g;
            this.b = b;
            this.setColorParams();
        }

        public ColorGL(Vector3 v)
            : this(v.X, v.Y, v.Z)
        {
        }

        public void setAlpha(float alpha)
        {
            this._Ambient[3] = alpha;
            this._Diffuse[3] = alpha;
            this._Emission[3] = alpha;
            this._Specular[3] = alpha;
        }

        public void sendToGL()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, this._Ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, this._Diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, this._Emission);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, this._Specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, this._Shininess);
        }

        public System.Drawing.Color Color()
        {
            return System.Drawing.Color.FromArgb(
                (int)(this.r * 255),
                (int)(this.g * 255),
                (int)(this.b * 255)
                );
        }

        public void setColor(System.Drawing.Color c)
        {
            this.r = c.R / 255.0f;
            this.g = c.G / 255.0f;
            this.b = c.B / 255.0f;
            this.setColorParams();
        }

        public static bool CheckMatch(ColorGL a, ColorGL b)
        {
            if ((a == null) && (b == null)) return true;
            else if (a == null)
            {
                for (int i = 0; i < 4; i++) if (b._Ambient[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (b._Diffuse[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (b._Emission[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (b._Specular[i] != 0) return false;
                if (b._Shininess[0] != 0) return false;
                return true;
            }
            else if (b == null)
            {
                for (int i = 0; i < 4; i++) if (a._Ambient[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (a._Diffuse[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (a._Emission[i] != 0) return false;
                for (int i = 0; i < 4; i++) if (a._Specular[i] != 0) return false;
                if (a._Shininess[0] != 0) return false;
                return true;
            }
            else if (a == b) return true;
            else
            {
                for (int i = 0; i < 4; i++) if (a._Ambient[i] != b._Ambient[i]) return false;
                for (int i = 0; i < 4; i++) if (a._Diffuse[i] != b._Diffuse[i]) return false;
                for (int i = 0; i < 4; i++) if (a._Emission[i] != b._Emission[i]) return false;
                for (int i = 0; i < 4; i++) if (a._Specular[i] != b._Specular[i]) return false;
                if (a._Shininess[0] != b._Shininess[0]) return false;
                return true;
            }
        }

        public void setColorParams(float a = 1.75f, float d = 1.75f, float e = 0, float s = 0)
        {
            this._Ambient[0] = a * this.r;
            this._Ambient[1] = a * this.g;
            this._Ambient[2] = a * this.b;

            this._Diffuse[0] = d * this.r;
            this._Diffuse[1] = d * this.g;
            this._Diffuse[2] = d * this.b;

            this._Emission[0] = e * this.r;
            this._Emission[1] = e * this.g;
            this._Emission[2] = e * this.b;

            this._Specular[0] = s * this.r;
            this._Specular[1] = s * this.g;
            this._Specular[2] = s * this.b;
        }
    }
}
