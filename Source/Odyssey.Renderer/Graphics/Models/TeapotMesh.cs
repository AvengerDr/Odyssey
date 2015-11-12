﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Models;
using SharpDX.Direct3D;
using SharpDX;

namespace SharpDX.Toolkit.Graphics
{
    public class TeapotMesh : IPrimitiveGenerator<VertexPositionNormalTexture>
    {
        private readonly Vector2 tileFactor;
        private readonly float size;
        private readonly int tessellation;

        public Vector2 TileFactor
        {
            get { return tileFactor; }
        }

        public float Size
        {
            get { return size; }
        }

        public int Tessellation
        {
            get { return tessellation; }
        }

        public TeapotMesh(float size, float tileX, float tileY, int tessellation)
        {
            Contract.Requires<ArgumentOutOfRangeException>(tessellation > 0, "tessellation must be > 0");
            tileFactor = new Vector2(tileX, tileY);
            this.size = size;
            this.tessellation = tessellation;
        }

        /// <summary>
        /// A Teapot primitive.
        /// </summary>

        // The teapot model consists of 10 bezier patches. Each patch has 16 control
        // points, plus a flag indicating whether it should be mirrored in the Z axis
        // as well as in X (all of the teapot is symmetrical from left to right, but
        // only some parts are symmetrical from front to back). The control points
        // are stored as integer indices into the TeapotControlPoints array.
        private struct TeapotPatch
        {
            public TeapotPatch(bool mirrorZ, params int[] indices)
            {
                this.mirrorZ = mirrorZ;
                this.indices = indices;
            }

            public readonly bool mirrorZ;
            public readonly int[] indices;
        }

        // Static data array defines the bezier patches that make up the teapot.
        private static readonly TeapotPatch[] TeapotPatches = new TeapotPatch[]
        {
            // Rim.
            new TeapotPatch(true, new[] {102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}),

            // Body.
            new TeapotPatch(true, new[] {12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27}),
            new TeapotPatch(true, new[] {24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40}),

            // Lid.
            new TeapotPatch(true, new[] {96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3}),
            new TeapotPatch(true, new[] {0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117}),

            // Handle.
            new TeapotPatch(false, new[] {41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56}),
            new TeapotPatch(false, new[] {53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67}),

            // Spout.
            new TeapotPatch(false, new[] {68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83}),
            new TeapotPatch(false, new[] {80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95}),

            // Bottom.
            new TeapotPatch(true, new[] {118, 118, 118, 118, 124, 122, 119, 121, 123, 126, 125, 120, 40, 39, 38, 37}),
        };


