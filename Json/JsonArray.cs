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
                    bool first = true;
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
                    bool first = true;
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

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char next = (char)sr.Read();

                switch (next)
                {
                    case '"':
                        ret.Add(JsonParser.ParseString(sr));
                        break;
                    case '{':
                        ret.Add(JsonDict.FromStream(sr));
                        break;
                    case '[':
                        ret.Add(JsonArray.FromStream(sr));
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
                    case ']':
                        if (sb.Length != 0)
                        {
                            ret.Add(Double.Parse(sb.ToString()));
                            sb.Length = 0;
                        }
                        return ret.ToArray();
                }
            }
        }
    }
}
