using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Json
{
    public delegate void CharWriter(char x);
    public delegate void StringWriter(String x);

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
                    // case '\'':
                    case '\"': cw('\\'); cw('"'); break;
                    case '\0': cw('\\'); cw('0'); break;
                    case '\a': cw('\\'); cw('a'); break;
                    case '\b': cw('\\'); cw('b'); break;
                    case '\f': cw('\\'); cw('f'); break;
                    case '\n': cw('\\'); cw('n'); break;
                    case '\r': cw('\\'); cw('r'); break;
                    case '\t': cw('\\'); cw('t'); break;
                    case '\v': cw('\\'); cw('v'); break;
                    case '\\': cw('\\'); cw('\\'); break;
                    default:
                        cw(c);
                        break;
                }
            }
            cw('"');
        }

        internal static String ParseString(StreamReader sr)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                if (sr.EndOfStream) throw new Exception("End Of File");
                char next = (char)sr.Read();

                switch (next)
                {
                    case '\\':
                        if (sr.EndOfStream) throw new Exception("End Of File");
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
                            case 'u': sb.Append("\\u"); break; // Unicode
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

            JsonParser.Print(o, (char c) => sb.Append(c), (string s) => sb.Append(s));

            return sb.ToString();
        }

        public static void Print(object o, StreamWriter writer)
        {
            JsonParser.Print(o, writer.Write, writer.Write);
        }

        public static void Print(Object o)
        {
            JsonParser.Print(o, Console.Out.Write, Console.Out.Write);
            Console.Write(Environment.NewLine);
        }


        public static void Print(Object o, CharWriter cw)
        {
            StringWriter sw = (String stringg) =>
            {
                foreach (char charr in stringg)
                {
                    cw(charr);
                }
            };
            JsonParser.Print(o, cw, sw);
        }

        /// <summary>
        /// Not sure if StringWriter is faster than char writer for all streams, so let user specify both
        /// </summary>
        /// <param name="o"></param>
        /// <param name="cw"></param>
        /// <param name="sw"></param>
        /// <param name="indent"></param>
        public static void Print(Object o, CharWriter cw, StringWriter sw, string indent = "")
        {
            if (o == null) sw("null");
            else if (o is String) JsonParser.ToLiteral(o as String, cw);
            else if (o is float) sw(o.ToString());
            else if (o is double) sw(o.ToString());
            else if (o is byte) sw(o.ToString());
            else if (o is int) sw(o.ToString());
            else if (o is uint) sw(o.ToString());
            else if (o is long) sw(o.ToString());
            else if (o is bool) sw(((bool)o) ? "true" : "false");
            else if (o is JsonDict) (o as JsonDict).Print(cw, sw, indent);
            else if (o is object[]) (o as Object[]).Print(cw, sw, indent);
            else throw new NotImplementedException("Can't Print JSON for:" + o.GetType());
        }
    }
}