        // Static array defines the control point positions that make up the teapot.
        private static readonly Vector3[] TeapotControlPoints = new Vector3[]
        {
            new Vector3(0, 0.345f, -0.05f),
            new Vector3(-0.028f, 0.345f, -0.05f),
            new Vector3(-0.05f, 0.345f, -0.028f),
            new Vector3(-0.05f, 0.345f, -0),
            new Vector3(0, 0.3028125f, -0.334375f),
            new Vector3(-0.18725f, 0.3028125f, -0.334375f),
            new Vector3(-0.334375f, 0.3028125f, -0.18725f),
            new Vector3(-0.334375f, 0.3028125f, -0),
            new Vector3(0, 0.3028125f, -0.359375f),
            new Vector3(-0.20125f, 0.3028125f, -0.359375f),
            new Vector3(-0.359375f, 0.3028125f, -0.20125f),
            new Vector3(-0.359375f, 0.3028125f, -0),
            new Vector3(0, 0.27f, -0.375f),
            new Vector3(-0.21f, 0.27f, -0.375f),
            new Vector3(-0.375f, 0.27f, -0.21f),
            new Vector3(-0.375f, 0.27f, -0),
            new Vector3(0, 0.13875f, -0.4375f),
            new Vector3(-0.245f, 0.13875f, -0.4375f),
            new Vector3(-0.4375f, 0.13875f, -0.245f),
            new Vector3(-0.4375f, 0.13875f, -0),
            new Vector3(0, 0.007499993f, -0.5f),
            new Vector3(-0.28f, 0.007499993f, -0.5f),
            new Vector3(-0.5f, 0.007499993f, -0.28f),
            new Vector3(-0.5f, 0.007499993f, -0),
            new Vector3(0, -0.105f, -0.5f),
            new Vector3(-0.28f, -0.105f, -0.5f),
            new Vector3(-0.5f, -0.105f, -0.28f),
            new Vector3(-0.5f, -0.105f, -0),
            new Vector3(0, -0.105f, 0.5f),
            new Vector3(0, -0.2175f, -0.5f),
            new Vector3(-0.28f, -0.2175f, -0.5f),
            new Vector3(-0.5f, -0.2175f, -0.28f),
            new Vector3(-0.5f, -0.2175f, -0),
            new Vector3(0, -0.27375f, -0.375f),
            new Vector3(-0.21f, -0.27375f, -0.375f),
            new Vector3(-0.375f, -0.27375f, -0.21f),
            new Vector3(-0.375f, -0.27375f, -0),
            new Vector3(0, -0.2925f, -0.375f),
            new Vector3(-0.21f, -0.2925f, -0.375f),
            new Vector3(-0.375f, -0.2925f, -0.21f),
            new Vector3(-0.375f, -0.2925f, -0),
            new Vector3(0, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.17625f, 0.4f),
            new Vector3(-0.075f, 0.2325f, 0.375f),
            new Vector3(0, 0.2325f, 0.375f),
            new Vector3(0, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.17625f, 0.575f),
            new Vector3(-0.075f, 0.2325f, 0.625f),
            new Vector3(0, 0.2325f, 0.625f),
            new Vector3(0, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.17625f, 0.675f),
            new Vector3(-0.075f, 0.2325f, 0.75f),
            new Vector3(0, 0.2325f, 0.75f),
            new Vector3(0, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.675f),
            new Vector3(-0.075f, 0.12f, 0.75f),
            new Vector3(0, 0.12f, 0.75f),
            new Vector3(0, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.06375f, 0.675f),
            new Vector3(-0.075f, 0.007499993f, 0.75f),
            new Vector3(0, 0.007499993f, 0.75f),
            new Vector3(0, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.04875001f, 0.625f),
            new Vector3(-0.075f, -0.09562501f, 0.6625f),
            new Vector3(0, -0.09562501f, 0.6625f),
            new Vector3(-0.075f, -0.105f, 0.5f),
            new Vector3(-0.075f, -0.18f, 0.475f),
            new Vector3(0, -0.18f, 0.475f),
            new Vector3(0, 0.02624997f, -0.425f),
            new Vector3(-0.165f, 0.02624997f, -0.425f),
            new Vector3(-0.165f, -0.18f, -0.425f),
            new Vector3(0, -0.18f, -0.425f),
            new Vector3(0, 0.02624997f, -0.65f),
            new Vector3(-0.165f, 0.02624997f, -0.65f),
            new Vector3(-0.165f, -0.12375f, -0.775f),
            new Vector3(0, -0.12375f, -0.775f),
            new Vector3(0, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.195f, -0.575f),
            new Vector3(-0.0625f, 0.17625f, -0.6f),
            new Vector3(0, 0.17625f, -0.6f),
            new Vector3(0, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.675f),
            new Vector3(-0.0625f, 0.27f, -0.825f),
            new Vector3(0, 0.27f, -0.825f),
            new Vector3(0, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.28875f, -0.7f),
            new Vector3(-0.0625f, 0.2934375f, -0.88125f),
            new Vector3(0, 0.2934375f, -0.88125f),
            new Vector3(0, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.28875f, -0.725f),
            new Vector3(-0.0375f, 0.298125f, -0.8625f),
            new Vector3(0, 0.298125f, -0.8625f),
            new Vector3(0, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.7f),
            new Vector3(-0.0375f, 0.27f, -0.8f),
            new Vector3(0, 0.27f, -0.8f),
            new Vector3(0, 0.4575f, -0),
            new Vector3(0, 0.4575f, -0.2f),
            new Vector3(-0.1125f, 0.4575f, -0.2f),
            new Vector3(-0.2f, 0.4575f, -0.1125f),
            new Vector3(-0.2f, 0.4575f, -0),
            new Vector3(0, 0.3825f, -0),
            new Vector3(0, 0.27f, -0.35f),
            new Vector3(-0.196f, 0.27f, -0.35f),
            new Vector3(-0.35f, 0.27f, -0.196f),
            new Vector3(-0.35f, 0.27f, -0),
            new Vector3(0, 0.3075f, -0.1f),
            new Vector3(-0.056f, 0.3075f, -0.1f),
            new Vector3(-0.1f, 0.3075f, -0.056f),
            new Vector3(-0.1f, 0.3075f, -0),
            new Vector3(0, 0.3075f, -0.325f),
            new Vector3(-0.182f, 0.3075f, -0.325f),
            new Vector3(-0.325f, 0.3075f, -0.182f),
            new Vector3(-0.325f, 0.3075f, -0),
            new Vector3(0, 0.27f, -0.325f),
            new Vector3(-0.182f, 0.27f, -0.325f),
            new Vector3(-0.325f, 0.27f, -0.182f),
            new Vector3(-0.325f, 0.27f, -0),
            new Vector3(0, -0.33f, -0),
            new Vector3(-0.1995f, -0.33f, -0.35625f),
            new Vector3(0, -0.31125f, -0.375f),
            new Vector3(0, -0.33f, -0.35625f),
            new Vector3(-0.35625f, -0.33f, -0.1995f),
            new Vector3(-0.375f, -0.31125f, -0),
            new Vector3(-0.35625f, -0.33f, -0),
            new Vector3(-0.21f, -0.31125f, -0.375f),
            new Vector3(-0.375f, -0.31125f, -0.21f),
        };


