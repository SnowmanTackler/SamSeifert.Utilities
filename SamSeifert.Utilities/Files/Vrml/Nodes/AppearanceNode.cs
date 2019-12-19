using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class AppearanceNode : Node
    {
        /// <summary>
        /// Nullable
        /// </summary>
        public readonly MaterialNode Material;

        /// <summary>
        /// Nullable
        /// </summary>
        public readonly PixelTextureNode Texture;

        public AppearanceNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            var material = fieldNodes.GetAndRemoveOrDefault("material", null);
            if (material != null)
            {
                this.Material = material as MaterialNode;
                Assert.IsNotNull(this.Material);
            }

            var texture = fieldNodes.GetAndRemoveOrDefault("texture", null);
            if (texture != null)
            {
                this.Texture = texture as PixelTextureNode;
                Assert.IsNotNull(this.Texture);
            }

            fieldAttributes.Count.AssertEquals(0);
            fieldNodes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }
    }
}