﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLO = OpenTK.Graphics.OpenGL.GL;
using GL = SamSeifert.GLE.GLR;

namespace SamSeifert.GLE
{
    public class Shaders : DeleteableObject
    {
        public int _GL_Program { get; private set; } = 0;
        public int _GL_Vertex { get; private set; } = 0;
        public int _GL_Frag { get; private set; } = 0;

        private Dictionary<String, int> UniformLocations = new Dictionary<String, int>();
       
        public void GLDelete()
        {
            if (this._GL_Vertex != 0)
            {
                GL.DeleteShader(this._GL_Vertex);
                this._GL_Vertex = 0;
            }
            if (this._GL_Frag != 0)
            {
                GL.DeleteShader(this._GL_Frag);
                this._GL_Frag = 0;
            }
            if (this._GL_Program != 0)
            {
                GL.DeleteShader(this._GL_Program);
                this._GL_Program = 0;
            }
        }

        private int UniformLocation(String key)
        {
            int of_the_king;
            if (this.UniformLocations.TryGetValue(key, out of_the_king))
            {
                return of_the_king;
            }
            else
            {
                Console.WriteLine("Error - Shaders - No uniform for key:" + key);
                return 0;
            }
        }

        public Shaders(String vertexShader, String fragmentShader, out bool success)
        {
            try
            {
                this._GL_Vertex = createVertShader(vertexShader);
                this._GL_Frag = createFragShader(fragmentShader);

                if (this._GL_Vertex != 0 && this._GL_Frag != 0)
                {
                    int shader = GL.CreateProgram();

                    GL.AttachShader(shader, this._GL_Vertex);
                    GL.AttachShader(shader, this._GL_Frag);

                    int param;

                    GL.LinkProgram(shader);
                    GL.GetProgram(shader, ProgramParameter.LinkStatus, out param);
                    if (param == 0)
                    {
                        Console.WriteLine("Linkage error:");
                        Console.WriteLine(GLO.GetProgramInfoLog(shader));
                        success = false;
                        return;
                    }

                    GL.ValidateProgram(shader);
                    GL.GetProgram(shader, ProgramParameter.ValidateStatus, out param);
                    if (param == 0)
                    {
                        Console.WriteLine("Validate error");
                        success = false;
                        return;
                    }

                    success = true;
                    this._GL_Program = shader;
                }
                else success = false;
            }
            finally
            {
                if (this._GL_Program == 0)
                {
                    this.GLDelete();
                }
                else
                {
                    var uniforms = new HashSet<string>();
                    foreach (string s in Shaders.FindUniforms(Shaders.PurgeComments(vertexShader)))
                        uniforms.Add(s);
                    foreach (string s in Shaders.FindUniforms(Shaders.PurgeComments(fragmentShader)))
                        uniforms.Add(s);

                    foreach (String s in uniforms)
                        this.UniformLocations[s] = GL.GetUniformLocation(this._GL_Program, s);
                }

                GL.UseProgram(0);
            }
        }

        /// <summary>
        /// What active textures correspond to what uniforms
        /// </summary>
        /// <param name="uniform_name"></param>
        /// <param name="texture_index"></param>
        public void setTextureLocation(String uniform_name, int texture_index)
        {
            GL.UseProgram(this._GL_Program);
            GLO.Uniform1(this.UniformLocation(uniform_name), texture_index);
            GL.UseProgram(0);
        }

        public void Uniform(String uniform_name, float f)
        {
            GLO.Uniform1(this.UniformLocation(uniform_name), f);
        }

        public void Uniform(String uniform_name, ref Vector2 v)
        {
            GLO.Uniform2(this.UniformLocation(uniform_name), ref v);
        }

        public void Uniform(String uniform_name, ref Vector3 v)
        {
            GLO.Uniform3(this.UniformLocation(uniform_name), ref v);
        }

        public void Uniform(String uniform_name, ref Matrix2 mat)
        {
            GLO.UniformMatrix2(this.UniformLocation(uniform_name), false, ref mat);
        }

        public void Uniform(String uniform_name, ref Matrix3 mat)
        {
            GLO.UniformMatrix3(this.UniformLocation(uniform_name), false, ref mat);
        }

        public void Uniform(String uniform_name, ref Matrix4 mat)
        {
            GLO.UniformMatrix4(this.UniformLocation(uniform_name), false, ref mat);
        }