        // Tessellates the specified bezier patch.
        private static void TessellatePatch(List<VertexPositionNormalTexture> vertices, List<int> indices,
            ref TeapotPatch patch, int tessellation, Vector3 scale, Vector2 tileFactor,
            bool isMirrored)
        {
            // Look up the 16 control points for this patch.
            var controlPoints = new Vector3[16];

            for (int i = 0; i < 16; i++)
            {
                controlPoints[i] = TeapotControlPoints[patch.indices[i]]*scale;
            }

            // Create the index data.
            int vbase = vertices.Count;
            indices.AddRange(CreatePatchIndices(tessellation, isMirrored, vbase));
            CreatePatchVertices(controlPoints, tessellation, isMirrored, tileFactor, vertices);
        }

        public void GenerateMesh(out VertexPositionNormalTexture[] vertices, out int[] indices)
        {
            var vertexList = new List<VertexPositionNormalTexture>();
            var indexList = new List<int>();

            var scaleVector = new Vector3(size, size, size);
            var scaleNegateX = scaleVector;
            scaleNegateX.X = -scaleNegateX.X;
            var scaleNegateZ = scaleVector;
            scaleNegateZ.Z = -scaleNegateZ.Z;
            var scaleNegateXZ = new Vector3(-size, size, -size);

            for (int i = 0; i < TeapotPatches.Length; i++)
            {
                var patch = TeapotPatches[i];

                // Because the teapot is symmetrical from left to right, we only store
                // data for one side, then tessellate each patch twice, mirroring in X.
                TessellatePatch(vertexList, indexList, ref patch, tessellation, scaleVector, tileFactor, false);
                TessellatePatch(vertexList, indexList, ref patch, tessellation, scaleNegateX, tileFactor, true);

                if (patch.mirrorZ)
                {
                    // Some parts of the teapot (the body, lid, and rim, but not the
                    // handle or spout) are also symmetrical from front to back, so
                    // we tessellate them four times, mirroring in Z as well as X.
                    TessellatePatch(vertexList, indexList, ref patch, tessellation, scaleNegateZ, tileFactor, true);
                    TessellatePatch(vertexList, indexList, ref patch, tessellation, scaleNegateXZ, tileFactor, false);
                }
            }

            vertices = vertexList.ToArray();
            indices = indexList.ToArray();
        }




        // Performs a cubic bezier interpolation between four control points,
        // returning the value at the specified time (t ranges 0 to 1).
        // This template implementation can be used to interpolate Vector3,
        // float, or any other types that define suitable * and + operators.
        public static Vector3 CubicInterpolate(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 p4, float t)
        {
            var t2 = t*t;
            var onet2 = (1 - t)*(1 - t);
            return p1*(1 - t)*onet2 +
                   p2*3*t*onet2 +
                   p3*3*t2*(1 - t) +
                   p4*t*t2;
        }


        // Computes the tangent of a cubic bezier curve at the specified time.
        // Template supports Vector3, float, or any other types with * and + operators.
        private static Vector3 CubicTangent(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 p4, float t)
        {
            var t2 = t*t;
            return p1*(-1 + 2*t - t2) +
                   p2*(1 - 4*t + 3*t2) +
                   p3*(2*t - 3*t2) +
                   p4*(t2);
        }

