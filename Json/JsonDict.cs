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

        /// <summary>
        /// Sends the JsonDict to a stream and 
        /// back to memory.  Useful for converting float[] (unparsed)
        /// to object[] filled with doubles (parsed)
        /// </summary>
        /// <returns></returns>
        public JsonDict Cycle()
        {
            using (var ms = new MemoryStream())
            {
                { // Don't dispose the streamwriter, it will dispose the memory stream
                    var sw = new StreamWriter(ms, Encoding.ASCII);
                    this.Print(sw.Write, sw.Write, "");
                    sw.Flush();
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms, Encoding.ASCII))
                    return JsonDict.FromStream(sr, false);
            }
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

        /// <summary>
        /// Custom JSON ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonParser.ToString(this);
        }

        public int asInt(String key)
        {
            object outo = this[key];
            if (outo is double) return (int)Math.Round((double)outo);
            else if (outo is int) return (int)outo;
            else throw new NotImplementedException();
        }

        public int asInt(String key, int empty_or_error_value)
        {
            object outo;
            if (this.TryGetValue(key, out outo))
            {
                if (outo is double) return (int)Math.Round((double)outo); // when Unpack() does doubles
                else if (outo is int) return (int)outo; // when user adds int
            }
            return empty_or_error_value;
        }

        public float asFloat(String key)
        {
            object outo = this[key];
            if (outo is double) return (float)(double)outo;
            else if (outo is float) return (float)outo;
            else throw new NotImplementedException();
        }

        public float asFloat(String key, float empty_or_error_value)
        {
            object outo;
            if (this.TryGetValue(key, out outo))
            {
                if (outo is double) return (float)(double)outo; // when Unpack() does doubles
                else if (outo is float) return (float)outo; // user adds float
            }
            return empty_or_error_value;
        }

        public double asDouble(String key)
        {
            return (double)this[key];
        }

        public double asDouble(String key, double empty_or_error_value)
        {
            object outo;
            if (this.TryGetValue(key, out outo))
                if (outo is double)
                    return (double)outo;

            return empty_or_error_value;
        }

        public bool asBool(String key)
        {
            return (bool)this[key];
        }

        public bool asBool(String key, bool empty_or_error_value)
        {
            object outo;
            if (this.TryGetValue(key, out outo))
                if (outo is bool)
                    return (bool)outo;

            return empty_or_error_value;
        }





        // //////////////////// DEFAULTS

        public string asString(String key)
        {
            return (string)this[key];
        }

        public string asString(String key, string empty_or_error_value)
        {
            return this.asGeneric(key, empty_or_error_value);
        }






        // ///////////////////// CONSTRUCTORS BELOW

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
                    case 'n': // null
                        if (key_off && sb.Length == 0)
                        {
                            key_off = false;
                            if ('u' != (char)sr.Read()) throw new Exception("Invalid nu");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid nul");
                            if ('l' != (char)sr.Read()) throw new Exception("Invalid nul");
                            ret[key] = null;
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
