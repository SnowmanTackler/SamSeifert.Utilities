﻿using SamSeifert.Utilities.Extensions;
using SamSeifert.Utilities.Files.Vrml;
using SamSeifert.Utilities.Files.Vrml.Nodes;
using SamSeifert.Utilities.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL = SamSeifert.GLE.GLR;
using SamSeifert.GLE.Extensions;

namespace SamSeifert.GLE.CAD.Generator
{
    public static partial class FromVrml
    {
        private static Vector3[] Vec3sFromDoubles(double[,] doubles)
        {
            doubles.GetLength(1).AssertEquals(3);
            var vecs = new Vector3[doubles.GetLength(0)];
            for (int i = 0; i < vecs.Length; i++)
                vecs[i] = new Vector3((float)doubles[i, 0], (float)doubles[i, 1], (float)doubles[i, 2]);
            return vecs;
        }

        private static Vector2[] Vec2sFromDoubles(double[,] doubles)
        {
            doubles.GetLength(1).AssertEquals(2);
            var vecs = new Vector2[doubles.GetLength(0)];
            for (int i = 0; i < vecs.Length; i++)
                vecs[i] = new Vector2((float)doubles[i, 0], (float)doubles[i, 1]);
            return vecs;
        }

        private static IEnumerable<int> getPolygonVertexCounts(int[] indicies)
        {
            int count = 0;
            foreach (var i in indicies)
            {
                if (i == -1)
                {
                    yield return count;
                    count = 0;
                }
                else
                {
                    count++;
                }
            }
        }

        private static ColorGL glColorForVrmlMaterial(MaterialNode material)
        {
            var mat = new ColorGL();

            mat.setAlpha((float)material.Transparency);
            mat._Shininess[0] = (float)material.Shininess;

            for (int i = 0; i < 3; i++)
            {
                mat._Ambient[i] = (float)material.Ambient;
                mat._Diffuse[i] = (float)material.DiffuseVec3[i];
                mat._Specular[i] = (float)material.SpecularVec3[i];
                mat._Emission[i] = (float)material.EmissiveVec3[i];
            }

            return mat;
        }

        public static CadObject Create(Node node)
        {
            if (node == null)
                return null;

            var children = node.Children().Select(it => Create(it)).Where(it => it != null).ToArray();
            if (children.Length > 0)
            {
                if (node is TransformNode)
                {
                    var transformNode = node as TransformNode;

                    var co = new CadObject(children, node.Name ?? "TransformNode");

                    var rot = Matrix4.CreateFromAxisAngle(new Vector3(
                        (float)transformNode.RotationVec3[0],
                        (float)transformNode.RotationVec3[1],
                        (float)transformNode.RotationVec3[2]),
                        (float)transformNode.RotationAngle);

                    var scale = Matrix4.CreateScale(new Vector3(
                        (float)transformNode.ScaleVec3[0],
                        (float)transformNode.ScaleVec3[1],
                        (float)transformNode.ScaleVec3[2]));

                    var trans = Matrix4.CreateTranslation(new Vector3(
                        (float)transformNode.TranslationVec3[0],
                        (float)transformNode.TranslationVec3[1],
                        (float)transformNode.TranslationVec3[2]));

                    co.SetMatrix(rot * scale * trans); // Equiv to (rot * trans * scale)
                    //Matrix4.CreateFromAxisAngle()
                    // co.SetMatrix()
                    return co;
                }
                else if (node is UnidentifiedNode) // Root of files 
                {
                    var co = new CadObject(children, node.Name ?? "UnidentifiedNode");
                    return co;
                }
                else
                {
                    throw new Exception("whoops!");
                }
            }
            else if (node is ShapeNode)
            {
                var shapeNode = node as ShapeNode;

                if (shapeNode.Geometry is IndexedFaceSetNode)
                {
                    var material = glColorForVrmlMaterial(shapeNode.Appearance?.Material);
                    var geometry = shapeNode.Geometry as IndexedFaceSetNode;
                    
                    var polygonVertexCounts = new HashSet<int>(getPolygonVertexCounts(geometry.CoordinatesIndicies));

                    polygonVertexCounts.Count.AssertEquals(1);

                    switch (polygonVertexCounts.First())
                    {
                        case 4:
                            var inVerts = Vec3sFromDoubles(geometry.Coordinates);
                            var inIndices = geometry.CoordinatesIndicies;

                            (inIndices.Length % 5).AssertEquals(0);

                            var verts = new List<Vector3>(inVerts.Length);
                            var norms = new List<Vector3>(inVerts.Length);

                            for (int i = 0; i < inIndices.Length; i += 5)
                            {
                                var v1 = inVerts[inIndices[i + 0]];
                                var v2 = inVerts[inIndices[i + 1]];
                                var v3 = inVerts[inIndices[i + 2]];
                                var v4 = inVerts[inIndices[i + 3]];
                                var v5 = v1;
                                var v6 = v2;

                                var c1 = Vector3.Cross(v1 - v2, v3 - v2).NormalizedSafe();
                                var c2 = Vector3.Cross(v1 - v2, v3 - v2).NormalizedSafe();
                                var c3 = Vector3.Cross(v1 - v2, v3 - v2).NormalizedSafe();
                                var c4 = Vector3.Cross(v1 - v2, v3 - v2).NormalizedSafe();

                                var norm = (c1 + c2 + c3 + c4).NormalizedSafe();

                                if (geometry.Solid || !geometry.CounterClockWise)
                                {
                                    norm = -norm;
                                    verts.AddRange(new Vector3[] { v1, v2, v3, v1, v3, v4 });
                                    norms.AddRange(new Vector3[] { norm, norm, norm, norm, norm, norm });
                                }
                                else // Counter clockwise
                                {
                                    verts.AddRange(new Vector3[] { v1, v3, v2, v1, v4, v3 });
                                    norms.AddRange(new Vector3[] { norm, norm, norm, norm, norm, norm });
                                }
                            }

                            var co = new CadObject(verts.ToArray(), norms.ToArray(), node.Name ?? "Shape");
                            co._CullFaceMode =  geometry.Solid ? (CullFaceMode?)null : 
                                geometry.CounterClockWise ? CullFaceMode.Back : CullFaceMode.Front;
                            co.SetColor(material);
                            return co;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else throw new NotImplementedException();
            }
            else return null;
        }
    }
}
