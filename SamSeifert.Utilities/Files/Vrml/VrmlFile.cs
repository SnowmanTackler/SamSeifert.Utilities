using SamSeifert.Utilities.Files.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SamSeifert.Utilities.Files.Vrml
{
    public class VrmlFile
    {
        /////////////////////// CONSTRUCTORS BELOW

        public static Node FromFile(String path)
        {
            using (StreamReader sr = new StreamReader(path))
                return VrmlFile.FromStream(sr);
        }

        public static Node FromString(String data)
        {
            if (data == null)
                throw new Exception("BracketFile: No Data");
            using (StreamReader sr = new StreamReader(data.AsStream()))
                return VrmlFile.FromStream(sr);
        }

        public static Node FromStream(StreamReader sr)
        {
            var ls = new List<Node>();

            var reader = new VrmlReader(sr);

            return Node.From(null, null, reader, new Dictionary<String, Node>());
        }


        internal enum BracketType
        {
            OpenSquare,
            CloseSquare, 
            OpenCurly,
            CloseCurly,
            Comment,
            Content
        }

        internal class VrmlReader
        {
            private StreamReader streamReader;

            private readonly Queue<String> nextTokenSaved = new Queue<String>();

            public VrmlReader(StreamReader streamReader)
            {
                this.streamReader = streamReader;
                this.nextTokenSaved.Enqueue("children");
                this.nextTokenSaved.Enqueue("[");
            }

            public string nextToken()
            {
                if (nextTokenSaved.Count > 0)
                    return this.nextTokenSaved.Dequeue();

                var sb = new StringBuilder();
                while (!this.streamReader.EndOfStream)
                {
                    char c = (char) this.streamReader.Read();

                    switch (c)
                    {
                        case '}':
                        case '{':
                        case ']':
                        case '[':
                            var cString = c.ToString();
                            if (sb.Length == 0)
                            {
                                return cString;
                            }
                            else
                            {
                                this.nextTokenSaved.Enqueue(cString);
                                return sb.ToString();
                            }
                        case ',':
                        case ' ':
                        case '\n':
                            if (sb.Length != 0)
                            {
                                return sb.ToString();
                            }
                            break;
                        case '#': // Comment.  Read until end of line.
                            while (!this.streamReader.EndOfStream)
                            {
                                if (this.streamReader.Read() == '\n')
                                {
                                    break;
                                }
                            }
                            break;
                        default:
                            sb.Append(c.ToString());
                            break;
                    }
                }


                if (sb.Length == 0)
                {
                    nextTokenSaved.Enqueue("}");  // Base node, end node
                    return "]"; // Base node, end children []
                }
                else
                {
                    throw new Exception("Not empty end of file: " + sb.ToString());
                }
            }

            public void rollBack(string newFirstItem)
            {
                var items = this.nextTokenSaved.ToArray();
                this.nextTokenSaved.Clear();
                this.nextTokenSaved.Enqueue(newFirstItem);
                foreach (var item in items)
                    this.nextTokenSaved.Enqueue(item);
            }

            public void nextShouldBe(string want)
            {
                string got = this.nextToken();

                if (got != want)
                    throw new Exception("Whoops");
            }
        }




    }
}