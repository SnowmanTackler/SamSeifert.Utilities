using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class MaterialNode : Node
    {
        public readonly double Ambient;
        public readonly double[] DiffuseVec3;
        public readonly double[] SpecularVec3;
        public readonly double[] EmissiveVec3;
        public readonly double Shininess;
        public readonly double Transparency;

        internal MaterialNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            var ambientIntensity = fieldAttributes.GetAndRemoveOrDefault("ambientIntensity", null)?.ToArray() ?? new double[] { 0 };
            ambientIntensity.Length.AssertEquals(1);
            this.Ambient = ambientIntensity[0];

            this.DiffuseVec3 = fieldAttributes.GetAndRemoveOrDefault("diffuseColor", null)?.ToArray() ?? new double[] { 0, 0, 0 };
            this.DiffuseVec3.Length.AssertEquals(3);

            this.SpecularVec3 = fieldAttributes.GetAndRemoveOrDefault("specularColor", null)?.ToArray() ?? new double[] { 0, 0, 0 };
            this.SpecularVec3.Length.AssertEquals(3);

            this.EmissiveVec3 = fieldAttributes.GetAndRemoveOrDefault("emissiveColor", null)?.ToArray() ?? new double[] { 0, 0, 0 };
            this.EmissiveVec3.Length.AssertEquals(3);

            var shininess = fieldAttributes.GetAndRemoveOrDefault("shininess", null)?.ToArray() ?? new double[] { 0 };
            shininess.Length.AssertEquals(1);
            this.Shininess = shininess[0];

            var transparency = fieldAttributes.GetAndRemoveOrDefault("transparency", null)?.ToArray() ?? new double[] { 0 };
            transparency.Length.AssertEquals(1);
            this.Transparency = transparency[0];

            fieldAttributes.Count.AssertEquals(0);
            fieldNodes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }
    }
}