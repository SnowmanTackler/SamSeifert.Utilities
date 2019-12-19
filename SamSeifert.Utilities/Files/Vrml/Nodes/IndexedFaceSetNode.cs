using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class IndexedFaceSetNode : Node
    {
        public readonly double[,] Coordinates;
        public readonly double[,] TexCoordinates;

        public readonly int[] CoordinatesIndicies;    
        public readonly int[] TexCoordinatesIndicies;

        /// <summary>
        /// Which face to cull
        /// </summary>
        public readonly bool CounterClockWise;

        /// <summary>
        /// Wheter or not we should cull a face.
        /// </summary>
        public readonly bool Solid;

        public readonly double CreaseAngle;

        internal IndexedFaceSetNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {

            this.Coordinates = unpackTwoDimensionalDoubleArray((fieldNodes.GetAndRemoveOrDefault("coord", null) as PointNode).Points, 3);
            Assert.IsNotNull(this.Coordinates);

            this.CoordinatesIndicies = unpackOneDimensionalIntArray(fieldAttributes.GetAndRemoveOrDefault("coordIndex", null));
            Assert.IsNotNull(this.CoordinatesIndicies);

            var texCoord = fieldNodes.GetAndRemoveOrDefault("texCoord", null);
            if (texCoord != null) {
                this.TexCoordinates = unpackTwoDimensionalDoubleArray((texCoord as PointNode).Points, 2);
                Assert.IsNotNull(this.TexCoordinates);
            }

            var texCoordIndex = fieldAttributes.GetAndRemoveOrDefault("texCoordIndex", null);
            if (texCoordIndex != null)
            {
                this.TexCoordinatesIndicies = unpackOneDimensionalIntArray(texCoordIndex);
                Assert.IsNotNull(this.TexCoordinatesIndicies);
            }

            switch ((this.TexCoordinates == null ? 0 : 1) + (this.TexCoordinatesIndicies == null ? 0 : 1))
            {
                case 0: // both null
                case 2: // both not null
                    break;
                default:
                    throw new Exception("whoops");
            }

            var ccw = fieldAttributes.GetAndRemoveOrDefault("ccw", null)?.ToArray() ?? new double[] { 0 };
            ccw.Length.AssertEquals(1);
            this.CounterClockWise = ccw[0] > 0.5;

            var solid = fieldAttributes.GetAndRemoveOrDefault("solid", null)?.ToArray() ?? new double[] { 1 };
            solid.Length.AssertEquals(1);
            this.Solid = solid[0] > 0.5;

            var creaseAngle = fieldAttributes.GetAndRemoveOrDefault("creaseAngle", null)?.ToArray() ?? new double[] { 0 };
            creaseAngle.Length.AssertEquals(1);
            this.CreaseAngle = creaseAngle[0];

            fieldAttributes.Count.AssertEquals(0);
            fieldNodes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }

        private double[,] unpackTwoDimensionalDoubleArray(double[] oneDimensionArray, int secondDimension)
        {
            if (oneDimensionArray == null)
                return null;

            (oneDimensionArray.Length % secondDimension).AssertEquals(0);

            int firstDimension = oneDimensionArray.Length / secondDimension;

            var result = new double[firstDimension, secondDimension];

            int dex = 0;

            for (int y = 0; y < firstDimension; y++)
            {
                for (int x = 0; x < secondDimension; x++)
                {
                    result[y, x] = oneDimensionArray[dex++];
                }
            }

            return result;
        }

        private int[] unpackOneDimensionalIntArray(List<double> input)
        {
            if (input == null)
                return null;

            var result = new int[input.Count];

            for (int x = 0; x < input.Count; x++)
            {
                result[x] = input[x].RoundToInt();
            }

            return result;
        }
    }
}