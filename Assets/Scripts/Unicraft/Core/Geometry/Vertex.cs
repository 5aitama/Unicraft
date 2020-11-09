using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.Collections;

using System.Runtime.InteropServices;

namespace Unicraft.Core.Geometry
{
    /// <summary>
    /// Vertex representation (without color)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {   
        /// <summary>
        /// Vertex position
        /// </summary>
        public float3 pos;

        /// <summary>
        /// Vertex normal
        /// </summary>
        public float3 norm;

        /// <summary>
        /// Vertex texture coordinate
        /// </summary>
        public float2 uv0;

        /// <summary>
        /// Create new vertex.
        /// </summary>
        /// <param name="pos">Vertex position</param>
        /// <param name="norm">Vertex normal</param>
        /// <param name="uvCoordinate">Vertex Uv coordinate</param>
        public Vertex(in float3 pos, in float3 norm, in float2 uvCoordinate)
        {
            this.pos = pos;
            this.norm = norm;
            uv0 = uvCoordinate;
        }

        /// <summary>
        /// Descriptors for Vertex.
        /// </summary>
        /// <param name="allocator">Allocator for descriptor array</param>
        public static NativeArray<VertexAttributeDescriptor> Descriptors(Allocator allocator)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(3, allocator);

            descriptors[0] = new VertexAttributeDescriptor(VertexAttribute.Position  , VertexAttributeFormat.Float32, 3, 0);
            descriptors[1] = new VertexAttributeDescriptor(VertexAttribute.Normal    , VertexAttributeFormat.Float32, 3, 0);
            descriptors[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0 , VertexAttributeFormat.Float32, 2, 0);

            return descriptors;
        }
    }

    /// <summary>
    /// Vertex representation (with color)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexColor
    {
        /// <summary>
        /// Vertex position
        /// </summary>
        public float3 pos;

        /// <summary>
        /// Vertex normal
        /// </summary>
        public float3 norm;

        /// <summary>
        /// Vertex color
        /// </summary>
        public float4 col;

        /// <summary>
        /// Descriptors for VertexColor.
        /// </summary>
        /// <param name="allocator">Allocator for descriptor array</param>
        public static NativeArray<VertexAttributeDescriptor> Descriptors(Allocator allocator)
        {
            var descriptors = new NativeArray<VertexAttributeDescriptor>(3, allocator);

            descriptors[0] = new VertexAttributeDescriptor(VertexAttribute.Position  , VertexAttributeFormat.Float32, 3);
            descriptors[1] = new VertexAttributeDescriptor(VertexAttribute.Normal    , VertexAttributeFormat.Float32, 3);
            descriptors[2] = new VertexAttributeDescriptor(VertexAttribute.Color     , VertexAttributeFormat.Float32, 4);

            return descriptors;
        }
    }
}