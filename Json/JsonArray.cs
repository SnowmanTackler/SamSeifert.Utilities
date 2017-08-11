using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Json
{
    public static class JsonArray
    {

        public static void Print(this object[] arg, CharWriter cw, StringWriter sw, string indent = "")
        {
            String nindent = indent + "\t";
            cw('[');

            if (arg.Length != 0)
            {
                var o = arg[0];

                bool first = true;

                if ((o is bool) ||
                    (o is byte) ||
                    (o is sbyte) ||
                    (o is UInt16) ||
                    (o is Int16) ||
                    (o is UInt32) ||
                    (o is Int32) ||
                    (o is UInt64) ||
                    (o is Int64) ||
                    (o is float) ||
                    (o is double) ||
                    (o is decimal)
                    )
                {
                    foreach (var obj in arg)
                    {
                        if (first) first = false;
                        else sw(",");
                        JsonParser.Print(obj, cw, sw, nindent);
                    }
                }
                else
                {
                    var sep = "," + Environment.NewLine + nindent;
                    sw(Environment.NewLine + nindent);
                    foreach (var obj in arg)
                    {
                        if (first) first = false;
                        else sw(sep);
                        JsonParser.Print(obj, cw, sw, nindent);
                    }
                    sw(Environment.NewLine);
                    sw(indent);
                }

            }
            cw(']');
        }

        public static Object[] FromFile(String path)
        {
            using (StreamReader sr = new StreamReader(path))
                return JsonArray.FromStream(sr, false);
        }

        /// <summary>
        /// Calls function on each object in first level of array.  Use this on huge Json Files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="act"></param>
        public static void FromFile(String path, Action<Object> act)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                    if (sr.Read() == '[')
                        break;

                if (sr.EndOfStream) return;

                FromStream(sr, act);
            }
        }

        public static Object[] FromString(String data)
        {
            if (data == null)
                throw new Exception("JSON Parser: No Data");
            using (StreamReader sr = new StreamReader(data.AsStream()))
                return JsonArray.FromStream(sr, false);
        }


        public static Object[] FromStream(StreamReader sr, bool first_bracket_found = true)
        {
            if (!first_bracket_found)
            {
                while (!sr.EndOfStream)
                    if (sr.Read() == '[')
                        break;

                if (sr.EndOfStream) return null;
            }

            var ret = new List<Object>();
            FromStream(sr, (Object o) => { ret.Add(o); });
            return ret.ToArray();
        }


        private static void FromStream(StreamReader sr, Action<Object> act)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char next = (char)sr.Read();

                switch (next)
                {
                    case '"':
                        act(JsonParser.ParseString(sr));
                        break;
                    case '{':
                        act(JsonDict.FromStream(sr));
                        break;
                    case '[':
                        act(JsonArray.FromStream(sr));
                        break;
                    case ']':
                        if (sb.Length != 0)
                        {
                            act(Double.Parse(sb.ToString()));
                            sb.Length = 0;
                        }
                        return;
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
                            act(Double.Parse(sb.ToString()));
                            sb.Length = 0;
                        }
                        break;
                    case 't': // true
                        if (sb.Length == 0)
                        {
                            if ('r' != (char)sr.Read()) throw new Exception("Invalid tr");
                            if ('u' != (char)sr.Read()) throw new Exception("Invalid tru");
                            if ('e' != (char)sr.Read()) throw new Exception("Invalid true");
                            act(true);
                        }
                        else throw new Exception("Invalid t + ");
                        break;
                    case 'f': // false
                        if (sb.Length == 0)
                        {
                            if ('a' != (char)sr.Read()) throw new Exception("Invalid fa");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid fal");
                            if ('s' != (char)sr.Read()) throw new Exception("Invalid fals");
                            if ('e' != (char)sr.Read()) throw new Exception("Invalid false");
                            act(false);
                        }
                        else throw new Exception("Invalid f");
                        break;
                    case 'n': // null
                        if (sb.Length == 0)
                        {
                            if ('u' != (char)sr.Read()) throw new Exception("Invalid nu");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid nul");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid nul");
                            act(null);
                        }
                        else throw new Exception("Invalid f");
                        break;
                }
            }
        }
    }
}
