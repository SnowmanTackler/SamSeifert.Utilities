using SamSeifert.Utilities.Maths;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    internal class PointNode : Node
    {
        public readonly double[] Points;

        internal PointNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            this.Points = fieldAttributes["point"].ToArray();

            fieldAttributes.Count.AssertEquals(1);
            fieldNodes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }
    }
}