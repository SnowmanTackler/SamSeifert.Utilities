using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class TransformNode : Node
    {
        public readonly double[] ScaleVec3;
        public readonly double[] TranslationVec3;
        public readonly double[] RotationVec3;
        public readonly double RotationAngle;

        public readonly Node[] children;

        internal TransformNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {

            this.TranslationVec3 = fieldAttributes.GetAndRemoveOrDefault("translation", null)?.ToArray() ?? new double[] { 0, 0, 0 };
            this.TranslationVec3.Length.AssertEquals(3);

            var rotation = fieldAttributes.GetAndRemoveOrDefault("rotation", null)?.ToArray() ?? new double[] { 1, 0, 0, 0};
            rotation.Length.AssertEquals(4);
            this.RotationVec3 = rotation.SubArray(0, 3);
            this.RotationAngle = rotation[3];

            this.ScaleVec3 = fieldAttributes.GetAndRemoveOrDefault("scale", null)?.ToArray() ?? new double[] { 1, 1, 1 };
            this.TranslationVec3.Length.AssertEquals(3);

            fieldNodes.Count.AssertEquals(0);
            fieldAttributes.Count.AssertEquals(0);

            this.children = children.ToArray();
        }

        public override ICollection<Node> Children()
        {
            return this.children;
        }

    }
}
