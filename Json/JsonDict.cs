using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Json
{
    public class JsonDict : Dictionary<String, Object>
    {
        public JsonDict() : base()
        {
        }

        public JsonDict(int capacity) : base(capacity)
        {

        }

        public void Print(CharWriter cw, StringWriter sw, string indent = "")
        {
            String nindent = indent + "\t";
            cw('{');
            sw(Environment.NewLine);
            bool first = true;
            foreach (var kvp in this)
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
                JsonParser.Print(kvp.Value, cw, sw, nindent);
            }
            sw(Environment.NewLine);
            sw(indent);
            cw('}');
        }

        public override string ToString()
        {
            return JsonParser.ToString(this);
        }

        public static JsonDict FromFile(String path)
        {
            using (StreamReader sr = new StreamReader(path))
                return JsonDict.FromStream(sr, false);
        }

        public static JsonDict FromString(String data)
        {
            if (data == null)
                throw new Exception("JSON Parser: No Data");
            using (StreamReader sr = new StreamReader(data.AsStream()))
                return JsonDict.FromStream(sr, false);
        }

        public static JsonDict FromStream(StreamReader sr, bool first_bracket_found = true)
        {
            if (!first_bracket_found)
            {
                while (!sr.EndOfStream)
                    if (sr.Read() == '{')
                        break;

                if (sr.EndOfStream) return null;
            }

            var ret = new JsonDict();

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
                            ret[key] = JsonParser.ParseString(sr);
                        }
                        else
                        {
                            key_off = true;
                            key = JsonParser.ParseString(sr);
                        }
                        break;
                    case '{':
                        if (key_off)
                        {
                            key_off = false;
                            ret[key] = JsonDict.FromStream(sr);
                        }
                        else throw new Exception("Entering Dictionary Without Key");
                        break;
                    case '[':
                        if (key_off)
                        {
                            key_off = false;
                            ret[key] = JsonArray.FromStream(sr);
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
                    case 't': // true
                        if (key_off && sb.Length == 0)
                        {
                            key_off = false;
                            if ('r' != (char)sr.Read()) throw new Exception("Invalid tr");
                            if ('u' != (char)sr.Read()) throw new Exception("Invalid tru");
                            if ('e' != (char)sr.Read()) throw new Exception("Invalid true");
                            ret[key] = true;
                        }
                        else throw new Exception("Invalid t + ");
                        break;
                    case 'f': // false
                        if (key_off && sb.Length == 0)
                        {
                            key_off = false;
                            if ('a' != (char)sr.Read()) throw new Exception("Invalid fa");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid fal");
                            if ('s' != (char)sr.Read()) throw new Exception("Invalid fals");
                            if ('e' != (char)sr.Read()) throw new Exception("Invalid false");
                            ret[key] = false;
                        }
                        else throw new Exception("Invalid f");
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
    }
}
