using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class ShapeNode : Node
    {
        /// <summary>
        /// Nullable
        /// </summary>
        public readonly AppearanceNode Appearance;
        public readonly Node Geometry;

        internal ShapeNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            var appearance = fieldNodes.GetAndRemoveOrDefault("appearance", null);
            if (appearance != null)
            {
                this.Appearance = appearance as AppearanceNode;
                this.Appearance.AssertNotNull();
            }

            this.Geometry = fieldNodes.GetAndRemoveOrDefault("geometry", null);
            this.Geometry.AssertNotNull();

            fieldAttributes.Count.AssertEquals(0);
            fieldNodes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }

    }
}