using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SamSeifert.Utilities.Files.Vrml.VrmlFile;

namespace SamSeifert.Utilities.Files.Vrml
{
    public abstract class Node
    {
        public readonly string Name;

        public Node(string name)
        {
            this.Name = name;
        }

        private static double? getValueForToken(String next)
        {
            double ret;
            if (next == "TRUE")
            {
                return 1;
            }
            else if (next == "FALSE")
            {
                return 0;
            }
            else if (Double.TryParse(next, out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }

        private static List<Node> ReadChildrenList(VrmlReader reader, Dictionary<string, Node> namedNodes)
        {
            var ls = new List<Node>();
            reader.nextShouldBe("[");
            while (true)
            {
                String token = reader.nextToken();

                if (token == "DEF")
                {
                    var name = reader.nextToken();
                    var type = reader.nextToken();
                    reader.nextShouldBe("{");
                    ls.Add(Node.From(name, type, reader, namedNodes));
                }
                else if (token == "]")
                {
                    return ls;
                }
                else if (Char.IsUpper(token.First()))
                {
                    reader.nextShouldBe("{");
                    ls.Add(Node.From(null, token, reader, namedNodes));
                } 
                else
                {
                    throw new Exception("whoops");
                }
            }
        }

        private static List<double> ReadValuesList(VrmlReader reader)
        {
            reader.nextShouldBe("[");
            var fl = new List<double>();
            while (true)
            {
                var nextToken = reader.nextToken();
                double ret;
                if (nextToken == "]")
                {
                    return fl;
                }
                else if (Double.TryParse(nextToken, out ret))
                {
                    fl.Add(ret);
                }
                else
                {
                    throw new Exception("Whoops");
                }
            }
        }

        private static Node From(
            string name,
            string type, List<Node> children,
            Dictionary<string, Node> fieldNodes, 
            Dictionary<string, List<double>> fieldAttributes)
        {

            switch (type)
            {
                case "Transform":
                    return new Nodes.TransformNode(name, children, fieldNodes, fieldAttributes);
                case "Material":
                    return new Nodes.MaterialNode(name, children, fieldNodes, fieldAttributes);
                case "PixelTexture":
                    return new Nodes.PixelTextureNode(name, children, fieldNodes, fieldAttributes);
                case "Appearance":
                    return new Nodes.AppearanceNode(name, children, fieldNodes, fieldAttributes);
                case "Coordinate":
                case "TextureCoordinate":
                    return new Nodes.PointNode(name, children, fieldNodes, fieldAttributes);
                case "IndexedFaceSet":
                    return new Nodes.IndexedFaceSetNode(name, children, fieldNodes, fieldAttributes);
                case "Shape":
                    return new Nodes.ShapeNode(name, children, fieldNodes, fieldAttributes);
                default:
                    Console.WriteLine(type);
                    return new Nodes.UnidentifiedNode(name, type, children, fieldNodes, fieldAttributes);
            }
        }

        internal static Node From(
            string name, 
            string type,
            VrmlReader reader,
            Dictionary<string, Node> namedNodes)
        {
            List<Node> children = null;
            var fieldNodes = new Dictionary<String, Node>();
            var fieldAttributes = new Dictionary<String, List<double>>();

            while (true)
            {
                String firstToken = reader.nextToken();
                switch (firstToken)
                {
                    case "}":
                        var node = From(name, type, children, fieldNodes, fieldAttributes);
                        if (name != null)
                        {
                            namedNodes.ContainsKey(name).AssertFalse();
                            namedNodes[name] = node;
                        }
                        return node;
                    case "children":
                        if (children == null)
                        {
                            children = ReadChildrenList(reader, namedNodes);
                        }
                        else
                        {
                            throw new Exception("whoops!");
                        }
                        break;
                    case "coordIndex":
                    case "texCoordIndex":
                    case "point":
                        {
                            if (fieldAttributes.ContainsKey(firstToken))
                                throw new Exception("Whoops");
                            fieldAttributes[firstToken] = ReadValuesList(reader);
                            break;
                        }
                    default:
                        {
                            var secondToken = reader.nextToken();
                            double? secondValue = null;

                            if (secondToken == "DEF")
                            {
                                // appearance DEF MAT_vein3 Appearance {
                                var childName = reader.nextToken();
                                var childType = reader.nextToken();
                                reader.nextShouldBe("{");
                                if (fieldNodes.ContainsKey(firstToken))
                                    throw new Exception("Whoops");
                                fieldNodes[firstToken] = Node.From(childName, childType, reader, namedNodes);
                            }
                            else if (secondToken == "USE")
                            {
                                var matchedNodeName = reader.nextToken(); // USE AT SOME POINT;
                                var matchedNode = namedNodes.GetOrDefault(matchedNodeName, null);
                                matchedNode.AssertNotNull();
                                fieldNodes[firstToken] = matchedNode;
                            }
                            else if ((secondValue = getValueForToken(secondToken)) == null)
                            {
                                // material Material {
                                reader.nextShouldBe("{");
                                if (fieldNodes.ContainsKey(firstToken))
                                    throw new Exception("Whoops");
                                fieldNodes[firstToken] = Node.From(null, secondToken, reader, namedNodes);
                            }
                            else
                            {
                                // solid FALSE
                                // creaseAngle 3.141593
                                // image 1024 512 3 7
                                var fl = new List<double>();
                                fl.Add(secondValue ?? 0);

                                while (true)
                                {
                                    var nextToken = reader.nextToken();
                                    var nextValue = getValueForToken(nextToken);
                                    if (nextValue == null)
                                    {
                                        reader.rollBack(nextToken);
                                        break;
                                    }
                                    else
                                    {
                                        fl.Add(nextValue ?? 0);
                                    }
                                }

                                if (fieldAttributes.ContainsKey(firstToken))
                                    throw new Exception("Whoops");
                                fieldAttributes[firstToken] = fl;
                            }
                            break;
                        } // end default case
                } // end switch (token)
            } // end while loop
        }

        #region HelperFunctionsForDevelopingNewNodes
        private static HashSet<String> fieldNodesKeys = new HashSet<string>();
        private static HashSet<String> fieldAttributesKeys = new HashSet<string>();    
        protected void PrintUniqueKeys(Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
        {
            foreach (var key in fieldNodes.Keys)
                if (!fieldNodesKeys.Contains(key))
                {
                    Logger.WriteLine("fieldNodes " + key);
                    fieldNodesKeys.Add(key);
                }
            foreach (var key in fieldAttributes.Keys)
                if (!fieldAttributesKeys.Contains(key))
                {
                    Logger.WriteLine("fieldAttributes " + key);
                    fieldAttributesKeys.Add(key);
                }
        }
        #endregion
    }
}
