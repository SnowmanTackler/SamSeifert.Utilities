using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace SamSeifert.Utilities.FileParsing
{
    public static class JsonParser
    {
        private static void ToLiteral(string text, CharWriter cw)
        {
            cw('"');
            foreach (var c in text)
            {
                switch (c)
                {
                    // case '\'':
                    case '\"':
                    case '\0':
                    case '\a':
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t':
                    case '\v':
                    case '\\':
                        cw('\\');
                        cw(c);
                        break;
                    default:
                        cw(c);
                        break;

                }
            }
            cw('"');
        }

        public static Dictionary<String, object> parseDictionary(StreamReader sr)
        {
            var ret = new Dictionary<String, object>();

            bool key_off = false;
            String key = "";

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char next = (char)sr.Read();

                switch (next)
                {
                    case '}': return ret;
                    case '"':
                        if (key_off)
                        {
                            key_off = false;
                            ret[key] = JsonParser.parseString(sr);
                        }
                        else
                        {
                            key_off = true;
                            key = JsonParser.parseString(sr);
                        }
                        break;
                    case '{':
                        if (key_off)
                        {
                            key_off = false;
                            ret[key] = JsonParser.parseDictionary(sr);
                        }
                        else throw new Exception("Entering Dictionary Without Key");
                        break;
                    case '[':
                        if (key_off)
                        {
                            key_off = false;
                            ret[key] = JsonParser.parseArray(sr);
                        }
                        else throw new Exception("Entering Array Without Key");
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-': // Negative
                    case '.': // Decimal
                    case 'E': // Exponent
                        sb.Append(next);
                        break;
                    default:
                        if (sb.Length != 0)
                        {
                            if (key_off)
                            {
                                key_off = false;
                                ret[key] = Double.Parse(sb.ToString());
                                sb.Length = 0;
                            }
                            else throw new Exception("Number Without Key");
                        }
                        break;
                }
            }
        }

        public static object[] parseArray(StreamReader sr)
        {
            var ret = new List<object>();

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char next = (char)sr.Read();

                switch (next)
                {
                    case ']': return ret.ToArray();
                    case '"':
                        ret.Add(JsonParser.parseString(sr));
                        break;
                    case '{':
                        ret.Add(JsonParser.parseDictionary(sr));
                        break;
                    case '[':
                        ret.Add(JsonParser.parseArray(sr));
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-': // Negative
                    case '.': // Decimal
                    case 'E': // Exponent
                        sb.Append(next);
                        break;
                    default:
                        if (sb.Length != 0)
                        {
                            ret.Add(Double.Parse(sb.ToString()));
                            sb.Length = 0;
                        }
                        break;
                }
            }
        }

        public static String parseString(StreamReader sr)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char next = (char)sr.Read();

                switch (next)
                {
                    case '\\':
                        char temper = (char)sr.Read();
                        switch (temper)
                        {
                            case 'a': sb.Append('\a'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'v': sb.Append('\v'); break;
                            case '0': sb.Append('\0'); break;
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            default:
                                Console.WriteLine("Unsupported Char After Escape Char: *" + temper + "*");
                                throw new NotImplementedException();
                        }
                        break;
                    case '"':
                        return sb.ToString();
                    default:
                        sb.Append(next);
                        break;
                }

            }
        }

        public delegate void CharWriter(char x);
        public delegate void StringWriter(String x);

        public static void WriteString(this NetworkStream q, String text) // Network Stream Extension
        {
            var arg = Encoding.ASCII.GetBytes(text);
            q.Write(arg, 0, arg.Length);
        }
        public static void WriteChar(this NetworkStream q, Char text) // Network Stream Extension
        {
            q.WriteByte((byte)text);
        }


        public static string ToString(Object o)
        {
            StringBuilder sb = new StringBuilder();

            JsonParser.print(o, (char c) => sb.Append(c), (string s) => sb.Append(s));

            return sb.ToString();
        }

        public static void print(object[] o, StreamWriter file)
        {
            JsonParser.print(o, file.Write, file.Write);
        }

        public static void print (Object o)
        {
            JsonParser.print(o, Console.Out.Write, Console.Out.Write);
            Console.Write(Environment.NewLine);
        }

        public static void print(Object o, CharWriter cw, StringWriter sw, string indent = "")
        {
            if (o is String) JsonParser.ToLiteral(o as String, cw);
            else if (o is float) sw(o.ToString());
            else if (o is double) sw(o.ToString());
            else if (o is int) sw(o.ToString());
            else if (o is long) sw(o.ToString());
            else if (o is Dictionary<String, object>) JsonParser.print(o as Dictionary<String, object>, cw, sw, indent);
            else if (o is object[]) JsonParser.print(o as object[], cw, sw, indent);
            else throw new NotImplementedException("Can't Print JSON");
        }

        private static void print(Dictionary<String, object> dict, CharWriter cw, StringWriter sw, string indent = "")
        {
            String nindent = indent + "\t";
            cw('{');
            sw(Environment.NewLine);
            bool first = true;
            foreach (var kvp in dict)
            {
                if (first) first = false;
                else
                {
                    cw(',');
                    sw(Environment.NewLine);
                }
                sw(nindent);
                cw('"');
                sw(kvp.Key);
                cw('"');
                cw(':');
                cw(' ');
                JsonParser.print(kvp.Value, cw, sw, nindent);
            }
            sw(Environment.NewLine);
            sw(indent);
            cw('}');
        }

        private static void print(object[] arg, CharWriter cw, StringWriter sw, string indent = "")
        {
            String nindent = indent + "\t";
            cw('[');
            sw(Environment.NewLine);
            bool first = true;
            foreach (var obj in arg)
            {
                if (first) first = false;
                else
                {
                    cw(',');
                    sw(Environment.NewLine);
                }
                sw(nindent);
                JsonParser.print(obj, cw, sw, nindent);
            }
            sw(Environment.NewLine);
            sw(indent);
            cw(']');
        }
    }
}
