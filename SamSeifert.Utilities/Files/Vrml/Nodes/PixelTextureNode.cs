using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;

namespace SamSeifert.Utilities.Files.Vrml.Nodes
{
    public class PixelTextureNode : Node
    {
        public readonly bool RepeatS;
        public readonly bool RepeatT;
        public readonly byte[,,] Data;

        internal PixelTextureNode(string name, List<Node> children, Dictionary<string, Node> fieldNodes, Dictionary<string, List<double>> fieldAttributes)
            : base(name)
        {
            var repeatS = fieldAttributes.GetAndRemoveOrDefault("repeatS", null)?.ToArray() ?? new double[] { 0 };
            repeatS.Length.AssertEquals(1);
            this.RepeatS = repeatS[0] > 0.5;

            var repeatT = fieldAttributes.GetAndRemoveOrDefault("repeatT", null)?.ToArray() ?? new double[] { 0 };
            repeatT.Length.AssertEquals(1);
            this.RepeatT = repeatS[0] > 0.5;
            
            var image = (fieldAttributes.GetAndRemoveOrDefault("image", null)?.ToArray() ?? new double[] { 1, 1, 1, 0 });

            var imageWidth = image[0].RoundToInt();
            var imageHeight = image[1].RoundToInt();
            var imageChannels = image[2].RoundToInt();

            image.Length.AssertEquals(3 + imageWidth * imageHeight);

            this.Data = new byte[imageHeight, imageWidth, imageChannels];

            switch (imageChannels)
            {
                case 3:
                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            int token = image[3 + x + y * imageWidth].RoundToInt();
                            this.Data[y, x, 0] = (byte)((token >> 16) & 0xff);
                            this.Data[y, x, 1] = (byte)((token >> 08) & 0xff);
                            this.Data[y, x, 2] = (byte)((token >> 00) & 0xff);
                        }
                    }
                    break;
                default:
                    throw new Exception("whoops!");
            }
            
            fieldNodes.Count.AssertEquals(0);
            fieldAttributes.Count.AssertEquals(0);
            (children ?? new List<Node>()).Count.AssertEquals(0);
        }
    }
}