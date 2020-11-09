using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

namespace Unicraft.Core.Geometry
{
    /// <summary>
    /// Methods to create/update mesh easily
    /// </summary>
    public static class PMesh
    {
        /// <summary>
        /// Job that convert array of vertex, normal and uv
        /// to a single Vertex array.
        /// </summary>
        [BurstCompile]
        private struct ToVertexArrayJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float3> vertices;

            [ReadOnly]
            public NativeArray<float3> normals;

            [ReadOnly]
            public NativeArray<float2> uvs;

            [WriteOnly]
            public NativeArray<Vertex> outputVertices;

            public void Execute(int index)
            {
                outputVertices[index] = new Vertex 
                {
                    pos     = vertices[index],
                    norm    = normals[index],
                    uv0     = uvs[index],
                };
            }
        }

        /// <summary>
        /// Job that convert array of vertex, normal and color
        /// to a single VertexColor array.
        /// </summary>
        [BurstCompile]
        private struct ToColorVertexArrayJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float3> vertices;

            [ReadOnly]
            public NativeArray<float3> normals;

            [ReadOnly]
            public NativeArray<float4> colors;

            [WriteOnly]
            public NativeArray<VertexColor> outputVertices;

            public void Execute(int index)
            {
                outputVertices[index] = new VertexColor 
                {
                    pos     = vertices[index],
                    norm    = normals[index],
                    col     = colors[index],
                };
            }
        }

        /// <summary>
        /// Create vertex array from 3 separate array (positions, normals, uvs).
        /// </summary>
        /// <param name="vertices">Vertex array created</param>
        /// <param name="p">Array that contains position of each vertex</param>
        /// <param name="n">Array that contains normal vector of each vertex</param>
        /// <param name="u">Array that contains uv coordinate of each vertex</param>
        /// <param name="allocator">Allocator of the vertex array</param>
        /// <param name="options">Option of the vertex array</param>
        /// <param name="previousJob">Previous job</param>
        public static JobHandle ToVertexArray(out NativeArray<Vertex> vertices, NativeArray<float3> p, NativeArray<float3> n, NativeArray<float2> u, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory, JobHandle previousJob = default)
        {
            if(p.Length != n.Length)
                throw new System.Exception("position and normal array must be have same size!");

            vertices = new NativeArray<Vertex>(p.Length, allocator, options);

            return new ToVertexArrayJob
            {
                vertices       = p,
                normals        = n,
                uvs            = u,
                outputVertices = vertices,
            }.Schedule(vertices.Length, 64, previousJob);
        }

        /// <summary>
        /// Create vertex array from 3 separate array (positions, normals, colors).
        /// </summary>
        /// <param name="vertices">Vertex array created</param>
        /// <param name="p">Array that contains position of each vertex</param>
        /// <param name="n">Array that contains normal vector of each vertex</param>
        /// <param name="c">Array that contains color of each vertex</param>
        /// <param name="allocator">Allocator of the vertex array</param>
        /// <param name="options">Option of the vertex array</param>
        /// <param name="previousJob">Previous job</param>
        public static JobHandle ToColorVertexArray(out NativeArray<VertexColor> vertices, NativeArray<float3> p, NativeArray<float3> n, NativeArray<float4> c, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory, JobHandle previousJob = default)
        {
            if(p.Length != n.Length)
                throw new System.Exception("position and normal array must be have same size!");

            vertices = new NativeArray<VertexColor>(p.Length, allocator, options);

            return new ToColorVertexArrayJob
            {
                vertices       = p,
                normals        = n,
                colors         = c,
                outputVertices = vertices,
            }.Schedule(vertices.Length, 64, previousJob);
        }

        /// <summary>
        /// Create mesh from triangle and vertex array.
        /// </summary>
        /// <param name="mesh">The mesh to be initialized with triangle and vertex array</param>
        /// <param name="triangles">The array that contains triangle (index)</param>
        /// <param name="vertices">The array that contains vertex</param>
        public static Mesh Create(this Mesh mesh, in NativeArray<Triangle> triangles, in NativeArray<Vertex> vertices)
            => Create(mesh, triangles, vertices, Vertex.Descriptors(Allocator.Temp));

        /// <summary>
        /// Create mesh from triangle and color vertex array.
        /// </summary>
        /// <param name="mesh">The mesh to be initialized with triangle and vertex array</param>
        /// <param name="triangles">The array that contains triangle (index)</param>
        /// <param name="vertices">The array that contains vertex</param>
        public static Mesh Create(this Mesh mesh, in NativeArray<Triangle> triangles, in NativeArray<VertexColor> vertices)
            => Create(mesh, triangles, vertices, VertexColor.Descriptors(Allocator.Temp));

        private static Mesh Create<T>(Mesh mesh, in NativeArray<Triangle> triangles, in NativeArray<T> vertices, in NativeArray<VertexAttributeDescriptor> descriptors) where T : struct
        {
            mesh.Clear();

            mesh.SetVertexBufferParams(vertices.Length, descriptors.ToArray());
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);

            mesh.SetIndexBufferParams(triangles.Length * 3, IndexFormat.UInt32);
            mesh.SetIndexBufferData(triangles, 0, 0, triangles.Length);

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new SubMeshDescriptor(0, triangles.Length * 3, MeshTopology.Triangles));

            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Create a new Mesh.
        /// </summary>
        /// <param name="triangles">Triangle array</param>
        /// <param name="vertices">Vertex array</param>
        public static Mesh Create(in NativeArray<Triangle> triangles, in NativeArray<Vertex> vertices) 
            => new Mesh().Create(triangles, vertices);
        
        /// <summary>
        /// Create a new Mesh.
        /// </summary>
        /// <param name="triangles">Triangle array</param>
        /// <param name="vertices">Color Vertex array</param>
        public static Mesh Create(in NativeArray<Triangle> triangles, in NativeArray<VertexColor> vertices) 
            => new Mesh().Create(triangles, vertices);

        /// <summary>
        /// Update Mesh with new geometry
        /// </summary>
        /// <param name="mesh">Mesh to be update</param>
        /// <param name="triangles">New triangle array</param>
        /// <param name="vertices">New vertex array</param>
        public static void Update(this Mesh mesh, in NativeArray<Triangle> triangles, in NativeArray<Vertex> vertices)
            => Update(mesh, triangles, vertices, Vertex.Descriptors(Allocator.Temp));

        private static void Update<T>(Mesh mesh, in NativeArray<Triangle> triangles, in NativeArray<T> vertices, in NativeArray<VertexAttributeDescriptor> descriptors) where T : struct
        {
            mesh.Clear();

            mesh.SetVertexBufferParams(vertices.Length, descriptors.ToArray());
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);

            mesh.SetIndexBufferParams(triangles.Length * 3, IndexFormat.UInt32);
            mesh.SetIndexBufferData(triangles, 0, 0, triangles.Length);

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new SubMeshDescriptor(0, triangles.Length * 3, MeshTopology.Triangles));

            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Update Mesh vertices
        /// </summary>
        /// <param name="mesh">Mesh to be update</param>
        /// <param name="vertices">New vertex array</param>
        public static void Update(this Mesh mesh, in NativeArray<Vertex> vertices)
        {
            mesh.SetVertexBufferParams(vertices.Length, Vertex.Descriptors(Allocator.Temp).ToArray());
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
        }

        /// <summary>
        /// Update Mesh vertices
        /// </summary>
        /// <param name="mesh">Mesh to be update</param>
        /// <param name="vertices">New vertex array</param>
        public static void Update(this Mesh mesh, in NativeArray<VertexColor> vertices)
        {
            mesh.SetVertexBufferParams(vertices.Length, VertexColor.Descriptors(Allocator.Temp).ToArray());
            mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
        }

        /// <summary>
        /// Update Mesh triangles
        /// </summary>
        /// <param name="mesh">Mesh to be update</param>
        /// <param name="triangles">New triangle array</param>
        public static void Update(this Mesh mesh, in NativeArray<Triangle> triangles)
        {
            mesh.SetIndexBufferParams(triangles.Length * 3, IndexFormat.UInt32);
            mesh.SetIndexBufferData(triangles, 0, 0, triangles.Length);

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new SubMeshDescriptor(0, triangles.Length * 3, MeshTopology.Triangles));
        }
    }
}