        // Creates vertices for a patch that is tessellated at the specified level.
        // Calls the specified outputVertex function for each generated vertex,
        // passing the position, normal, and texture coordinate as parameters.
        private static void CreatePatchVertices(Vector3[] patch, int tessellation, bool isMirrored, Vector2 tileFactor,
            List<VertexPositionNormalTexture> outputList)
        {
            for (int i = 0; i <= tessellation; i++)
            {
                float u = (float) i/tessellation;

                for (int j = 0; j <= tessellation; j++)
                {
                    float v = (float) j/tessellation;

                    // Perform four horizontal bezier interpolations
                    // between the control points of this patch.
                    var p1 = CubicInterpolate(ref patch[0], ref patch[1], ref patch[2], ref patch[3], u);
                    var p2 = CubicInterpolate(ref patch[4], ref patch[5], ref patch[6], ref patch[7], u);
                    var p3 = CubicInterpolate(ref patch[8], ref patch[9], ref patch[10], ref patch[11], u);
                    var p4 = CubicInterpolate(ref patch[12], ref patch[13], ref patch[14], ref patch[15], u);

                    // Perform a vertical interpolation between the results of the
                    // previous horizontal interpolations, to compute the position.
                    var position = CubicInterpolate(ref p1, ref p2, ref p3, ref p4, v);

                    // Perform another four bezier interpolations between the control
                    // points, but this time vertically rather than horizontally.
                    var q1 = CubicInterpolate(ref patch[0], ref patch[4], ref patch[8], ref patch[12], v);
                    var q2 = CubicInterpolate(ref patch[1], ref patch[5], ref patch[9], ref patch[13], v);
                    var q3 = CubicInterpolate(ref patch[2], ref patch[6], ref patch[10], ref patch[14], v);
                    var q4 = CubicInterpolate(ref patch[3], ref patch[7], ref patch[11], ref patch[15], v);

                    // Compute vertical and horizontal tangent vectors.
                    var tangent1 = CubicTangent(ref p1, ref p2, ref p3, ref p4, v);
                    var tangent2 = CubicTangent(ref q1, ref q2, ref q3, ref q4, u);

                    // Cross the two tangent vectors to compute the normal.
                    var normal = Vector3.Cross(tangent1, tangent2);

                    if (!Vector3.NearEqual(normal, Vector3.Zero, new Vector3(1e-7f)))
                    {
                        normal.Normalize();

                        // If this patch is mirrored, we must invert the normal.
                        if (isMirrored)
                        {
                            normal = -normal;
                        }
                    }
                    else
                    {
                        // In a tidy and well constructed bezier patch, the preceding
                        // normal computation will always work. But the classic teapot
                        // model is not tidy or well constructed! At the top and bottom
                        // of the teapot, it contains degenerate geometry where a patch
                        // has several control points in the same place, which causes
                        // the tangent computation to fail and produce a zero normal.
                        // We 'fix' these cases by just hard-coding a normal that points
                        // either straight up or straight down, depending on whether we
                        // are on the top or bottom of the teapot. This is not a robust
                        // solution for all possible degenerate bezier patches, but hey,
                        // it's good enough to make the teapot work correctly!
                        normal.X = 0.0f;
                        normal.Y = position.Y < 0.0f ? -1.0f : 1.0f;
                        normal.Z = 0.0f;
                    }

                    // Compute the texture coordinate.
                    float mirroredU = isMirrored ? 1 - u : u;

                    var textureCoordinate = new Vector2(mirroredU, v) * tileFactor;

                    // Output this vertex.
                    outputList.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));
                }
            }
        }

        // Creates indices for a patch that is tessellated at the specified level.
        // Calls the specified outputIndex function for each generated index value.
        private static IEnumerable<int> CreatePatchIndices(int tessellation, bool isMirrored, int baseIndex)
        {
            int stride = tessellation + 1;
            // Make a list of six index values (two triangles).
            var indices = new int[6];

            for (int i = 0; i < tessellation; i++)
            {
                for (int j = 0; j < tessellation; j++)
                {
                    indices[0] = baseIndex + i*stride + j;
                    indices[1] = baseIndex + (i + 1)*stride + j;
                    indices[2] = baseIndex + (i + 1)*stride + j + 1;
                    indices[3] = baseIndex + i*stride + j;
                    indices[4] = baseIndex + (i + 1)*stride + j + 1;
                    indices[5] = baseIndex + i*stride + j + 1;

                    // If this patch is mirrored, reverse indices to fix the winding order.
                    if (isMirrored)
                    {
                        Array.Reverse(indices);
                    }

                    foreach (var index in indices)
                    {
                        yield return index;
                    }
                }
            }
        }

        public static Model New(DirectXDevice device, float size = 1.0f, int tessellation = 8, float tileX = 1.0f,
            float tileY = 1.0f, PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList, ModelOperation modelOperations = ModelOperation.None)
        {
            var teapot = new TeapotMesh(size, tileX, tileY, tessellation);
            VertexPositionNormalTexture[] vertices;
            int[] indices;
            teapot.GenerateMesh(out vertices, out indices);
            return GeometricPrimitive.New(device, "Teapot", vertices, indices, primitiveTopology, modelOperations);
        }
    }
}