        /// <summary>
        /// If we want to use program, we can call using(this.asProgram) and this 
        /// automatically sets up the drawing and turns it off when we're done!
        /// </summary>
        public class Program : IDisposable
        {
            public Program(int program_index)
            {
                GL.UseProgram(program_index);
            }

            public void Dispose()
            {
                GL.UseProgram(0);
            }
        }


        /// <summary>
        /// Make sure you wrap this is an using()
        /// </summary>
        public Program AsProgram
        {
            get
            {
                return new Program(this._GL_Program);
            }
        }

































































        private static string PurgeComments(String s)
        {
            var sb = new StringBuilder();

            bool in_line_comment = false;
            bool in_multiline_comment = false;

            var char_array = s.ToCharArray();
            for (int i = 0; i < char_array.Length; i++)
            {
                char c = char_array[i];

                if (in_line_comment)
                {
                    switch (c)
                    {
                        case '\n':
                            in_line_comment = false;
                            sb.Append(c);
                            break;
                    }
                }
                else if (in_multiline_comment)
                {
                    if (i < char_array.Length - 1) // two char features
                    {
                        switch (c)
                        {
                            case '*':
                                switch (char_array[i + 1])
                                {
                                    case '/':
                                        i++;
                                        in_multiline_comment = false;
                                        break;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (i < char_array.Length - 1) // two char features
                    {
                        switch (c)
                        {
                            case '/':
                                switch (char_array[i + 1])
                                {
                                    case '/':
                                        i++;
                                        in_line_comment = true;
                                        break;
                                    case '*':
                                        i++;
                                        in_multiline_comment = true;
                                        break;
                                    default:
                                        sb.Append(c);
                                        break;
                                }
                                break;
                            default:
                                sb.Append(c);
                                break;
                        }
                    }
                    else sb.Append(c);
                }
            }

            String of_the_jedi = sb.ToString();

            /*
            Console.WriteLine(" CONVERT ");
            Console.WriteLine();
            Console.WriteLine(s);
            Console.WriteLine();
            Console.WriteLine(" TO ");
            Console.WriteLine();
            Console.WriteLine(of_the_jedi);
            Console.WriteLine();
            Console.WriteLine(" DONE ");
            */

            return of_the_jedi;
        }

        private static IEnumerable<string> FindUniforms(string text)
        {
            int counter = 0;

            var white_space = new char[] { ' ', '\t', '\n', ';' };

            foreach (var s in text.Split(white_space, StringSplitOptions.RemoveEmptyEntries))
            {
                var trim = s.Trim();
                if (trim.Length != 0)
                {
                    counter--;
                    switch (trim)
                    {
                        case "uniform":
                            counter = 2;
                            break;
                        default:
                            if (counter == 0) yield return trim; // var name 2 after uniform
                            break;
                    }
                }
            }
        }

        private static int createVertShader(String vertexCode)
        {
            int vertShader = GL.CreateShader(ShaderType.VertexShader);

            if (vertShader == 0) return 0;

            GL.ShaderSource(vertShader, vertexCode);
            GL.CompileShader(vertShader);

            int param;
            GL.GetShader(vertShader, ShaderParameter.CompileStatus, out param);
            if (param == 0)
            {
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine();
                Console.WriteLine("Vert Shader Compile Error");
                Console.WriteLine();
                Console.WriteLine("*****************************************");
                Console.WriteLine();
                Console.WriteLine(vertexCode);
                Console.WriteLine();
                Console.WriteLine(GLO.GetShaderInfoLog(vertShader));
                Console.WriteLine();
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                return 0;
            }
            return vertShader;
        }

        private static int createFragShader(String fragCode)
        {
            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            if (fragShader == 0) return 0;

            GL.ShaderSource(fragShader, fragCode);
            GL.CompileShader(fragShader);

            int param;
            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out param);

            if (param == 0)
            {
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine();
                Console.WriteLine("Frag Shader Compile Error");
                Console.WriteLine();
                Console.WriteLine("*****************************************");
                Console.WriteLine();
                Console.WriteLine(fragCode);
                Console.WriteLine();
                Console.WriteLine(GLO.GetShaderInfoLog(fragShader));
                Console.WriteLine();
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                Console.WriteLine("*****************************************");
                return 0;
            }
            return fragShader;
        }
    }
}
