using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    internal class UnidentifiedNode : Node
    {
        public readonly List<Node> children;
        public readonly string type;
        public readonly Dictionary<string, Node> fieldNodes;
        public Dictionary<string, List<double>> fieldAttributes;

        public UnidentifiedNode(string name, string type, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            this.children = children;
            this.type = type;
            this.fieldNodes = fieldNodes;
            this.fieldAttributes = fieldAttributes;
        }
    }
}