using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Engine;
using SharpDX;
using SharpDX.Direct3D;

namespace Odyssey.Graphics.Models
{
    public class GeometricPrimitive<TVertex> : Component
        where TVertex: struct
    {
        private readonly PrimitiveType primitiveType;
        private readonly static VertexInputLayout InputLayout = VertexInputLayout.New<TVertex>(0);

        public PrimitiveType PrimitiveType { get { return primitiveType; } }
        public int VertexCount { get { return Vertices.Length; } }
        public int IndexCount { get { return Indices.Length; } }

        protected TVertex[] Vertices { get; set; }
        protected int[] Indices { get; set; }
        
        protected GeometricPrimitive(string name, PrimitiveType primitiveType)
        {
            Name = name;
            this.primitiveType = primitiveType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometricPrimitive" /> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vertices">The vertices described in right handed form.</param>
        /// <param name="indices">The indices described in right handed form.</param>
        /// <param name="primitiveType"></param>
        /// <param name="toLeftHanded">if set to <c>true</c> vertices and indices will be transformed to left handed. Default is true.</param>
        /// <exception cref="System.InvalidOperationException">Cannot generate more than 65535 indices on feature level HW &lt;= 9.3</exception>
        public GeometricPrimitive(string name, TVertex[] vertices, int[] indices, PrimitiveType primitiveType, bool toLeftHanded = false) : this(name, primitiveType)
        {
            Contract.Requires<ArgumentNullException>(vertices != null, "vertices");
            Vertices = vertices;
            Indices = indices;
            if (toLeftHanded)
                ReverseWinding(vertices, indices);
        }

        /// <summary>
        /// Helper for flipping winding of geometric primitives for LH vs. RH coordinates
        /// </summary>
        /// <typeparam name="TIndex">The type of the T index.</typeparam>
        /// <param name="vertices">The vertices.</param>
        /// <param name="indices">The indices.</param>
        protected virtual void ReverseWinding<TIndex>(TVertex[] vertices, TIndex[] indices)
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                SharpDX.Utilities.Swap(ref indices[i], ref indices[i + 2]);
            }
        }
  
        public static Model New<TVertex>(DirectXDevice device, string name, TVertex[] vertices, int[] indices,
            PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList, ModelOperation modelOperations= ModelOperation.None)
            where TVertex : struct
        {
            bool toLeftHanded = modelOperations.HasFlag(ModelOperation.ReverseIndices);

            return new GeometricPrimitive<TVertex>(name, vertices, indices, primitiveTopology, toLeftHanded).ToModel(device);
        }

        public Model ToModel(DirectXDevice device)
        {
            Buffer indexBuffer = null;
            if (Indices != null)
            {
                if (Indices.Length < 0xFFFF)
                {
                    ushort[] indicesShort = new ushort[Indices.Length];
                    for (int i = 0; i < Indices.Length; i++)
                    {
                        indicesShort[i] = (ushort) Indices[i];
                    }
                    indexBuffer = Buffer.Index.New(device, indicesShort);
                }
                else
                {
                    if (device.Features.Level <= FeatureLevel.Level_9_3)
                    {
                        throw new InvalidOperationException(
                            "Cannot generate more than 65535 indices on feature level HW <= 9.3");
                    }

                    indexBuffer = Buffer.Index.New(device, Indices);
                }
                indexBuffer.DebugName = "IB_" + Name;
            }
            Buffer vertexBuffer = Buffer.Vertex.New(device, Vertices);
            vertexBuffer.DebugName = "VB_" + Name;
            
            return new Model(Name, PrimitiveType, vertexBuffer, InputLayout, indexBuffer);
        }
    }

    public class GeometricPrimitive : GeometricPrimitive<VertexPositionNormalTexture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometricPrimitive" /> class.
        /// </summary>
        /// <param name="name">The name of this <see cref="GeometricPrimitive"/>.</param>
        /// <param name="vertices">The vertices described in right handed form.</param>
        /// <param name="indices">The indices described in right handed form.</param>
        /// <param name="toLeftHanded">if set to <c>true</c> vertices and indices will be transformed to left handed. Default is true.</param>
        /// <exception cref="System.InvalidOperationException">Cannot generate more than 65535 indices on feature level HW &lt;= 9.3</exception>
        private GeometricPrimitive(string name, VertexPositionNormalTexture[] vertices, int[] indices, PrimitiveType primitiveType, bool toLeftHanded = false)
            : base(name, vertices, indices, primitiveType, toLeftHanded)
        {
        }

        protected override void ReverseWinding<TIndex>(VertexPositionNormalTexture[] vertices, TIndex[] indices)
        {
            base.ReverseWinding(vertices, indices);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].TextureUV = new Vector2(1.0f - vertices[i].TextureUV.X, vertices[i].TextureUV.Y);
            }
        }
        public static Model New(DirectXDevice device, string name, VertexPositionNormalTexture[] vertices, int[] indices,
            PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList, ModelOperation modelOperations = ModelOperation.None)
        {
            bool toLeftHanded = modelOperations.HasFlag(ModelOperation.ReverseIndices);
            if (modelOperations.HasFlag(ModelOperation.ReshuffleIndices))
                indices = ModelEditor.RandomizePrimitiveOrder(indices, 2);

            if (modelOperations.HasFlag(ModelOperation.CalculateTangents))
            {
                var tangentVertices = ModelEditor.CalculateTangentArray(vertices, indices);
                return new GeometricPrimitive<VertexPositionNormalTextureTangent>(name, tangentVertices, indices, primitiveTopology, toLeftHanded).ToModel(device);
            }
            else if (modelOperations.HasFlag(ModelOperation.CalculateBarycentricCoordinates))
            {
                var barycentricVertices = ModelEditor.ConvertToBarycentricVertices(vertices, indices);
                return new GeometricPrimitive<VertexPositionNormalTextureBarycentric>(name, barycentricVertices, null, primitiveTopology, false).ToModel(device);
            }
            else if (modelOperations.HasFlag(ModelOperation.CalculateBarycentricCoordinatesAndExcludeEdges))
            {
                var barycentricVertices = ModelEditor.ConvertToBarycentricEdgeNormalVertices(vertices, indices);
                int[] newIndices = Enumerable.Range(0, barycentricVertices.Length).ToArray();
                return new GeometricPrimitive<VertexPositionNormalTextureBarycentric>(name, barycentricVertices, newIndices, primitiveTopology, false).ToModel(device);
            }
            else return new GeometricPrimitive(name, vertices, indices, primitiveTopology, toLeftHanded).ToModel(device);

        }

    }
}
